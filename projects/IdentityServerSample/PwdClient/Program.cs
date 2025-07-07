using IdentityModel.Client;
using System.Net.Http;

Console.WriteLine("Hello, World!");

// 1. 创建 HttpClient
var httpClient = new HttpClient();

// 2. 获取发现文档（新版 API）
var disco = await httpClient.GetDiscoveryDocumentAsync("http://localhost:5000");
if (disco.IsError)
{
    Console.WriteLine(disco.Error);
    return;
}

// 3. 请求 Token（新版 API）

var passwordTokenResponse = await httpClient.RequestPasswordTokenAsync(new PasswordTokenRequest
{
    Address = disco.TokenEndpoint,
    ClientId = "pwclient",
    ClientSecret = "secret",
    Scope = "api",
    UserName = "alice",
    Password = "password"
});

if (passwordTokenResponse.IsError)
{
    Console.WriteLine(passwordTokenResponse.Error);
    return;
}

Console.WriteLine(passwordTokenResponse.Json);
Console.WriteLine(passwordTokenResponse.AccessToken);

// 4. 调用受保护的 API
var apiClient = new HttpClient();
apiClient.SetBearerToken(passwordTokenResponse.AccessToken);

var response = await apiClient.GetAsync("http://localhost:5001/weatherforecast");
if (response.IsSuccessStatusCode)
{
    var content = await response.Content.ReadAsStringAsync();
    Console.WriteLine(content);
}
else
{
    Console.WriteLine(response.StatusCode);
}