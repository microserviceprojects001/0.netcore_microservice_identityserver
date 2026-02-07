using Duende.IdentityServer.Models;
using System.Security.Claims;  // 添加这个命名空间
using Duende.IdentityServer.Test;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;

public class Config
{
    // 定义身份资源（用于获取用户信息）
    public static IEnumerable<IdentityResource> GetIdentityResources()
    {
        return new List<IdentityResource>
        {
            new IdentityResources.OpenId(),          // 必需：openid scope
            new IdentityResource(
                name: "profile",
                displayName: "User profile",
                userClaims: new[]
                {
                    "name",
                    "family_name",
                    "given_name",
                    "middle_name",
                    "nickname",
                    "preferred_username",
                    "profile",
                    "picture",
                    "website",
                    "gender",
                    "birthdate",
                    "zoneinfo",
                    "locale",
                    "updated_at",
                    "givenName11"  // 添加自定义声明
                })
                {
                    Description = "Your user profile information"
                },       // 用户基本信息
        new IdentityResources.Email(),            // 用户邮箱
        // 创建自定义身份资源
        new IdentityResource(
                name: "custom",
                displayName: "Custom user information",
                userClaims: new[]
                {
                    "givenName",  // 你的自定义声明
                    "customField1",
                    "customField2"
                })
            {
                Description = "Custom user information"
            },// 创建自定义身份资源
    };
    }

    // 定义 API 资源
    public static IEnumerable<ApiResource> GetResource()
    {
        return new List<ApiResource>
        {
            new ApiResource("api", "My API")
            {
                Scopes = { "api" }
            }
        };
    }

    // 定义 API Scope
    public static IEnumerable<ApiScope> GetApiScopes()
    {
        return new List<ApiScope>
        {
            new ApiScope("api", "My API Scope")
        };
    }

    // 客户端配置
    public static IEnumerable<Client> GetClients()
    {
        return new List<Client>
        {
            new Client
            {
                ClientId = "client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("secret".Sha256()) },
                AllowedScopes = { "api" }
            },
            new Client
            {
                ClientId = "pwclient",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                ClientSecrets = { new Secret("secret".Sha256()) },
                RequireClientSecret = false,
                AllowedScopes = {
                    "api",                     // API 访问权限
                    IdentityServerConstants.StandardScopes.OpenId,    // OpenID Connect
                    IdentityServerConstants.StandardScopes.Profile,   // 用户基本信息
                    IdentityServerConstants.StandardScopes.Email,
                    "custom"  // 添加自定义 scope
                }
            }
        };
    }

    public static List<TestUser> GetUsers()
    {
        return new List<TestUser>
        {
            new TestUser
            {
                SubjectId = "1",
                Username = "alice",
                Password = "password",
                Claims = new List<Claim>  // 使用 System.Security.Claims.Claim
                {
                    new Claim("name", "Alice Smith"),
                    new Claim("email", "alice@example.com"),
                    new Claim("website", "https://alice.com"),
                    new Claim("given_name", "Alice11"),
                    new Claim("givenName11", "Alice1111"),  // 添加自定义声明
                    new Claim("customField1", "Custom Value 1"), // 添加自定义声明
                    new Claim("customField2", "Custom Value 2")  // 添加自定义
                }
            },
            new TestUser
            {
                SubjectId = "2",
                Username = "bob",
                Password = "password",
                Claims = new List<Claim>  // 使用 System.Security.Claims.Claim
                {
                    new Claim("name", "Bob Johnson"),
                    new Claim("email", "bob@example.com")
                }
            }
        };
    }
}