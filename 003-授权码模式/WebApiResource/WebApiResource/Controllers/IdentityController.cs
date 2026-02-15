using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebApiResource.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IdentityController : ControllerBase
    {
        [HttpGet]
        [Route("GetInit")]
        public IActionResult GetInit()
        {
            return new JsonResult(new string[]
            {
                "1",
                "2",
                "3",
                "4",
                "5"
            });
        }


        [HttpGet]
        [Authorize]
        [Route("GetUser")]
        public IActionResult GetUser()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }

        [HttpGet]
        [Route("GetUserNew")]
        [Authorize(policy: "prolicy")]
        public IActionResult GetUserNew()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }


        [HttpGet]
        [Route("GetResource")]
        [Authorize]  //如果标记Authorize，就需要鉴权
        public IActionResult GetResource()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }

    }
}
