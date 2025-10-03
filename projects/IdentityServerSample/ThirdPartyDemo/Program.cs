// 
// 代码使用了 旧版 IdentityModel (v4.x) 的 API，而最新版 (v7.x) 已经废弃了 DiscoveryClient 和 TokenClient 类


// using IdentityModel.Client;
// // See https://aka.ms/new-console-template for more information
// Console.WriteLine("Hello, World!");

// var diso = await DiscoveryClient.GetAsync("http://localhost:5000");

// if (diso.IsError)
// {
//     Console.WriteLine(diso.Error);
//     return;
// }

// var tokenClient = new TokenClient(diso.TokenEndpoint, "client", "secret");
// var tokenResponse = await tokenClient.RequestClientCredentialsAsync("api");
// if (tokenResponse.IsError)
// {
//     Console.WriteLine(tokenResponse.Error);
//     return;
// }
// else
// {
//     Console.WriteLine(tokenResponse.Json);
//     Console.WriteLine(tokenResponse.AccessToken);
//     Console.WriteLine(tokenResponse.IdentityToken);
//     Console.WriteLine(tokenResponse.RefreshToken);
// }


// HttpClient client = new HttpClient();
// client.SetBearerToken(tokenResponse.AccessToken);
// var response = await client.GetAsync("http://localhost:5001/weatherforecast");
// if (response.IsSuccessStatusCode)
// {
//     var content = await response.Content.ReadAsStringAsync();
//     Console.WriteLine(content);
// }
// else
// {
//     Console.WriteLine(response.StatusCode);
// }

// Client Credentials 方式的用户

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
var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
{
    Address = disco.TokenEndpoint,
    ClientId = "client",
    ClientSecret = "secret",
    Scope = "api"
});

if (tokenResponse.IsError)
{
    Console.WriteLine(tokenResponse.Error);
    return;
}

Console.WriteLine("Json:" + tokenResponse.Json);
Console.WriteLine("Token:" + tokenResponse.AccessToken);

// 4. 调用受保护的 API
var apiClient = new HttpClient();
apiClient.SetBearerToken(tokenResponse.AccessToken);

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
//Console.ReadLine(diso.TokenEndpoint);