using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MvcClient
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddSession(options =>
                {
                    options.IdleTimeout = TimeSpan.FromMinutes(20); // 会话过期时间
                    //options.Cookie.HttpOnly = true;                 // 防止客户端脚本访问
                    options.Cookie.IsEssential = true;              // 即使未同意 Cookie 策略也发送
                });
            #region MyRegion 
            //JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();//Jwtӳ��ر� 
            //services.AddAuthentication(option =>
            //{
            //    option.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    option.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;

            //})
            //.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
            //.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            //{
            //    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    options.Authority = "https://localhost:5001";//��Ȩ�������ĵ�ַ
            //    options.RequireHttpsMetadata = true;//ʹ��Https  ����ʹ�ã��������Https�ᱨ��
            //    options.ClientId = "CodePattern";
            //    options.ClientSecret = "CodePatternSecret";
            //    options.ResponseType = "code";
            //    options.Scope.Clear();
            //    options.Scope.Add("Client-ApiScope");
            //    options.Scope.Add(OidcConstants.StandardScopes.OpenId);
            //    options.Scope.Add(OidcConstants.StandardScopes.Profile);
            //    //options.Scope.Add(OidcConstants.StandardScopes.Email);
            //    //options.Scope.Add(OidcConstants.StandardScopes.Phone);
            //    //options.Scope.Add(OidcConstants.StandardScopes.Address);
            //    //options.Scope.Add(OidcConstants.StandardScopes.OfflineAccess); //��ȡ��ˢ��Token
            //    options.SaveTokens = true;//��ʾTokenҪ�洢
            //    options.GetClaimsFromUserInfoEndpoint = true;

            //    options.Events = new OpenIdConnectEvents()
            //    {
            //        OnRemoteFailure = context =>
            //        {

            //            context.Response.Redirect(location: "/");
            //            context.HandleResponse();
            //            return Task.FromResult(0);
            //        }
            //    };
            //});
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();
            //app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Code}/{action=CodeTokenView}/{id?}");
            });
        }
    }
}
