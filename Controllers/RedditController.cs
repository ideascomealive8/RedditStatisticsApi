using Microsoft.AspNetCore.Mvc;

namespace RedditStatisticsApi.Controllers;

[Route("api/Reddit")]
[ApiController]
public class RedditController : ControllerBase
{
    private readonly IRedditService _redditService;

    public RedditController(IRedditService redditService)
    {
        _redditService = redditService;
    }

    [HttpGet("Stats")]
    public async Task<IActionResult> GetRedditStats()
    {
        try
        {
            return Ok(await _redditService.GetRedditStats());
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, null, 500);
        }
    }
}
