// Copyright (c) Duende Software. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using MvcCookieAuthSample.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using MvcCookieAuthSample.Data;

public class SeedData
{
    public static async Task EnsureSeedDataAsync(string connectionString)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDbContext<ApplicationDbContext>(options =>
           options.UseSqlite(connectionString, o => o.MigrationsAssembly(typeof(Program).Assembly.FullName)));

        services.AddIdentity<ApplicationUser, ApplicationUserRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        using (var serviceProvider = services.BuildServiceProvider())
        {
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                context.Database.Migrate();

                RoleManager<ApplicationUserRole> roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationUserRole>>();

                // 1. 先创建角色 }}
                var adminRole = await roleMgr.FindByNameAsync("admin1");
                if (adminRole == null)
                {
                    var role = new ApplicationUserRole("admin1");
                    var result = await roleMgr.CreateAsync(role);
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                    Log.Debug("admin role created");
                }
                var managerRole = await roleMgr.FindByNameAsync("manager");
                if (managerRole == null)
                {
                    var role = new ApplicationUserRole("manager");
                    var result = await roleMgr.CreateAsync(role);
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                    Log.Debug("manager role created");
                }

                var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                var alice = await userMgr.FindByNameAsync("bob66");
                if (alice == null)
                {
                    alice = new ApplicationUser
                    {
                        UserName = "bob66",
                        Email = "bob66@example.com",
                        EmailConfirmed = true,
                        //SecurityStamp= "admin",
                        Avatar = "https://p7.itc.cn/q_70/images03/20220311/203a337631b1490885368bb14ee3e6ea.jpeg"
                    };

                    var result = await userMgr.CreateAsync(alice, "ACSdev312!@");
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    var addRoleResult = await userMgr.AddToRoleAsync(alice, "admin1");
                    if (!addRoleResult.Succeeded)
                    {
                        throw new Exception(addRoleResult.Errors.First().Description);
                    }
                    var addmanagerRoleResult = await userMgr.AddToRoleAsync(alice, "manager");
                    if (!addmanagerRoleResult.Succeeded)
                    {
                        throw new Exception(addmanagerRoleResult.Errors.First().Description);
                    }

                    result = userMgr.AddClaimsAsync(alice, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "bob44 Smith"),
                            new Claim(JwtClaimTypes.GivenName, "bob44"),
                            new Claim(JwtClaimTypes.FamilyName, "bob44"),
                            new Claim(JwtClaimTypes.WebSite, "http://bob44.example.com"),
                        }).Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                    Log.Debug("alice created");
                }
                else
                {
                    Log.Debug("alice already exists");
                }

                var bob = userMgr.FindByNameAsync("bob").Result;
                if (bob == null)
                {
                    bob = new ApplicationUser
                    {
                        UserName = "bob",
                        Email = "BobSmith@example.com",
                        EmailConfirmed = true
                    };
                    var result = userMgr.CreateAsync(bob, "Pass123$").Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    result = userMgr.AddClaimsAsync(bob, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Bob Smith"),
                            new Claim(JwtClaimTypes.GivenName, "Bob"),
                            new Claim(JwtClaimTypes.FamilyName, "Smith"),
                            new Claim(JwtClaimTypes.WebSite, "http://bob.example.com"),
                            new Claim("location", "somewhere")
                        }).Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                    Log.Debug("bob created");
                }
                else
                {
                    Log.Debug("bob already exists");
                }
            }
        }
    }
}