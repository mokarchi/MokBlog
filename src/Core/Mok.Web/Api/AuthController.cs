using Mok.Membership;
using Mok.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Mok.Web.Api
{
    /// <summary>
    /// Api for authentication.
    /// </summary>
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userSvc;
        private readonly SignInManager<User> _signInManager;

        public AuthController(IUserService userService,
            SignInManager<User> signInManager)
        {
            _userSvc = userService;
            _signInManager = signInManager;
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromBody] LoginVM loginUser)
        {
            // get user
            var user = await _userSvc.FindByEmailOrUsernameAsync(loginUser.UserName);
            if (user == null)
                return BadRequest("Invalid credentials!");

            // sign user in
            var result = await _signInManager.PasswordSignInAsync(user, loginUser.Password,
                loginUser.RememberMe, lockoutOnFailure: false);

            if (!result.Succeeded)
                return BadRequest("Invalid credentials!");

            return Ok();
        }
    }
}
