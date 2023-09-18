using Microsoft.Extensions.Options;

namespace RedditStatisticsApi;

public class RedditBackgroundService : BackgroundService
{
    private readonly IRedditService _redditService;
    private readonly RedditConfig _redditConfig;

    public RedditBackgroundService(RedditConfig redditConfig, IRedditService redditService)
    {
        _redditConfig = redditConfig;
        _redditService = redditService;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            _redditService.ProcessRedditPosts();
            await Task.Delay(TimeSpan.FromSeconds(_redditConfig.FetchIntervalMinutes), cancellationToken);
        }
    }
}



