namespace _4Module
{
    public class MySettings
    {
        public string ApplicationName { get; set; } = string.Empty;
        public int MaxBooksPerPage { get; set; }
        public ApiSettings ApiSettings { get; set; } = new();
    }

    public class ApiSettings
    {
        public int TimeoutInSeconds { get; set; }  
        public int RetryCount { get; set; }
    }
}
