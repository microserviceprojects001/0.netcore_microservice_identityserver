using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using MvcCookieAuthSample.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using IdentityModel;
using System.Threading.Tasks;
using Duende.IdentityServer.Extensions;



namespace MvcCookieAuthSample.Services;

public class ProfileService : IProfileService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IIdentityServerInteractionService _interaction;
    private readonly ILogger<ProfileService> _logger;

    public ProfileService(
        UserManager<ApplicationUser> userManager,
        IIdentityServerInteractionService interaction,
        ILogger<ProfileService> logger)
    {
        _userManager = userManager;
        _interaction = interaction;
        _logger = logger;
    }

    private async Task<List<Claim>> GetUserClaimsAsync(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtClaimTypes.Subject, user.Id.ToString()),
            new Claim(JwtClaimTypes.PreferredUserName, user.UserName),

        };

        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(JwtClaimTypes.Role, role));
        }

        if (!string.IsNullOrEmpty(user.Avatar))
        {
            claims.Add(new Claim("Avatar", user.Avatar));
        }
        return claims;
    }
    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var user = await _userManager.GetUserAsync(context.Subject);
        if (user == null)
        {
            _logger.LogError("User not found: {SubjectId}", context.Subject.GetSubjectId());
            return;
        }
        //var claims = await _userManager.GetClaimsAsync(user);
        var claims = await GetUserClaimsAsync(user);
        context.IssuedClaims.AddRange(claims);
    }
    // https://jd.bai.gay/#/dashboard

    // public Task GetProfileDataAsync(ProfileDataRequestContext context)
    // {
    //     throw new NotImplementedException();
    // }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        context.IsActive = false;
        var user = await _userManager.GetUserAsync(context.Subject);
        context.IsActive = user != null;


    }
}