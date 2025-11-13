using k8s;
using k8s.Models;
using Microsoft.Extensions.Primitives;
using Yarp.ReverseProxy.Configuration;

public class KubernetesServiceDiscovery : IProxyConfigProvider
{
    private readonly IKubernetes _client;
    private readonly ILogger<KubernetesServiceDiscovery> _logger;
    private volatile InMemoryConfig _config;

    public KubernetesServiceDiscovery(ILogger<KubernetesServiceDiscovery> logger)
    {
        _logger = logger;
        _client = new Kubernetes(KubernetesClientConfiguration.InClusterConfig());
        _config = new InMemoryConfig(new List<RouteConfig>(), new List<ClusterConfig>());
        LoadServicesAsync().GetAwaiter().GetResult();
    }


    public IProxyConfig GetConfig() => _config;

    private async Task LoadServicesAsync()
    {
        try
        {
            var services = await _client.CoreV1.ListNamespacedServiceAsync("default");
            var routes = new List<RouteConfig>();
            var clusters = new List<ClusterConfig>();

            var excludeServices = new[] { "kubernetes", "mongodb-internal", "redis-headless" };

            foreach (var svc in services.Items.Where(s => !excludeServices.Contains(s.Metadata.Name)))
            {
                var serviceName = svc.Metadata.Name;
                var port = svc.Spec.Ports?.FirstOrDefault()?.Port ?? 80;

                clusters.Add(new ClusterConfig
                {
                    ClusterId = serviceName,
                    Destinations = new Dictionary<string, DestinationConfig>
                    {
                        [serviceName] = new() { Address = $"http://{serviceName}:{port}" }
                    }
                });

                routes.Add(new RouteConfig
                {
                    RouteId = $"{serviceName}-route",
                    ClusterId = serviceName,
                    Match = new RouteMatch { Path = $"/{serviceName}/{{**catch-all}}" },
                    Transforms = new[]
                    {
                    new Dictionary<string, string> { ["PathPattern"] = "/{**catch-all}" }
                }
                });
            }

            _config = new InMemoryConfig(routes, clusters);
            _logger.LogInformation("Loaded {Count} services from Kubernetes", clusters.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load services from Kubernetes");
        }
    }



    private class InMemoryConfig : IProxyConfig
    {
        private readonly CancellationTokenSource _cts = new();
        public InMemoryConfig(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
        {
            Routes = routes;
            Clusters = clusters;
            ChangeToken = new CancellationChangeToken(_cts.Token);
        }
        public IReadOnlyList<RouteConfig> Routes { get; }
        public IReadOnlyList<ClusterConfig> Clusters { get; }
        public IChangeToken ChangeToken { get; }
    }
}
