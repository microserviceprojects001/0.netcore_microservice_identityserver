using MvcCookieAuthSample.Services;
using Microsoft.Extensions.Configuration;
using MvcCookieAuthSample.Data;
using System;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using MvcCookieAuthSample.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.EntityFramework.DbContexts;


//System.Diagnostics.Debugger.Break(); // 强制中断

var builder = WebApplication.CreateBuilder(args);
// 从 appsettings.json 读取 Urls 配置
var urls = builder.Configuration["Urls"];
// if (!string.IsNullOrEmpty(urls))
// {
builder.WebHost.UseUrls("https://localhost:5002");
//}

// Add services to the container.
builder.Services.AddControllersWithViews();
// 先构建 ServiceProvider
var serviceProvider = builder.Services.BuildServiceProvider();
var config = serviceProvider.GetRequiredService<IConfiguration>();  // 现在可以获取配置
var connectionString = config.GetConnectionString("DefaultConnection");

//Configuration.GetConnectionString("DefaultConnection")
builder.Services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlite(connectionString,
                   o => o.MigrationsAssembly(typeof(Program).Assembly.FullName)));

builder.Services.AddIdentity<ApplicationUser, ApplicationUserRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// 添加 IdentityServer
builder.Services.AddIdentityServer()
    // .AddInMemoryApiScopes(Config.GetApiScopes())
    // .AddInMemoryClients(Config.GetClients())
    // .AddInMemoryIdentityResources(Config.GetIdentityResources())
    .AddConfigurationStore(options =>
    {
        options.ConfigureDbContext = builder => builder.UseSqlite(connectionString,
            sql => sql.MigrationsAssembly(typeof(Program).Assembly.FullName));
    })
    .AddOperationalStore(options =>
    {
        options.ConfigureDbContext = builder => builder.UseSqlite(connectionString,
            sql => sql.MigrationsAssembly(typeof(Program).Assembly.FullName));
        // options.EnableTokenCleanup = true; // 启用令牌清理
        // options.TokenCleanupInterval = 30; // 清理间隔，单位分钟
    })
    .AddAspNetIdentity<ApplicationUser>();

builder.Services.AddScoped<IProfileService, ProfileService>(); // 添加 ProfileService;

//builder.Services.AddSingleton(Config.GetUsers()); // 单例模式
// ...其他服务注册...

// 设定密码规则的，
builder.Services.Configure<IdentityOptions>(options =>
{
    // 设置密码复杂度要求
    options.Password.RequireDigit = true; // 是否需要数字
    options.Password.RequiredLength = 6;  // 最小长度
    options.Password.RequireNonAlphanumeric = false; // 是否需要特殊字符
    options.Password.RequireUppercase = true; // 是否需要大写字母
    options.Password.RequireLowercase = true; // 是否需要小写字母

    // 设置锁定选项
    //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    //options.Lockout.MaxFailedAccessAttempts = 5;
});


builder.Services.AddScoped<ConsentService>();


SeedData.EnsureSeedDataAsync(connectionString ?? "Data Source=AspIdUsers.db;");

// ...后续中间件配置...
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
    scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>().Database.Migrate();

    var configDb = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
    {
        {
            await ConfigInitializer.InitializeAsync(configDb);
        }
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStaticFiles(); // 必须在 UseRouting 之前
//app.UseHttpsRedirection();
app.UseRouting();

app.UseIdentityServer();

//app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
