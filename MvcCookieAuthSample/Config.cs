using System.Security.Claims;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;
public class Config
{
    // 定义 API 资源
    public static IEnumerable<ApiResource> GetApiResources()
    {
        return new List<ApiResource>
        {
            new ApiResource("api1", "My API")
            {
                Scopes = { "api1" }  // 关联到 ApiScope
            }
        };
    }

    // 新增 IdentityResource 定义
    public static IEnumerable<IdentityResource> GetIdentityResources()
    {
        return new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
        };
    }

    // 新增：定义 API Scope
    public static IEnumerable<ApiScope> GetApiScopes()
    {
        return new List<ApiScope>
        {
            new ApiScope("api1", "My API Scope")  // 必须和 AllowedScopes 中的名称一致
        };
    }

    // 客户端配置
    public static IEnumerable<Client> GetClients()
    {
        return new List<Client>
        {
            new Client
            {
                ClientId = "mvc",
                ClientName = "MVC Client",
                ClientUri = "https://localhost:5003", // 客户端地址
                LogoUri = "https://p7.itc.cn/q_70/images03/20220311/203a337631b1490885368bb14ee3e6ea.jpeg", // 客户端 Logo 地址
                Description = "MVC Client for testing",
                AllowRememberConsent = true, // 允许记住同意

                AllowedGrantTypes = GrantTypes.Code, // Implicit 模式
                ClientSecrets = { new Secret("secret".Sha256()) },
                RedirectUris = { "https://localhost:5003/signin-oidc" },
                PostLogoutRedirectUris = { "https://localhost:5003/signout-callback-oidc" },
                AlwaysIncludeUserClaimsInIdToken = true, // 始终包含用户声明,在IdToken 中包含用户声明
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    IdentityServerConstants.StandardScopes.OfflineAccess, // 允许离线访问
                },

                RequireConsent = true,
                //AllowAccessTokensViaBrowser = true

            }

        };
    }

    // 测试用户
    public static List<TestUser> GetUsers()
    {
        return new List<TestUser>
        {
            new TestUser
            {
                SubjectId = "1000",
                Username = "alice",
                Password = "password",
                Claims = new List<Claim> // 必须初始化Claims集合！
                {
                    new Claim("name", "Alice"), // 添加name claim
                    new Claim("email", "alice@example.com") // 可选
                }
            }
        };
    }
}
