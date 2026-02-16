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
    }
}
