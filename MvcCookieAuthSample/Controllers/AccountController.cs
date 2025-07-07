using Duende.IdentityServer;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MvcCookieAuthSample.Models.Account;
using System.Security.Claims;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using MvcCookieAuthSample.Models;

public class AccountController : Controller
{
    private readonly IIdentityServerInteractionService _interaction;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountController(
             UserManager<ApplicationUser> userManager,
             SignInManager<ApplicationUser> signInManager,
             IIdentityServerInteractionService interaction)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _interaction = interaction;

    }

    //private readonly IEnumerable<TestUser> _testUsers; // 直接注入测试用户集合
    //private readonly TestUserStore _users;

    // public AccountController(IIdentityServerInteractionService interaction, TestUserStore users)
    // {
    //     _interaction = interaction;
    //     _users = users;
    // }
    // public AccountController(IIdentityServerInteractionService interaction, IEnumerable<TestUser> testUsers)
    // {
    //     _interaction = interaction;
    //     _testUsers = Config.GetUsers(); // 直接使用注入的实例
    // }


    // 登录页
    [HttpGet]
    public IActionResult Login(string returnUrl)
    {
        //HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        var vm = new LoginViewModel
        {
            ReturnUrl = returnUrl,
            Input = new LoginInputModel()
        };
        return View(vm);
    }

    // 登录页
    [HttpGet]
    public IActionResult Register(string returnUrl = null)
    {
        //HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        var vm = new RegisterViewModel
        {
            ReturnUrl = returnUrl,
            //Input = new RegisterInputModel()
        };
        return View(vm);
    }


    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // 注册成功后，自动登录
                //await _signInManager.SignInAsync(user, isPersistent: false);
                //return Redirect(model.ReturnUrl);
                return RedirectToAction("Login", new { returnUrl = model.ReturnUrl });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
        return View(model);
    }


    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        // 获取授权上下文
        var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);

        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(model.Input.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "用户名或密码错误");
                return View(model); // 返回当前model保持表单数据
            }
            // 验证用户凭据
            if (_signInManager.CheckPasswordSignInAsync(user, model.Input.Password, false).Result.Succeeded)
            {
                // // 创建IdentityServer用户
                // var isuser = new IdentityServerUser(user.SubjectId)
                // {
                //     DisplayName = user.Username,
                //     IdentityProvider = "local",
                //     AdditionalClaims = new[]
                //     {
                //         new Claim(ClaimTypes.Name, user.Username),
                //         new Claim("name", user.Username),
                //         new Claim("email", $"{user.Username}@example.com")
                //     }
                // };

                // // 登录
                // ClaimsPrincipal claimsPrincipal = isuser.CreatePrincipal();
                // await HttpContext.SignInAsync(claimsPrincipal);

                AuthenticationProperties authenticationProperties = null;
                if (model.Input.RememberLogin)
                {
                    authenticationProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1) // 设置过期时间
                    };
                }
                await _signInManager.SignInAsync(user, authenticationProperties);

                // if (context != null)
                // {
                //     // 如果有授权上下文，返回到IdentityServer继续处理
                //     return Redirect(model.ReturnUrl);
                // }
                if (_interaction.IsValidReturnUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }

                // 如果没有返回URL，重定向到首页
                return Redirect("~/");
            }

            ModelState.AddModelError("", "用户名或密码错误");
        }

        // 如果到达这里，说明出现了错误，显示登录表单
        return View(model);
    }
    // 处理登录提交
    // [HttpPost]
    // public async Task<IActionResult> Login(LoginViewModel model)
    // {
    //     if (ModelState.IsValid)
    //     {
    //         var user = _testUsers.FirstOrDefault(u =>
    //             u.Username == model.Input.Username &&
    //             u.Password == model.Input.Password);

    //         if (user == null)
    //         {
    //             ModelState.AddModelError("", "Invalid username or password");
    //             return View(model); // 返回当前model保持表单数据
    //         }

    //         // 安全创建Claims
    //         var claims = new List<Claim>
    //         {
    //             new Claim("sub", user.SubjectId),
    //             new Claim("name", user.Claims?.FirstOrDefault(c => c.Type == "name")?.Value ?? user.Username),
    //             new Claim("preferred_username", user.Username) // OpenID标准声明
    //         };

    //         // 添加所有用户自定义Claim
    //         if (user.Claims != null)
    //         {
    //             claims.AddRange(user.Claims);
    //         }

    //         var identity = new ClaimsIdentity(
    //             claims,
    //             IdentityServerConstants.DefaultCookieAuthenticationScheme); // 使用标准Scheme

    //         await HttpContext.SignInAsync(
    //             new ClaimsPrincipal(identity),
    //             new AuthenticationProperties
    //             {
    //                 IsPersistent = true,
    //                 ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1) // 设置过期时间
    //             });

    //         return Redirect(model.ReturnUrl ?? "~/");
    //     }

    //     // 模型验证失败时保留返回URL
    //     model.ReturnUrl ??= Request.Query["ReturnUrl"];
    //     return View(model);
    // }

    // 注销
    [HttpGet]
    public async Task<IActionResult> Logout(string logoutId)
    {
        // 显示注销确认页（可选）
        var vm = new { LogoutId = logoutId };
        return View(vm);
    }

    // 处理注销提交
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        // 注销用户（清除Cookie）
        //await HttpContext.SignOutAsync();
        //return Redirect(model.ReturnUrl);
        await _signInManager.SignOutAsync();
        // 跳转到IdentityServer的注销端点
        return RedirectToAction("Index", "Home");
    }
}