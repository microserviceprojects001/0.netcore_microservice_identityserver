# 用简书登录过程举例

https://www.jianshu.com/sign_in

点击微博登录会跳到

https://api.weibo.com/oauth2/authorize?
client_id=1881139527&
redirect_uri=https%3A%2F%2Fwww.jianshu.com%2Fusers%2Fauth%2Fweibo%2Fcallback&
response_type=code&
state=%257B%257D###

# weibo 开放平台 授权机制

https://open.weibo.com/wiki/%E6%8E%88%E6%9D%83%E6%9C%BA%E5%88%B6

# 介绍 oauth2.0 四种模式

https://www.ruanyifeng.com/blog/2014/05/oauth_2_0.html

## 1, 授权码模式

![alt text](<../0.课程截图/第一部分 Identity Server/10，第一部分，授权码模式.png>)

OAuth 2.0 主要角色：
1， 资源所有者（Resource Owner）
也就是用户，拥有受保护的资源（如个人数据）。
2，客户端（Client）
需要访问资源的应用（如网站、App）。,
3，授权服务器（Authorization Server）
负责验证用户身份并颁发访问令牌（Access Token）。
4，资源服务器（Resource Server）
存储受保护资源，验证并响应客户端的请求。

比方说我想登录简书，然后会让我选择微信登录，然后回跳转到一个连接下
https://open.weixin.qq.com/connect/qrconnect?
appid=wxe9199d568fe57fdd&
client_id=wxe9199d568fe57fdd&
redirect_uri=https%3A%2F%2Fwww.jianshu.com%2Fusers%2Fauth%2Fwechat%2Fcallback&
response_type=code&
scope=snsapi_login&
state=%257B%257D#
wechat_redirect 让我扫码登录，请就这个简书登录过程帮我介绍一下你提到的这四个角色，分别谁是 资源所有者（Resource Owner），客户端（Client），授权服务器（Authorization Server），资源服务器（Resource Server）

### 1. 资源所有者（Resource Owner）

```
你（用户）

你是微信的用户，你的微信账号里存储了你的身份信息。
你拥有授权的权利，可以决定是否允许 简书 获取你的微信身份信息。
```

### 2. 客户端（Client）

```
简书（JianShu）

简书是需要访问你的微信账号信息的第三方应用。
它希望你用微信登录，并请求你的授权，以获取你的身份信息。
在授权请求中，简书的 client_id（即 appid=wxe9199d568fe57fdd）用来标识简书这个应用。
```

### 3. 授权服务器（Authorization Server）

```
微信开放平台（WeChat Open Platform）

授权服务器负责处理用户授权，并颁发 授权码（Authorization Code） 和 访问令牌（Access Token）。
当你点击“微信登录”时，简书将你重定向到 微信授权服务器（https://open.weixin.qq.com/connect/qrconnect），微信会生成一个二维码让你扫码。
当你扫码并确认授权后，微信授权服务器会回调 redirect_uri=https%3A%2F%2Fwww.jianshu.com%2Fusers%2Fauth%2Fwechat%2Fcallback，并携带授权码（code）。


```

### 4. 资源服务器（Resource Server）

```

微信 API 服务器（WeChat API Server）

资源服务器存储你的微信账户信息，比如 昵称、头像、UnionID 等。



```

```
简书用 code 向微信授权服务器（Authorization Server）申请 Access Token：

POST https://api.weixin.qq.com/sns/oauth2/access_token?
    appid=wxe9199d568fe57fdd
    &secret=APP_SECRET
    &code=AUTH_CODE
    &grant_type=authorization_code
```

```
微信 API 服务器返回 access_token，简书再用这个 access_token 请求微信的 用户信息接口：
GET https://api.weixin.qq.com/sns/userinfo?
    access_token=ACCESS_TOKEN
    &openid=OPENID

```

