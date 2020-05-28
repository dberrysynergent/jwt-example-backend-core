using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserAuthBackend.Models;

namespace UserAuthBackend.Controllers
{
    [ApiController]
    public class UserController : Controller
    {
        private List<User> Users = new List<User>
        {
            new User { UserName = "test", Password = "testing", Token = null },
            new User { UserName = "test2", Password = "testing2", Token = null }
        };
        [HttpPost]
        [Route("authenticate")]
        public IActionResult Authenticate([FromBody]User user)
        {
            var authenticatedUser = Users.SingleOrDefault<User>(x => x.UserName == user.UserName && x.Password == user.Password);

            if (authenticatedUser == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(authenticatedUser);
        }
    }
}