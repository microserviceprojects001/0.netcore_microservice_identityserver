using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;

namespace MvcClient.Controllers
{
    public class CodeController : Controller
    {
        /// <summary>
        /// 1. 生成授权链接（PKCE + State + Nonce）
        /// </summary>
        public IActionResult CodeTokenView()
        {
            // 生成 PKCE 参数
            var codeVerifier = CryptoRandom.CreateUniqueId(32);
            var codeChallenge = codeVerifier.ToSha256();

            // 生成 state 和 nonce
            var state = CryptoRandom.CreateUniqueId(16);
            var nonce = CryptoRandom.CreateUniqueId(16);

            // 保存到 Session（供回调时验证、换 Token 时使用）
            HttpContext.Session.SetString("code_verifier", codeVerifier);
            HttpContext.Session.SetString("auth_state", state);
            HttpContext.Session.SetString("auth_nonce", nonce);

            // 构造授权 URL
            var redirectUri = "https://localhost:7001/Code/IndexCodeView";
            var scope = "Client-ApiScope openid profile";

            var authorizeUrl = $"https://localhost:5001/connect/authorize?" +
                               $"client_id={Uri.EscapeDataString("CodeManualTest")}" +
                               $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                               $"&response_type=code" +
                               $"&scope={Uri.EscapeDataString(scope)}" +
                               $"&code_challenge={codeChallenge}" +
                               $"&code_challenge_method=S256" +
                               $"&response_mode=form_post" +
                               $"&state={Uri.EscapeDataString(state)}" +
                               $"&nonce={Uri.EscapeDataString(nonce)}";

            ViewBag.AuthorizeUrl = authorizeUrl;
            return View();
        }

        /// <summary>
        /// 2. 回调接收页面（显示参数，用户手动触发换 Token）
        /// </summary>
        public IActionResult IndexCodeView()
        {
            // ✅ 通过键名获取表单字段（不依赖顺序）
            string code = Request.Form["code"];
            string state = Request.Form["state"];
            string scope = Request.Form["scope"];
            string session_state = Request.Form["session_state"];

            // ✅ 强制验证 state（防 CSRF）
            var savedState = HttpContext.Session.GetString("auth_state");
            if (string.IsNullOrEmpty(state) || state != savedState)
            {
                return BadRequest("Invalid state parameter.");
            }

            // 将接收到的参数存入 ViewBag，供视图显示
            ViewBag.code = code;
            ViewBag.scope = scope;
            ViewBag.state = state;
            ViewBag.session_state = session_state;

            // 返回视图，让用户手动点击“获取Token”
            return View();
        }

        /// <summary>
        /// 3. 手动触发换取 Token（从查询字符串接收 code）
        /// </summary>
        public async Task<IActionResult> IndexTokenView(string code)
        {
            // 检查 code
            if (string.IsNullOrEmpty(code))
                return BadRequest("No authorization code provided.");

            // 从 Session 取出之前保存的 code_verifier
            var codeVerifier = HttpContext.Session.GetString("code_verifier");
            if (string.IsNullOrEmpty(codeVerifier))
                return BadRequest("No code verifier found. Please start over.");

            // 构造 Token 请求
            var httpClient = new HttpClient();
            var tokenResponse = await httpClient.RequestAuthorizationCodeTokenAsync(
                new AuthorizationCodeTokenRequest
                {
                    Address = "https://localhost:5001/connect/token",
                    ClientId = "CodeManualTest",
                    ClientSecret = "CodePatternSecret",
                    Code = code,
                    CodeVerifier = codeVerifier,
                    RedirectUri = "https://localhost:7001/Code/IndexCodeView"
                });

            if (tokenResponse.IsError)
            {
                return Content($"Token 请求失败: {tokenResponse.Error}");
            }

            // 成功，存入 ViewBag 供视图显示
            ViewBag.AccessToken = tokenResponse.AccessToken;
            ViewBag.RefreshToken = tokenResponse.RefreshToken;
            ViewBag.IdentityToken = tokenResponse.IdentityToken;
            ViewBag.ExpiresIn = tokenResponse.ExpiresIn;

            return View(); // 对应视图 IndexTokenView.cshtml
        }
    }
}