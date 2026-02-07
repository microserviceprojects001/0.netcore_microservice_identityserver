using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ClientCredentialApi.Controllers;


[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [Authorize]  //如果标记了Authorize特性，用户未通过身份验证将无法访问此端点
    [HttpGet("GetWeatherForecast")]
    public IActionResult Get()
    {
        return Ok(from c in User.Claims select new { c.Type, c.Value });
    }

    [HttpGet("clientTest")] // 访问路径：/WeatherForecast/clientTest
    public IActionResult clientTest()
    {
        return Ok(from c in User.Claims select new { c.Type, c.Value });
    }
}
