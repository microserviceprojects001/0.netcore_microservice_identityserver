# 简化模式（Implicit Grant）的完整流程

示例场景：
你打开 简书（JianShu），点击 "微信登录"，前端直接从 微信授权服务器（Authorization Server） 获取 access_token，然后用它请求用户信息。

## 用户点击 "使用微信登录"

你的浏览器（前端应用）跳转到 微信授权服务器，请求获取 access_token

```
GET https://open.weixin.qq.com/connect/oauth2/authorize?
    appid=wxe9199d568fe57fdd
    &redirect_uri=https%3A%2F%2Fspa.example.com%2Fcallback
    &response_type=token
    &scope=snsapi_userinfo
    &state=xyz


```

## 用户授权

你用微信扫码，点击 "确认授权"。
微信的 授权服务器（Authorization Server） 立即重定向回 redirect_uri，并在 URL 哈希片段（# 后面）直接返回 access_token
它 不会被浏览器发给服务器，但 前端 JavaScript 可以读取它。

```
https://spa.example.com/callback#
    access_token=ACCESS_TOKEN_VALUE
    &expires_in=7200
    &openid=USER_OPENID
    &scope=snsapi_userinfo
    &state=xyz

```

注意 access_token 是 URL 片段（Fragment）的一部分，
💡 不会发送给服务器，只能被前端 JavaScript 解析。

## 前端 JavaScript 解析 access_token

前端代码提取 access_token：

```
const hash = window.location.hash.substring(1);
const params = new URLSearchParams(hash);
const accessToken = params.get("access_token");
console.log("Access Token:", accessToken);

```

此时，前端已经拥有了 access_token，可以直接使用它访问微信 API。

## 前端用 access_token 请求用户信息

```
GET https://api.weixin.qq.com/sns/userinfo?
    access_token=ACCESS_TOKEN_VALUE
    &openid=USER_OPENID

```

服务器返回：

```
{
    "openid": "o6_bmjrPTlm6_2sgVt7hMZOPfL2M",
    "nickname": "微信用户",
    "sex": 1,
    "province": "Guangdong",
    "city": "Guangzhou",
    "headimgurl": "http://thirdwx.qlogo.cn/mmopen/...",
    "unionid": "o6_bmjrPTlm6_2sgVt7hMZOPfL2M"
}


```

前端拿到用户信息，展示在页面上，比如：

```
<img src="http://thirdwx.qlogo.cn/mmopen/..." />
<p>欢迎你，微信用户！</p>


```

## 简化模式的安全风险

🚨 OAuth 2.1 废弃简化模式的主要原因：

### Access Token 直接暴露在 URL

access_token 直接放在 URL 片段（# 号后面），如果恶意代码（XSS）或插件能访问 window.location.hash，就能窃取 Token。
用户如果 复制了 URL 并粘贴给别人，别人也能获取 access_token！
某些浏览器可能 在历史记录中存储 access_token

### Token 劫持

任何 恶意 JavaScript 都可以访问 access_token，比如：

```
console.log(window.location.hash); // 黑客代码

```

如果 access_token 被拦截，黑客可以伪装成你，访问你的账户数据！

### 无法刷新 Token

简化模式 不提供 refresh_token，access_token 过期后，用户必须 重新授权，增加了用户体验上的麻烦。

# 😨 这就意味着，后端完全不管理令牌？

是的，在简化模式下：

access_token 直接交给前端
后端没有参与授权流程
前端直接用令牌去获取用户数据
没有 refresh_token 机制，令牌过期后需要重新授权
