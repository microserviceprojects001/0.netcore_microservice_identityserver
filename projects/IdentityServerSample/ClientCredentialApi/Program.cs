using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);
// 从 appsettings.json 读取 Urls 配置
var urls = builder.Configuration["Urls"];
if (!string.IsNullOrEmpty(urls))
{
    builder.WebHost.UseUrls(urls);
}
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// add identity server

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "http://localhost:5000";
        //options.requirehttpsmetadata = false;
        options.Audience = "api";
        // 手动配置 MetadataAddress，绕过 HTTPS 检查（仅开发环境！）
        options.ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
            metadataAddress: "http://localhost:5000/.well-known/openid-configuration",
            configRetriever: new OpenIdConnectConfigurationRetriever(),
            docRetriever: new HttpDocumentRetriever { RequireHttps = false } // 关键设置
        );
        options.TokenValidationParameters = new()
        {

            ValidateIssuer = false,  // 允许 HTTP 开发环境
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidateAudience = true
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// 必须先调用认证
app.UseAuthentication();

// 然后调用授权
app.UseAuthorization(); ;

app.MapControllers();

app.Run();
