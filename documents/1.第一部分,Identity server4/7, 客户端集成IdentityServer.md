# 回到上级目录 再 new 一个工程

cd C:\Users\Abel\Desktop\技术\NodeJsLearning\Code\1.netcore_microservice\netcore_microservice\projects\IdentityServerSample
dotnet new webapi --name ClientCredentialApi # 默认没有 Controllers 文件夹
dotnet new webapi --name ClientCredentialApi --use-controllers

dotnet run

# 然后浏览器访问

http://localhost:5135/weatherforecast

```
[{"date":"2025-03-28","temperatureC":-12,"temperatureF":11,"summary":"Sweltering"},{"date":"2025-03-29","temperatureC":-13,"temperatureF":9,"summary":"Warm"},{"date":"2025-03-30","temperatureC":-20,"temperatureF":-3,"summary":"Hot"},{"date":"2025-03-31","temperatureC":-19,"temperatureF":-2,"summary":"Freezing"},{"date":"2025-04-01","temperatureC":47,"temperatureF":116,"summary":"Freezing"}]

```

会得到这样的输出

# 添加包

dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer

# 遇到问题

# 问题 1

http://localhost:5001/weatherforecast，报错 InvalidOperationException: The MetadataAddress or Authority must use HTTPS unless disabled for development by setting RequireHttpsMetadata=false.
Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerPostConfigureOptions.PostConfigure(string name, JwtBearerOptions options)
Microsoft.Extensions.Options.OptionsFactory<TOptions>.Create(string name)
System.Lazy<T>.ViaFactory(LazyThreadSafetyMode mode)
System.Lazy<T>.ExecutionAndPublication(LazyHelper executionAndPublication, bool useDefaultConstructor)
System.Lazy<T>.CreateValue()
Microsoft.Extensions.Options.OptionsCache<TOptions>.GetOrAdd<TArg>(string name, Func<string, TArg, TOptions> createOptions, TArg factoryArgument)
Microsoft.Extensions.Options.OptionsMonitor<TOptions>.Get(string name)
Microsoft.AspNetCore.Authentication.AuthenticationHandler<TOptions>.InitializeAsync(AuthenticationScheme scheme, HttpContext context)
Microsoft.AspNetCore.Authentication.AuthenticationHandlerProvider.GetHandlerAsync(HttpContext context, string authenticationScheme)
Microsoft.AspNetCore.Authentication.AuthenticationService.AuthenticateAsync(HttpContext context, string scheme)
Microsoft.AspNetCore.Authentication.AuthenticationMiddleware.Invoke(HttpContext context)
Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddlewareImpl.Invoke(HttpContext context) 我的.net 版本是 9

options.RequireHttpsMetadata = false; 这个我之前了解到，已经在.net 9 中 没有这个属性喽

# 方案

临时绕过 HTTPS（仅开发环境，不推荐）
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
options.Authority = "http://localhost:5000";
options.Audience = "api";

        // 手动配置 MetadataAddress，绕过 HTTPS 检查（仅开发环境！）
        options.ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
            metadataAddress: "http://localhost:5000/.well-known/openid-configuration",
            configRetriever: new OpenIdConnectConfigurationRetriever(),
            docRetriever: new HttpDocumentRetriever { RequireHttps = false } // 关键设置
        );

        options.TokenValidationParameters = new()
        {
            ValidateIssuer = false, // 开发环境可关闭
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidateAudience = true
        };
    });

## 同时需要添加两个包

dotnet add package Microsoft.IdentityModel.Protocols.OpenIdConnect
dotnet add package Microsoft.IdentityModel.Protocols

然后在你的 Program.cs 文件顶部添加：

using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

## 1. 这段代码的作用

（1）ConfigurationManager<OpenIdConnectConfiguration>
它是 动态获取 OpenID Connect 配置（OIDC Metadata） 的核心组件。

默认情况下，.NET 的 JwtBearer 认证会通过 Authority 自动从 /.well-known/openid-configuration 获取元数据（如签名密钥、Token 端点等）。