```
资源服务器返回用户的基本信息，比如
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

简书就能用这些数据创建你的简书账户或让你登录了。

## 总结：

OAuth 2.0 角色 在简书微信登录中的实际对应方
资源所有者（Resource Owner） 你（微信用户）
客户端（Client） 简书（JianShu）
授权服务器（Authorization Server） 微信开放平台（open.weixin.qq.com）
资源服务器（Resource Server） 微信 API 服务器（api.weixin.qq.com）

## 具体流程分析

### 1, 简书引导用户去微信授权服务器（Authorization Server）获取授权码（Authorization Code）

```
https://open.weixin.qq.com/connect/qrconnect?
    appid=wxe9199d568fe57fdd
    &redirect_uri=https%3A%2F%2Fwww.jianshu.com%2Fusers%2Fauth%2Fwechat%2Fcallback
    &response_type=code
    &scope=snsapi_login
    &state=xxxx

```

你扫码授权后，微信 授权服务器 生成 code 并回调到简书：

```
https://www.jianshu.com/users/auth/wechat/callback?code=AUTH_CODE&state=xxxx

```

### 2, 简书用 code 向微信授权服务器（Authorization Server）申请 Access Token

```
POST https://api.weixin.qq.com/sns/oauth2/access_token?
    appid=wxe9199d568fe57fdd
    &secret=APP_SECRET
    &code=AUTH_CODE
    &grant_type=authorization_code

```

个请求是发给 授权服务器（Authorization Server） 的，不是资源服务器。资源服务器只是验证 access_token 并返回用户信息。

### 3, 微信授权服务器返回 Access Token

```
{
    "access_token": "ACCESS_TOKEN_VALUE",
    "expires_in": 7200,
    "refresh_token": "REFRESH_TOKEN_VALUE",
    "openid": "USER_OPENID",
    "scope": "snsapi_login"
}

```

### 4, 简书用 Access Token 向微信资源服务器（Resource Server）获取用户信息

```
GET https://api.weixin.qq.com/sns/userinfo?
    access_token=ACCESS_TOKEN_VALUE
    &openid=USER_OPENID


```

这个请求才是发给 资源服务器（Resource Server），获取你的微信昵称、头像等信息。

## 理解分析

### code 是出现在 URL 的查询参数（?code=...），而不是 #fragment，所以前端 JavaScript 默认无法获取。 我需要理解下 fragment 是什么意思，JavaScript 获取不到吗，

什么是 fragment（#片段标识符）？
fragment 指的是 URL 中 # 开头的部分，它通常不会被浏览器发送到服务器，只会在前端可见。例如:

```
https://www.example.com/callback?code=12345#token=abcdef

```

查询参数（Query Parameters）： ?code=12345 ✅ 服务器可以获取
片段标识符（Fragment）： #token=abcdef ❌ 服务器不会收到，只在前端可见
JavaScript 可以用 window.location.hash 读取 fragment，例如：

```
console.log(window.location.hash);  // 输出: "#token=abcdef"

```

如果你想前端读取查询参数（?code=...），可以这样：

```
const queryParams = new URLSearchParams(window.location.search);
console.log(queryParams.get("code"));  // 输出: 12345


```

### 是的，前端技术上可以用 window.location.search 解析 ?code=...，但是 OAuth 2.0 设计的目的是让后端拿 code，而不是前端。 请问这是怎么控制的呢，就我认为，前端能拿到 code 呀，Oauth 2.0 设计目的是让后端拿到 code，那后端拿到 code 都做什么了呢

```
你问得很好！OAuth 2.0 是怎么控制让 code 只能由后端使用的？ 这个问题的核心在于：

