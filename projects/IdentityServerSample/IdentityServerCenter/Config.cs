using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;
public class Config
{
    // 定义 API 资源
    public static IEnumerable<ApiResource> GetResource()
    {
        return new List<ApiResource>
        {
            new ApiResource("api", "My API")
            {
                Scopes = { "api" }  // 关联到 ApiScope
            }
        };
    }

    // 新增：定义 API Scope
    public static IEnumerable<ApiScope> GetApiScopes()
    {
        return new List<ApiScope>
        {
            new ApiScope("api", "My API Scope")  // 必须和 AllowedScopes 中的名称一致
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
                AllowedScopes = { "api" }  // 这里引用的是 ApiScope 的名称
            },
            new Client
            {
                ClientId = "pwclient",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                ClientSecrets = { new Secret("secret".Sha256()) },
                RequireClientSecret = false,
                AllowedScopes = { "api" }  // 这里引用的是 ApiScope 的名称
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
                Password = "password"
            },
            new TestUser
            {
                SubjectId = "2",
                Username = "bob",
                Password = "password"
            }
        };
    }
}

// public class TestUser
// {
//     public string SubjectId { get; set; } = string.Empty;
//     public string Username { get; set; } = string.Empty;
//     public string Password { get; set; } = string.Empty;
// }