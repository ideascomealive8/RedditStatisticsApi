using System.Text.Json.Serialization;

namespace RedditStatisticsApi;

//public class RedditResponse
//{
//    public string kind { get; set; }
//    public RedditData data { get; set; }
//}

public class RedditResponse
{
    public string Kind { get; set; }
    public RedditData Data { get; set; }
}


public class RedditData
{
    public List<RedditPost> Children { get; set; }
    public string After { get; set; }
}

public class RedditPost
{
    public string Kind { get; set; }
    public RedditPostData Data { get; set; }
}

public class RedditPostData
{
    public string Title { get; set; }
    public string Name { get; set; }

    public string Id { get; set; }

    public string Author { get; set; }
    public bool Is_video { get; set; }

    public double Upvote_ratio { get; set; }

    public string Url { get; set; }

    public int Score { get; set; }
    public string After { get; set; }

}


public class RedditStatsOutput
{
    public RedditPostData PostWithMostUpvotes { get; set; }

    public RedditPostData PostWithMostUpvotesRatio { get; set; }

    public List<RedditUserStats> UserPostCountsList { get; set; }

}

public class RedditUserStats
{
    public string Username { get; set; }
    public int PostCount { get; set; }
}


