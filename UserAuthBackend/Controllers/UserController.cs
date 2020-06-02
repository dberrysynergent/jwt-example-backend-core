using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserAuthBackend.Interfaces;
using UserAuthBackend.Models;
using UserAuthBackend.Services;

namespace UserAuthBackend.Controllers
{
    [ApiController]
    public class UserController : Controller
    {
        private IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost]
        [Route("authenticate")]
        public IActionResult Authenticate([FromBody]User user)
        {
            var authenticatedUser = _userService.Authenticate(user.UserName, user.Password);

            if (authenticatedUser == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(authenticatedUser);
        }

        [HttpGet]
        [Route("action")]
        public IActionResult PriviledgedAction()
        {
            string token = this.Request.Headers["Authorization"];
            if(_userService.IsAuthenticated(token))
            {
                var resp = Ok("top secret info");
                return Ok("top secret info");
            }
            return Unauthorized("you are not allowed to see the top secret info");
        }
    }
}