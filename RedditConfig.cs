namespace RedditStatisticsApi
{
    public class RedditConfig
    {
        public int FetchIntervalMinutes { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string UserAgent { get; set; }
        public string SubredditName { get; set; }

        public string BaseUrl { get;set; }
        public int FetchLimit { get; set; }
    }
}
