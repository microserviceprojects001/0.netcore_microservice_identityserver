# 官方文档链接

https://identityserver4docs.readthedocs.io/zh-cn/latest/index.html
https://identityserver4.readthedocs.io/en/latest/index.html

# Identity server 创建几个步骤

添加 Nuget 包: IdentityServer4
添加 Startup 配置
添加 Config.cs 配置类
更改 Identity server 配置
添加客户端配置

# 操作步骤 执行以下命令

cd C:\Users\Abel\Desktop\技术\NodeJsLearning\Code\1.netcore_microservice\netcore_microservice\projects\IdentityServerSample

dotnet new webapi --name IdentityServerCenter

ctrl + p
输入>nuget (已不生效)

## 对于 .NET 6 及更新版本（Duende.IdentityServer）

cd C:\Users\Abel\Desktop\技术\NodeJsLearning\Code\1.netcore_microservice\netcore_microservice\projects\IdentityServerSample\IdentityServerCenter

dotnet add package Duende.IdentityServer

## 你可以使用以下命令查看已安装的 NuGet 包：

dotnet list package

dotnet restore

## 形成最基本代码后

dotnet run

## 访问 http://localhost:5000/.well-known/openid-configuration

```
{
  "issuer": "http://localhost:5000",
  "jwks_uri": "http://localhost:5000/.well-known/openid-configuration/jwks",
  "authorization_endpoint": "http://localhost:5000/connect/authorize",
  "token_endpoint": "http://localhost:5000/connect/token",
  "userinfo_endpoint": "http://localhost:5000/connect/userinfo",
  "end_session_endpoint": "http://localhost:5000/connect/endsession",
  "check_session_iframe": "http://localhost:5000/connect/checksession",
  "revocation_endpoint": "http://localhost:5000/connect/revocation",
  "introspection_endpoint": "http://localhost:5000/connect/introspect",
  "device_authorization_endpoint": "http://localhost:5000/connect/deviceauthorization",
  "backchannel_authentication_endpoint": "http://localhost:5000/connect/ciba",
  "pushed_authorization_request_endpoint": "http://localhost:5000/connect/par",
  "require_pushed_authorization_requests": false,
  "frontchannel_logout_supported": true,
  "frontchannel_logout_session_supported": true,
  "backchannel_logout_supported": true,
  "backchannel_logout_session_supported": true,
  "scopes_supported": [
    "offline_access"
  ],
  "claims_supported": [],
  "grant_types_supported": [
    "authorization_code",
    "client_credentials",
    "refresh_token",
    "implicit",
    "urn:ietf:params:oauth:grant-type:device_code",
    "urn:openid:params:grant-type:ciba"
  ],
  "response_types_supported": [
    "code",
    "token",
    "id_token",
    "id_token token",
    "code id_token",
    "code token",
    "code id_token token"
  ],
  "response_modes_supported": [
    "form_post",
    "query",
    "fragment"
  ],
  "token_endpoint_auth_methods_supported": [
    "client_secret_basic",
    "client_secret_post"
  ],
  "id_token_signing_alg_values_supported": [
    "RS256"
  ],
  "subject_types_supported": [
    "public"
  ],
  "code_challenge_methods_supported": [
    "plain",
    "S256"
  ],
  "request_parameter_supported": true,
  "request_object_signing_alg_values_supported": [
    "RS256",
    "RS384",
    "RS512",
    "PS256",
    "PS384",
    "PS512",
    "ES256",
    "ES384",
    "ES512",
    "HS256",
    "HS384",
    "HS512"
  ],
  "prompt_values_supported": [
    "none",
    "login",
    "consent",
    "select_account"
  ],
  "authorization_response_iss_parameter_supported": true,
  "backchannel_token_delivery_modes_supported": [
    "poll"
  ],
  "backchannel_user_code_parameter_supported": true,
  "dpop_signing_alg_values_supported": [
    "RS256",
    "RS384",
    "RS512",
    "PS256",
    "PS384",
    "PS512",
    "ES256",
    "ES384",
    "ES512"
  ]
}


```