前端 确实能拿到 code，但不能用 code 直接换取 access_token（因为 OAuth 2.0 规定了 code 必须通过后端交换 access_token）。
后端拿到 code 后，会做安全检查，并用它换取 access_token，前端无法绕过这个过程。
```

#### OAuth 2.0 如何保证 code 只能由后端使用？

1, 授权码（code）只能用一次

认证服务器（微信）生成的 code 只能被交换一次，一旦使用过，就无法再次使用。
这样，即使前端能看到 code，如果它没有办法直接去换 access_token，这个 code 也没有价值。

2, code 必须用 client_secret 交换 access_token（前端无法获取 client_secret）

服务器换取 access_token 需要：
code
client_id（应用 ID）
client_secret（应用密钥）

client_secret 只能存储在后端，前端不能访问，所以前端拿到 code 也无法自己换取 access_token。

换取 access_token 的请求示例（由后端发送）：

```
response = requests.post("https://api.weixin.qq.com/sns/oauth2/access_token", params={
    "appid": "wxe9199d568fe57fdd",
    "secret": "APP_SECRET",  # 只有后端知道
    "code": "12345",
    "grant_type": "authorization_code"
})
access_token_data = response.json()
access_token = access_token_data["access_token"]

```

3, 授权码只能在预定义的 redirect_uri 里使用

OAuth 2.0 服务器会检查 redirect_uri，只有合法的后端服务器地址才能使用 code。
例如，微信会强制要求 redirect_uri 事先注册，不能随便更换，防止 code 被劫持到别的地方。

### code 传给后端后，后端做了什么？

1, 后端解析 code

用户授权后，浏览器跳转到 https://www.jianshu.com/users/auth/wechat/callback?code=12345
后端接收 code，而不是前端
2, 后端用 code 交换 access_token
后端服务器向微信认证服务器（https://api.weixin.qq.com）发送请求

```
response = requests.post("https://api.weixin.qq.com/sns/oauth2/access_token", params={
    "appid": "wxe9199d568fe57fdd",
    "secret": "APP_SECRET",  # 只有后端知道
    "code": "12345",
    "grant_type": "authorization_code"
})
access_token_data = response.json()
access_token = access_token_data["access_token"]


```

3,后端存储 access_token 并创建用户会话
获取 access_token 后，后端可能：
存储 access_token，用于后续调用微信 API
创建用户会话（Session / JWT Token）
返回前端一个 session ID（而不是 access_token）
查询用户信息并写入数据库
4, 后端返回已登录的用户数据给前端
由于前端不能直接访问 access_token，后端会处理用户数据后返回：

```
{
    "user_id": 12345,
    "nickname": "微信用户",
    "avatar": "https://wx.qlogo.cn/...",
    "session_token": "xyz123"  // 仅供前端验证
}

```

前端不会直接接触 access_token，所有 API 请求都交给后端代理。

## 奥，那我大概理解了，https://www.jianshu.com/users/auth/wechat/callback 这个简书，微信会强制要求 redirect_uri 事先注册，就是说 微信事先是知道 简书的 的 url 的，先对这个 url 生成授权码，交给简书，然后简书 后台服务器需要 拿到这个授权码，和其他必要的信息再去向微信获取令牌

是的，你理解得很准确！👍

微信的 OAuth 2.0 认证过程确实有 预先注册 redirect_uri 的机制，保证只有信任的回调地址才能接收授权码（code）。整个流程如下：
1, 简书在微信开发者平台提前注册回调地址
例如，简书注册的 redirect_uri 是：

https://www.jianshu.com/users/auth/wechat/callback
这样，微信服务器就知道只会把授权码（code）发送到这个地址。
2, 用户访问简书，点击“使用微信登录”

浏览器会被重定向到微信的授权页面（微信 OAuth 服务器）：

https://open.weixin.qq.com/connect/oauth2/authorize?
appid=wxe9199d568fe57fdd
&redirect_uri=https%3A%2F%2Fwww.jianshu.com%2Fusers%2Fauth%2Fwechat%2Fcallback
&response_type=code
&scope=snsapi_userinfo
&state=xyz
这里的 redirect_uri 必须和微信开发者后台登记的地址一致，否则微信会拒绝请求！
用户同意授权后，微信就会生成授权码（code）。
3, 微信把授权码（code）返回给简书

授权成功后，微信会让浏览器跳转到简书，并在 URL 查询参数里附上 code：

https://www.jianshu.com/users/auth/wechat/callback?code=12345&state=xyz
这个 code 是一次性的，有效期很短（通常 10 分钟内）。
4, 简书后端拿 code 向微信服务器换取 access_token

简书后端会发送一个请求到微信 API：

POST https://api.weixin.qq.com/sns/oauth2/access_token
参数：
json

{
"appid": "wxe9199d568fe57fdd",
"secret": "APP_SECRET", // 只有简书后端知道
"code": "12345",
"grant_type": "authorization_code"
}
注意！只有后端能做这个请求，因为它需要 APP_SECRET！
5, 微信返回 access_token 和用户信息

微信返回的数据：

```
{
  "access_token": "ACCESS_TOKEN_VALUE",
  "expires_in": 7200,
  "refresh_token": "REFRESH_TOKEN_VALUE",
  "openid": "USER_OPENID",
  "scope": "snsapi_userinfo"
}

