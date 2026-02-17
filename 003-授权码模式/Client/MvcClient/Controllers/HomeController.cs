using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MvcClient.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;

namespace MvcClient.Controllers
{
    public class HomeController : Controller
    {
        protected UserIdentity UserIdentity
        {
            get
            {
                // 直接从 User.Claims 中获取信息
                var userIdClaim = User.FindFirst("sub") ?? User.FindFirst(ClaimTypes.NameIdentifier);
                var nameClaim = User.FindFirst("name1") ?? User.FindFirst(ClaimTypes.Name);
                var companyClaim = User.FindFirst("company");
                var titleClaim = User.FindFirst("title");
                var avatarClaim = User.FindFirst("avatar");

                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    return new UserIdentity
                    {
                        UserId = userId,
                        Name = nameClaim?.Value ?? string.Empty,
                        Company = companyClaim?.Value ?? string.Empty,
                        Title = titleClaim?.Value ?? string.Empty,
                        Avatar = avatarClaim?.Value ?? string.Empty
                    };
                }

                // 开发环境回退（仅在无法从 token 获取信息时使用）
                return new UserIdentity
                {
                    UserId = 1,
                    Name = "jesse",
                    Company = "测试公司",
                    Title = "测试职位",
                    Avatar = "https://example.com/avatar.jpg"
                };
            }
        }
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }


        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [Authorize]
        public IActionResult Privacy()
        {
            var user = UserIdentity; // 获取当前用户信息
            return View(user);        // 将用户对象传递给视图
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            // 清除本地 Cookie
            //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            // 触发 OIDC 登出，重定向到 IdentityServer
            // await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);

            // 注意：上面的 SignOutAsync 会处理重定向，不需要返回视图
            // 但如果上面的调用没有自动重定向（例如由于配置问题），可以手动重定向到首页
            //return RedirectToAction("Index");
            // 先清除本地 Cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return SignOut(OpenIdConnectDefaults.AuthenticationScheme);
        }

        public IActionResult Login()
        {
            // 如果用户已登录，直接重定向到首页（可选）
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index");
            }

            // 触发 OpenID Connect 认证挑战，登录成功后重定向到首页
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = "/" // 或 Url.Action("Index", "Home")
            });
        }
    }
}
