# 密码模式

## 1，首先需要向 IdentityServer 中添加修改

添加

config.cs 文件中

```
 new Client
            {
                ClientId = "pwclient",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                ClientSecrets = { new Secret("secret".Sha256()) },
                AllowedScopes = { "api" }  // 这里引用的是 ApiScope 的名称
            }
```

```
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
```

program.cs 文件中

```
.AddTestUsers(Config.GetUsers());
```

## 新建控制台程序 PwdClient

dotnet new console --name PwdClient
dotnet add package IdentityModel --version 7.0.0

# postman 中操作

新建请求 POST /connect/token (Password)
得到的 token 可以应用于
GET/weatherforecast (ClientCredential 测试)

# ClientCredentials 和 用户名密码模式总结

![alt text](<../0.课程截图/第一部分 Identity Server/13，第一部分，ClientCredentials 和 用户名密码模式 总结.png>)
