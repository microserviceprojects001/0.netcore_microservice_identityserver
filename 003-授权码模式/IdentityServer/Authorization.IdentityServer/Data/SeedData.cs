using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Authorization.IdentityServer.Data;
using Authorization.IdentityServer.Models;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Authorization.IdentityServer.Data
{
    public class SeedData
    {
        public static async Task EnsureSeedDataAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.MigrateAsync(); // 确保数据库已创建

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationUserRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // 创建角色
            string[] roles = { "admin1", "manager" };
            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var role = new ApplicationUserRole(roleName);
                    var result = await roleManager.CreateAsync(role);
                    if (!result.Succeeded)
                    {
                        throw new Exception($"创建角色 {roleName} 失败: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                    Log.Debug($"角色 {roleName} 创建成功");
                }
            }

            // 创建用户 alice (bob66)
            var alice = await userManager.FindByNameAsync("bob66");
            if (alice == null)
            {
                alice = new ApplicationUser
                {
                    UserName = "bob66",
                    Email = "bob66@example.com",
                    EmailConfirmed = true,
                    // 如果有自定义属性，例如 Avatar
                    // Avatar = "https://example.com/avatar.jpg"
                };

                var result = await userManager.CreateAsync(alice, "ACSdev312!@");
                if (!result.Succeeded)
                {
                    throw new Exception($"创建用户 bob66 失败: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }

                // 添加角色
                await userManager.AddToRoleAsync(alice, "admin1");
                await userManager.AddToRoleAsync(alice, "manager");

                // 添加声明
                var claims = new[]
                {
                    new Claim(JwtClaimTypes.Name, "bob44 Smith"),
                    new Claim(JwtClaimTypes.GivenName, "bob44"),
                    new Claim(JwtClaimTypes.FamilyName, "bob44"),
                    new Claim(JwtClaimTypes.WebSite, "http://bob44.example.com")
                };
                await userManager.AddClaimsAsync(alice, claims);

                Log.Debug("用户 bob66 创建成功");
            }
            else
            {
                Log.Debug("用户 bob66 已存在");
            }

            // 创建用户 bob
            var bob = await userManager.FindByNameAsync("bob");
            if (bob == null)
            {
                bob = new ApplicationUser
                {
                    UserName = "bob",
                    Email = "BobSmith@example.com",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(bob, "Pass123$");
                if (!result.Succeeded)
                {
                    throw new Exception($"创建用户 bob 失败: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }

                var claims = new[]
                {
                    new Claim(JwtClaimTypes.Name, "Bob Smith"),
                    new Claim(JwtClaimTypes.GivenName, "Bob"),
                    new Claim(JwtClaimTypes.FamilyName, "Smith"),
                    new Claim(JwtClaimTypes.WebSite, "http://bob.example.com"),
                    new Claim("location", "somewhere")
                };
                await userManager.AddClaimsAsync(bob, claims);

                Log.Debug("用户 bob 创建成功");
            }
            else
            {
                Log.Debug("用户 bob 已存在");
            }
        }
    }
}