但在 .NET 9 中，默认强制 HTTPS，所以如果你想用 HTTP，必须手动配置 ConfigurationManager。

（2）metadataAddress
指定 OpenID Connect 元数据地址（通常是 {Authority}/.well-known/openid-configuration）。

例如：

```
metadataAddress: "http://localhost:5000/.well-known/openid-configuration"
```

（3）OpenIdConnectConfigurationRetriever
负责 解析 OpenID Connect 元数据（JSON 格式）并转换为 OpenIdConnectConfiguration 对象。

它决定了如何读取 /.well-known/openid-configuration 返回的 JSON 数据。

（4）HttpDocumentRetriever { RequireHttps = false }
关键设置！ 它控制 是否强制 HTTPS：

RequireHttps = true（默认）：必须 HTTPS，否则报错（.NET 9 默认行为）。

RequireHttps = false：允许 HTTP（仅限开发环境！）。

这样，即使你的 Authority 是 http://localhost:5000，它也不会报错。

## 2. 为什么需要手动配置？

在 .NET 9 中：

RequireHttpsMetadata 属性已被移除，无法直接通过 options.RequireHttpsMetadata = false 禁用 HTTPS。

所以必须 手动构造 ConfigurationManager，并通过 HttpDocumentRetriever.RequireHttps = false 绕过 HTTPS 检查。

## 问题 2

The MetadataAddress or Authority must use HTTPS unless disabled for development by setting RequireHttpsMetadata=false. 我之前遇到这个错误，奥，那你的意思是说，这是访问 IdentityServer 的 metadataAddress 默认走的是 https://localhost:5000/.well-known/openid-configuration ，是 https 吗，然而 我的认证服务器现在不支持 https

## 是的，你的理解是正确的！这个错误的核心原因是：

1. 默认情况下，.NET 的 JWT 认证中间件会：

自动将你配置的 Authority（如 http://localhost:5000）转换为 HTTPS 去访问元数据端点（/.well-known/openid-configuration）

例如，即使你配置 Authority = "http://localhost:5000"，它仍然会尝试访问 https://localhost:5000/.well-known/openid-configuration

2. 你的 IdentityServer 不支持 HTTPS（只运行在 HTTP），所以会报错

## 问题 3

public static IEnumerable<ApiScope> GetApiScopes()
{
return new List<ApiScope>
{
new ApiScope("api", "My API Scope") // 必须和 AllowedScopes 中的名称一致
};
}
我之前为了 API 访问，在 IdentityServer 中加了这个和.AddInMemoryApiScopes(Config.GetApiScopes()) // 新增这一行！，请问这个是.net 9 里才加的吗，我看老师视频中没有这个呀?

老师视频基于 IdentityServer4 的

你老师视频中使用的 IdentityServer4（旧版）和现在项目用的 Duende IdentityServer（新版，即 IdentityServer 的商业版）在 ApiScope 配置上确实有差异。以下是关键区别和迁移指导：

必须单独定义 ApiScope + 关联到 ApiResource

更清晰的职责分离
ApiResource 定义资源（如 API 名称），ApiScope 定义权限（如 api.read、api.write）

### 问题四

UseAuthentication 和 UseAuthorization 的区别

1. UseAuthentication() - 认证中间件
   作用：负责识别用户身份（即"你是谁"）

功能：

解析请求中的身份凭证（如 JWT、Cookie 等）

验证凭证的有效性

创建 ClaimsPrincipal 对象并设置到 HttpContext.User

不决定用户能做什么，只决定用户是谁

在你的代码中的体现：

```
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => { ... });
```

2. UseAuthorization() - 授权中间件
   作用：决定用户是否有权限执行操作（即"你能做什么"）

功能：

检查已认证用户的权限

执行策略检查（如 [Authorize] 属性指定的要求）

验证角色或声明

依赖于 UseAuthentication 先识别用户身份
