using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace MultiAuthSchemas.Controllers
{
    //Only AAD token can access this endpoint.
    [Authorize(Policy = "AADAuth")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthAADController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "AAD Authentication" };
        }
    }

    //Both AAD/AADB2C token can access this endpoint.
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
           return new string[] { "AAD/B2C Authentication" };
        }
    }
}