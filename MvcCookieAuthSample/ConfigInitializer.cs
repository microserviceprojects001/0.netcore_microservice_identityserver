using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;

namespace MvcCookieAuthSample.Services;

public class ConfigInitializer
{
    public static async Task InitializeAsync(ConfigurationDbContext context)
    {
        //检查是否已存在数据
        if (await context.Clients.AnyAsync()) return;
        if (await context.ApiResources.AnyAsync()) return;
        if (await context.IdentityResources.AnyAsync()) return;
        if (await context.ApiScopes.AnyAsync()) return;

        // 写入配置
        { { await context.Clients.AddRangeAsync(Config.GetClients().Select(c => c.ToEntity())); } }
        { { await context.ApiResources.AddRangeAsync(Config.GetApiResources().Select(c => c.ToEntity())); } }
        { { await context.IdentityResources.AddRangeAsync(Config.GetIdentityResources().Select(r => r.ToEntity())); } }
        { { await context.ApiScopes.AddRangeAsync(Config.GetApiScopes().Select(s => s.ToEntity())); } }

        await context.SaveChangesAsync();
    }
}