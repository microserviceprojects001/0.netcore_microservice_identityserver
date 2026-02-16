// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace Authorization.IdentityServer
{
    public static class Config
    {
        /// <summary>
        /// 获取用户信息的集合配置
        /// </summary>
        public static IEnumerable<IdentityResource> IdentityResources =>
          new IdentityResource[]
          {
                   new IdentityResources.OpenId(),//内置的信息
                   new IdentityResources.Profile(),//内置的信息
                   //new IdentityResources.Email(),//内置的信息
                   new IdentityResources.Email{
                        Enabled=true,//是否启用，默认为true
                        DisplayName="这里是修改过的DisplayName", //显示的名称，如在同意界面中将使用此值
                        Name="这里是修改过的-身份资源的唯一名称--Name",
                        Description="这里是修改过的Description", //显示的描述，如在同意界面中将使用此值。
                        Required=true,//指定用户是否可以在同意界面中取消选择范围（如果同意界面要实现这样的功能）。false 表示可以取消，true 则为必须。默认为 false。 
                   },//内置的信息
                   new IdentityResources.Phone(),//内置的信息
                   new IdentityResources.Address(),
                   new IdentityResource("roles", "角色信息", new List<string> {JwtClaimTypes.Role}) //自定义的信息
          };

        public static IEnumerable<ApiResource> ApiResources =>
          new ApiResource[]
          {
                new ApiResource()
               {
                    Name="Api1",
                    DisplayName="这里是测试Api",
                    Scopes=new  List<string>{
                        "Client-ApiScope"
                    }
               }
          };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope(){
                    Name="Client-ApiScope"
                }
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                  new Client
                    {
                        ClientId = "ClientPattern", 
                        // 无交互用户，请使用clientid/secret进行身份验证
                        AllowedGrantTypes = GrantTypes.ClientCredentials, 
                        // 身份验证的秘钥
                        ClientSecrets =
                        {
                            new Secret("ClientPatternSecret".Sha256())
                        }, 
                        // scopes that client has access to 
                        // 客户端有权访问的作用域--可以访问资源---必须在这里定义，才能访问--才能体现在token中
                        AllowedScopes = {
                            "Client-ApiScope" //必须是这里声明了Api的Scope,在获取Token的时候获取到  
                        }
                    },
                  new Client
                 {
                    ClientId = "PassPattern", 
                    // 无交互用户，请使用clientid/secret进行身份验证
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword, 
                    // 身份验证的秘密
                    ClientSecrets =
                    {
                        new Secret("PassPatternSecret".Sha256())
                    }, 
                    // scopes that client has access to 
                    // 客户端有权访问的作用域--可以访问资源---必须在这里定义，才能访问--才能体现在token中
                    AllowedScopes = {
                        "Client-ApiScope", //必须是这里声明了Api的Scope,在获取Token的时候获取到  
                        IdentityServerConstants.StandardScopes.OpenId,//必须在IdentityResources中声明过的，才能在这里使用
                        IdentityServerConstants.StandardScopes.Profile,//必须在IdentityResources中声明过的，才能在这里使用
                        IdentityServerConstants.StandardScopes.Email,//必须在IdentityResources中声明过的，才能在这里使用
                        IdentityServerConstants.StandardScopes.Address//必须在IdentityResources中声明过的，才能在这里使用
                    }
                },
                  new Client
                     {
                                    ClientId = "CodePattern",          //客户端Id
                                    ClientName = "MvcApplication",     //客户端名称
                                    AlwaysIncludeUserClaimsInIdToken = true,
                                    AllowedGrantTypes = GrantTypes.Code,   //认证模式--授权码模式

                                    RedirectUris ={
                                        "https://localhost:7001/signin-oidc", //跳转登录到的客户端的地址
                                    },
                                    // RedirectUris = {"http://localhost:7001/auth.html" }, //跳转登出到的客户端的地址
                                    PostLogoutRedirectUris ={
                                        "http://localhost:7001/signout-callback-oidc",
                                    },
                                    ClientSecrets = { new Secret("CodePatternSecret".Sha256()) },

                                    AllowedScopes = {
                                        IdentityServerConstants.StandardScopes.OpenId,
                                        IdentityServerConstants.StandardScopes.Profile,
                                        "Client-ApiScope"
                                    },
                                    //允许将token通过浏览器传递
                                    AllowAccessTokensViaBrowser=true,
                                    // 是否需要同意授权 （默认是false）
                                    RequireConsent=true
                      },
                  new Client
                     {
                                    ClientId = "CodeManualTest",          //客户端Id 这个Client是给  \实现流程解析\Client  这里代码使用的，手动去模拟授权码的过程
                                    ClientName = "MvcCodeManualTest",     //客户端名称

                                    AllowedGrantTypes = GrantTypes.Code,   //认证模式--授权码模式

                                    RedirectUris ={
                                        "https://localhost:7001/Code/IndexCodeView", //跳转登录到的客户端的地址
                                    },
                                    // RedirectUris = {"http://localhost:7001/auth.html" }, //跳转登出到的客户端的地址
                                    PostLogoutRedirectUris ={
                                        "http://localhost:7001/auth.html",
                                    },
                                    ClientSecrets = { new Secret("CodePatternSecret".Sha256()) },

                                    AllowedScopes = {
                                        IdentityServerConstants.StandardScopes.OpenId,
                                        IdentityServerConstants.StandardScopes.Profile,
                                        "Client-ApiScope"
                                    },
                                    //允许将token通过浏览器传递
                                    AllowAccessTokensViaBrowser=true,
                                    // 是否需要同意授权 （默认是false）
                                    RequireConsent=true
                      }
            };

        public static List<TestUser> Users =>
            new List<TestUser>
            {
                new TestUser{

                    SubjectId = "1",
                    //ProviderSubjectId="pwdClient",
                    Username = "Richard",
                    Password = "Richard",
                    Claims =
                    {
                        new Claim(JwtClaimTypes.Name, "Richard"),
                        new Claim(JwtClaimTypes.GivenName, "Richard"),
                        new Claim(JwtClaimTypes.FamilyName, "Richard-FamilyName"),
                        new Claim(JwtClaimTypes.Email, "Richard@email.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                        new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json)
                    }
                }
            };
    }
}