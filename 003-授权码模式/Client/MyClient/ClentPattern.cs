using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MyClient
{
    public class ClentPattern
    {
        public static void Show()
        {
            Console.WriteLine("******************访问发现文档********************");
            HttpClient client = new HttpClient();
            DiscoveryDocumentResponse disco = client.GetDiscoveryDocumentAsync("https://localhost:5001/").Result;
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            string accessToken = null;
            {
                {
                    Console.WriteLine("************客户端模式获取token*************");
                    TokenResponse tokenResponse = client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                    {
                        Address = disco.TokenEndpoint,
                        ClientId = "ClientPattern",
                        ClientSecret = "ClientPatternSecret",
                        Scope = "Client-ApiScope"
                    }).Result;
                    accessToken = tokenResponse.AccessToken;
                }
            }

            //
            {
                Console.WriteLine("************调用Api资源没有权限验证的Api资源*************");
                HttpClient apiClient = new HttpClient();
                HttpResponseMessage response = apiClient.GetAsync("https://localhost:6001/identity/GetInit").Result;
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

            {
                //Console.WriteLine("************调用Api资源，Api资源有权限验证，但是在请求的时候，没有带上Token*************");
                //HttpClient apiClient = new HttpClient();
                //HttpResponseMessage response = apiClient.GetAsync("https://localhost:6001/identity/GetUser").Result;
                //if (!response.IsSuccessStatusCode)
                //{
                //    Console.WriteLine(response.StatusCode);
                //}
                //else
                //{
                //    var content = response.Content.ReadAsStringAsync().Result;
                //    Console.WriteLine(JArray.Parse(content));
                //}
            }

            {
                Console.WriteLine("************带上Token访问Api资源*************");
                HttpClient apiClient = new HttpClient();
                apiClient.SetBearerToken(accessToken);
                HttpResponseMessage response = apiClient.GetAsync("https://localhost:6001/identity/GetUser").Result;
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
    }
}
