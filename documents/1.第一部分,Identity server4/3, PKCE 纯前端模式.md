# 为什么需要 PKCE？

在 传统的授权码模式 中：

授权码（code）暴露在 URL，攻击者可能拦截并冒充合法应用去换取 access_token。
如果是前端 SPA（单页应用）或移动端，client_secret 无法安全存储，可能被恶意用户获取。
PKCE 解决了这些问题，即使授权码被拦截，攻击者也无法换取令牌。

适用于无后端的前端应用（SPA）

传统 OAuth 需要 client_secret，但前端应用无法安全存储它。
PKCE 允许前端不使用 client_secret 进行 OAuth 认证，提高了安全性
