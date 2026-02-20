using IdentityServer4.Models;
using IdentityServer4.Services;
using System.Linq;              // 👈 添加此行
using System.Security.Claims;
using System.Threading.Tasks;

public class CustomProfileService : IProfileService
{
    public Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var claims = context.Subject.Claims.ToList();

        // 添加自定义声明
        claims.Add(new Claim("name1", "Alice11"));

        context.IssuedClaims = claims;
        return Task.CompletedTask;
    }

    public Task IsActiveAsync(IsActiveContext context)
    {
        context.IsActive = true;
        return Task.CompletedTask;
    }
}
