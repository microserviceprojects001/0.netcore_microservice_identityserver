using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MvcClient.Models;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http;
using IdentityModel.Client;

namespace MvcClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        //[Authorize(policy: "eMailPolicy")]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <returns></returns>
        public IActionResult Logout()
        {
            base.HttpContext.SignOutAsync().Wait(); 
            return SignOut(CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
        }
         
        public IActionResult Privacy()
        {
            ///Mvc应用程序获取Token
            string accesstoken = HttpContext.AuthenticateAsync().Result.Properties.GetTokenValue("access_token");
            HttpClient httpClient = new HttpClient();
            httpClient.SetBearerToken(accesstoken);
            HttpResponseMessage responseMessage = httpClient.GetAsync("https://localhost:6001/identity/GetInfoResource").Result;
            string result = string.Empty;
            if (responseMessage.IsSuccessStatusCode)
            {
                result = Newtonsoft.Json.JsonConvert.SerializeObject(responseMessage.Content);
            }

            ViewBag.result = result;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
