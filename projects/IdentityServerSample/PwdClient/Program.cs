using IdentityModel.Client;
using System.Net.Http;
using System.Text.Json;

Console.WriteLine("Hello, World!");

// 1. 创建 HttpClient
var httpClient = new HttpClient();

// 2. 获取发现文档
var disco = await httpClient.GetDiscoveryDocumentAsync("http://localhost:5000");
if (disco.IsError)
{
    Console.WriteLine(disco.Error);
    return;
}

// 3. 请求 Token
var passwordTokenResponse = await httpClient.RequestPasswordTokenAsync(new PasswordTokenRequest
{
    Address = disco.TokenEndpoint,
    ClientId = "pwclient",
    ClientSecret = "secret",
    Scope = "openid api profile email custom", // 添加必要的scopes
    UserName = "alice",
    Password = "password"
});

if (passwordTokenResponse.IsError)
{
    Console.WriteLine(passwordTokenResponse.Error);
    return;
}

Console.WriteLine("Token Response:");
Console.WriteLine(passwordTokenResponse.Json);
Console.WriteLine($"Access Token: {passwordTokenResponse.AccessToken}");

// 4. 获取用户信息（新增代码）
try
{
    var userInfoResponse = await httpClient.GetUserInfoAsync(new UserInfoRequest
    {
        Address = disco.UserInfoEndpoint,
        Token = passwordTokenResponse.AccessToken
    });

    if (userInfoResponse.IsError)
    {
        Console.WriteLine($"获取用户信息失败: {userInfoResponse.Error}");
    }
    else
    {
        Console.WriteLine("\n用户信息:");
        Console.WriteLine(JsonSerializer.Serialize(userInfoResponse.Claims,
            new JsonSerializerOptions { WriteIndented = true }));

        // 提取特定声明
        var name = userInfoResponse.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
        var email = userInfoResponse.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
        var sub = userInfoResponse.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

        Console.WriteLine($"\n用户名: {name}");
        Console.WriteLine($"邮箱: {email}");
        Console.WriteLine($"用户ID: {sub}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"获取用户信息时发生异常: {ex.Message}");
}

// 5. 调用受保护的 API
var apiClient = new HttpClient();
apiClient.SetBearerToken(passwordTokenResponse.AccessToken);

var response = await apiClient.GetAsync("http://localhost:5001/weatherforecast");
if (response.IsSuccessStatusCode)
{
    var content = await response.Content.ReadAsStringAsync();
    Console.WriteLine("\n受保护API的响应:");
    Console.WriteLine(content);
}
else
{
    Console.WriteLine($"API调用失败: {response.StatusCode}");
}