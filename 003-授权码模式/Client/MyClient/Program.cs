using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;

namespace MyClient
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                HttpClient client = new HttpClient();
                DiscoveryDocumentResponse disco = client.GetDiscoveryDocumentAsync("https://localhost:5001/").Result;
                if (disco.IsError)
                {
                    Console.WriteLine(disco.Error);
                    return;
                }

                //密码模式获取token
                string accessToken = null;
                {
                    Console.WriteLine("************密码模式获取token*************");
                    TokenResponse tokenResponse = client.RequestPasswordTokenAsync(new PasswordTokenRequest
                    {
                        Address = disco.TokenEndpoint,
                        ClientId = "PassPattern",
                        ClientSecret = "PassPatternSecret",
                        Scope = "Client-ApiScope openid profile", //以空格间隔摆放
                        UserName = "Richard",
                        Password = "Richard"
                    }).Result;
                    accessToken = tokenResponse.AccessToken;
                }

                {
                    Console.WriteLine("************带上token去授权服务器获取用户信息*************");
                    //Url地址：
                    Console.WriteLine(disco.UserInfoEndpoint); //disco.UserInfoEndpoint：获取用户信息的api地址
                    HttpClient apiClient = new HttpClient();
                    //apiClient.SetBearerToken(accessToken);
                    var response = apiClient.GetUserInfoAsync(new UserInfoRequest()
                    {
                        Token = accessToken,
                        Address = disco.UserInfoEndpoint
                    }).Result;
                    if (!response.IsError)
                    {
                        Console.WriteLine(response.Raw);
                    }
                }

                {
                    Console.WriteLine("************调用Api资源没有权限验证的Api资源*************");
                    HttpClient apiClient = new HttpClient();
                  apiClient.SetBearerToken(accessToken);
                    HttpResponseMessage response = apiClient.GetAsync("https://localhost:6001/identity/GetResource").Result;
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine(response.StatusCode);
                    }
                    else
                    {
                        var content = response.Content.ReadAsStringAsync().Result;
                        Console.WriteLine(JArray.Parse(content));
                    }
                }


            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
