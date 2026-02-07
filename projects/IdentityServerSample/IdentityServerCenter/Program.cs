using Duende.IdentityServer;
using Duende.IdentityServer.Models;

var builder = WebApplication.CreateBuilder(args);

// 从 appsettings.json 读取 Urls 配置
var urls = builder.Configuration["Urls"];
if (!string.IsNullOrEmpty(urls))
{
    builder.WebHost.UseUrls(urls);
}
// 添加 OpenAPI 支持
//builder.Services.AddOpenApi();

// 添加 IdentityServer
builder.Services.AddIdentityServer()
    .AddDeveloperSigningCredential()// 开发环境下自动生成签名凭据
    .AddInMemoryApiResources(Config.GetResource())
    .AddInMemoryIdentityResources(Config.GetIdentityResources())
    .AddInMemoryApiScopes(Config.GetApiScopes())  // 新增这一行！
    .AddInMemoryClients(Config.GetClients())
    .AddTestUsers(Config.GetUsers())
    .AddProfileService<CustomProfileService>();

builder.Services.AddControllers();

var app = builder.Build();

// 配置 HTTP 请求管道
// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
// }

// app.UseHttpsRedirection();

// 启用 IdentityServer 认证服务
app.UseIdentityServer();

// var summaries = new[]
// {
//     "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
// };

// app.MapGet("/weatherforecast", () =>
// {
//     var forecast = Enumerable.Range(1, 5).Select(index =>
//         new WeatherForecast
//         (
//             DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//             Random.Shared.Next(-20, 55),
//             summaries[Random.Shared.Next(summaries.Length)]
//         ))
//         .ToArray();
//     return forecast;
// })
// .WithName("GetWeatherForecast");

app.Run();

// record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
// {
//     public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
// }
