using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Authorization.IdentityServer.Models
{
    public class ApplicationUserRole : IdentityRole
    {
        public ApplicationUserRole() : base() { }
        public ApplicationUserRole(string roleName) : base(roleName) { }
    }
}