```

简书后端保存 access_token，用它来获取用户信息。
6, 后端用 access_token 获取微信用户信息
后端再调用：
nginx

GET https://api.weixin.qq.com/sns/userinfo?access_token=ACCESS_TOKEN_VALUE&openid=USER_OPENID
返回：
json

{
"openid": "USER_OPENID",
"nickname": "张三",
"sex": 1,
"province": "上海",
"city": "上海",
"country": "中国",
"headimgurl": "https://wx.qlogo.cn/mmopen/...",
"privilege": []
}
这时候，简书的后端就拿到了微信的用户信息，可以创建一个简书账号，并返回给前端。

## 获取授权码的流程

获取授权码（code）的操作是 前端通过浏览器重定向完成的，而 后端不会直接请求授权码。我们详细拆解一下这个过程：
获取授权码的流程
1, 前端让用户跳转到微信授权页面
用户在简书点击“微信登录”按钮时，前端代码会引导用户访问微信的授权 URL：

```
window.location.href = "https://open.weixin.qq.com/connect/oauth2/authorize?" +
  "appid=wxe9199d568fe57fdd" +
  "&redirect_uri=" + encodeURIComponent("https://www.jianshu.com/users/auth/wechat/callback") +
  "&response_type=code" +
  "&scope=snsapi_userinfo" +
  "&state=xyz";

```

这一步是浏览器重定向，并不是后端发起 HTTP 请求。

appid 由 微信官方 分配
appid 是 微信分配给简书的唯一标识，相当于简书在微信开发者平台上的 开发者 ID。
appid 的作用
唯一性：每个应用（如简书、知乎、B 站）都有 独立的 appid
微信授权的凭证：微信会检查 appid 是否 在开发者平台注册过
安全限制：必须和 预先注册的 redirect_uri 匹配，否则拒绝授权

appid + appsecret
appid（公开的）： 用于 OAuth 登录，任何人都能看到
appsecret（私密的）： 只有简书后台知道，用于换取 access_token
🚨 安全警告：appsecret 不能暴露，否则攻击者可以伪造简书，获取用户数据！

appsecret 是 微信生成 并提供给 简书开发者 的，它是 简书的私密凭证，用来在后端安全地交换 access_token。
appsecret 的生成方式
简书开发者 在 微信开放平台 或 微信公众平台 注册应用
微信审核通过后，会自动生成一个 appsecret
简书开发团队 只能在微信后台查看 appsecret，它不会暴露给外部

2, 微信服务器验证并让用户扫码授权
微信服务器会：
检查 appid 是否正确
检查 redirect_uri 是否事先注册
引导用户扫码并授权

3, 用户授权后，微信重定向到简书，并带上 code
授权完成后，微信会让 浏览器 跳转回 redirect_uri，并在 URL 查询参数里附带授权码

```
https://www.jianshu.com/users/auth/wechat/callback?code=12345&state=xyz

```

这个 code=12345 是一次性的 授权码。

4, 简书后端从 URL 解析 code
后端接管 /users/auth/wechat/callback 这个接口，解析 URL 中的 code：

```
public IActionResult WeChatCallback(string code, string state)
{
    if (string.IsNullOrEmpty(code))
    {
        return BadRequest("授权失败");
    }
    // 接下来后端去换取 access_token
}

```
