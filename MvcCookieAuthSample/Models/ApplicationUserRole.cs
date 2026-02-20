// Copyright (c) Duende Software. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Identity;

namespace MvcCookieAuthSample.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUserRole : IdentityRole
    {
        public string role1 { get; set; } = string.Empty;
        public ApplicationUserRole() : base() { } // 需要无参数构造函数
        public ApplicationUserRole(string roleName) : base(roleName) { } // 需要有参数构造函数
    }
}
