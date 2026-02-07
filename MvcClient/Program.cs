using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Text; // 包含 OpenIdConnectDefaults
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// 从 appsettings.json 读取 Urls 配置
var urls = builder.Configuration["Urls"];
if (!string.IsNullOrEmpty(urls))
{
    builder.WebHost.UseUrls(urls);
}

// Add services to the container.
builder.Services.AddControllersWithViews();


JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

builder.Services.AddAuthentication(option =>
{
    option.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    //options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme; // 使用 Cookies 进行身份验证
    options.Authority = "https://localhost:5002"; // IdentityServer 地址使用 HTTPS
    options.RequireHttpsMetadata = true;
    options.ClientId = "mvc";
    options.ClientSecret = "secret";
    options.ResponseType = "code"; // Implicit 模式

    // 配置作用域
    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");
    //options.Scope.Add("offline_access"); // 添加离线访问权限

    options.SaveTokens = true; // 保存访问令牌


    //options.GetClaimsFromUserInfoEndpoint = true;

    //由于IdentityServer端设定client时候，AlwaysIncludeUserClaimsInIdToken = true
    // options.ClaimActions.MapJsonKey("Avatar", "Avatar");  // 映射自定义 claim
    // options.ClaimActions.MapJsonKey("sub", "sub");
    // options.ClaimActions.MapJsonKey("preferred_username", "preferred_username");
    // options.ClaimActions.MapUniqueJsonKey("role", "role");  // 映射角色 claim
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();  // 启用 HSTS
}
app.UseStaticFiles(); // 必须在 UseRouting 之前

app.UseHttpsRedirection(); // 强制使用 HTTPS
app.UseRouting();

// 必须按此顺序！
app.UseAuthentication();  // 认证
app.UseAuthorization();   // 授权

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();