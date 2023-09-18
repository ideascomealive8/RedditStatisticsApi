using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;

namespace RedditStatisticsApi;


public interface IRedditService
{
    Task<RedditStatsOutput> GetRedditStats();
    Task ProcessRedditPosts();
}
public class RedditService : IRedditService
{
    private readonly RedditConfig _redditConfig;
    private static Dictionary<string, int> userPostCounts = new Dictionary<string, int>();
    private static RedditPostData postWithMaxUpvotes = null;
    private static RedditPostData postWithMaxUpmostRatio = null;


    public RedditService(RedditConfig redditConfig) => _redditConfig = redditConfig;

    public async Task ProcessRedditPosts()
    {
        List<RedditPost> allRedditPosts = new();

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(_redditConfig.UserAgent);
        var authHeaderValue = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_redditConfig.ClientId}:{_redditConfig.ClientSecret}"));
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);
        string after = null;

        var sw = new Stopwatch();
        sw.Start();
        while (true)
        {
            var response = await httpClient.GetAsync($"{_redditConfig.BaseUrl}{_redditConfig.SubredditName}/new.json?limit={_redditConfig.FetchLimit}&after={after}");

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                var subRedditData = JsonSerializer.Deserialize<RedditResponse>(content, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true })?.Data;
                var redditPostBatch = subRedditData?.Children;
                if (redditPostBatch != null)
                {
                    allRedditPosts.AddRange(redditPostBatch);
                    after = subRedditData.After;

                    if (string.IsNullOrEmpty(after)) break;
                }
                else break;

            }
            else break;

        }
        sw.Stop();
        var b = sw.ElapsedMilliseconds;  // To determine excecution time in the debug mode.

        userPostCounts.Clear();
        int maxUpvotes = 0;
        double maxUpvotesRatio = 0;

        if (allRedditPosts != null)
        {
            var allRedditPostsData = allRedditPosts.Select(p => p.Data).ToList();
            allRedditPostsData.ForEach(post =>
            {
                var author = post.Author;
                var upvotes = post.Score;
                var upvoteRatio = post.Upvote_ratio;

                if (userPostCounts.ContainsKey(author))
                    userPostCounts[author]++;
                else
                    userPostCounts.Add(author, 1);
                if (upvotes > maxUpvotes)
                {
                    maxUpvotes = upvotes;
                    postWithMaxUpvotes = post;
                }
                if (upvoteRatio > maxUpvotesRatio && upvotes > 0)
                {
                    maxUpvotesRatio = upvoteRatio;
                    postWithMaxUpmostRatio = post;
                }
            });

        }
    }

    public async Task<RedditStatsOutput> GetRedditStats()
    {
        var userStats = new List<RedditUserStats>();
        userPostCounts.OrderByDescending(kv => kv.Value)
            .Take(5)
            .ToList()
            .ForEach(u => userStats.Add(new RedditUserStats() { Username = u.Key, PostCount = u.Value }));

        return new RedditStatsOutput()
        {
            PostWithMostUpvotes = postWithMaxUpvotes,
            UserPostCountsList = userStats,
            PostWithMostUpvotesRatio = postWithMaxUpmostRatio
        };
    }
}

