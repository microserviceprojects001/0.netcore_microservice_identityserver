using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;                           // 包含 UseMySql 扩展
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;      // 包含 MySqlServerVersion
using System;
using System.Linq;
using Authorization.IdentityServer.Models;
using Authorization.IdentityServer.Data;
using Microsoft.AspNetCore.Identity;

namespace Authorization.IdentityServer
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            // 从 appsettings.json 读取连接字符串
            var connectionString = Configuration.GetConnectionString("MySQL");
            Console.WriteLine($"DB Server: {connectionString}");

            // 指定 MySQL 版本（根据你的实际版本修改，例如 8.0.23）
            //var serverVersion = new Pomelo.EntityFrameworkCore.MySql.Infrastructure.MySqlServerVersion(new Version(8, 0, 23));


            // 注册 ApplicationDbContext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(connectionString,
                    mySqlOptions => mySqlOptions.MigrationsAssembly(typeof(Startup).Assembly.FullName)));

            // 注册 Identity
            services.AddIdentity<ApplicationUser, ApplicationUserRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // 配置 IdentityServer 使用数据库存储
            var builder = services.AddIdentityServer(options =>
            {
                options.EmitStaticAudienceClaim = true;
            })
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseMySql(connectionString,
                            mySqlOptions => mySqlOptions.MigrationsAssembly(typeof(Startup).Assembly.FullName));
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseMySql(connectionString,
                            mySqlOptions => mySqlOptions.MigrationsAssembly(typeof(Startup).Assembly.FullName));
                    options.EnableTokenCleanup = true;
                    options.TokenCleanupInterval = 3600;
                })
                .AddAspNetIdentity<ApplicationUser>()   // 使用 Identity 管理用户                // 测试用户（可保留）
                .AddProfileService<CustomProfileService>(); // 自定义 Profile 服务

            // 开发环境临时证书（生产环境请替换为正式证书）
            builder.AddDeveloperSigningCredential();
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // 初始化数据库种子数据（仅在首次运行需要）
            InitializeDatabase(app);
            SeedData.EnsureSeedDataAsync(app.ApplicationServices).Wait();

            app.UseStaticFiles();
            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate(); // 自动创建/更新表结构

                // 如果没有任何客户端，则从 Config 类导入种子数据
                if (!context.Clients.Any())
                {
                    foreach (var client in Config.Clients)
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Config.IdentityResources)
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiScopes.Any())
                {
                    // 注意：循环变量改名，避免与外层 scope 冲突
                    foreach (var apiScope in Config.ApiScopes)
                    {
                        context.ApiScopes.Add(apiScope.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in Config.ApiResources)
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}