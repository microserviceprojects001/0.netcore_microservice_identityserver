using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;


namespace Authorization.IdentityServer.Models
{
    public class ApplicationUser : IdentityUser
    {
        // 可以添加自定义属性，例如：
        // public string Avatar { get; set; }
    }
}