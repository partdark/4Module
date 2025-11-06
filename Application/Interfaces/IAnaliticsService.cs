namespace Application.Interfaces
{
    public interface IAnaliticsService
    {
        Task SendEventAsync(string topic, string key, string message);
    }
}