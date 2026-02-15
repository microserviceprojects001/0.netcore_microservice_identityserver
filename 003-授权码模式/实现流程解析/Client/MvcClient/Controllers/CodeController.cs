using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
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

            // ✅ 显式使用 SHA256 + Base64Url 编码（无填充）
            byte[] hash;
            using (var sha = SHA256.Create())
            {
                hash = sha.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
            }
            var codeChallenge = Base64Url.Encode(hash); // 无填充，且 / 替换为 _

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
                               $"&code_challenge={codeChallenge}" +          // ✅ 现在是无填充的 Base64Url
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
            string code = Request.Form["code"];
            string state = Request.Form["state"];
            string scope = Request.Form["scope"];
            string session_state = Request.Form["session_state"];

            // 验证 state（防 CSRF）
            var savedState = HttpContext.Session.GetString("auth_state");
            if (string.IsNullOrEmpty(state) || state != savedState)
            {
                return BadRequest("Invalid state parameter.");
            }

            ViewBag.code = code;
            ViewBag.scope = scope;
            ViewBag.state = state;
            ViewBag.session_state = session_state;

            return View();
        }

        /// <summary>
        /// 3. 手动触发换取 Token
        /// </summary>
        public async Task<IActionResult> IndexTokenView(string code)
        {
            if (string.IsNullOrEmpty(code))
                return BadRequest("No authorization code provided.");

            var codeVerifier = HttpContext.Session.GetString("code_verifier");
            if (string.IsNullOrEmpty(codeVerifier))
                return BadRequest("No code verifier found. Please start over.");

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

            ViewBag.AccessToken = tokenResponse.AccessToken;
            ViewBag.RefreshToken = tokenResponse.RefreshToken;
            ViewBag.IdentityToken = tokenResponse.IdentityToken;
            ViewBag.ExpiresIn = tokenResponse.ExpiresIn;

            return View();
        }
    }
}