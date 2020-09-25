using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UsuariosLogin.Data.Entity;
using UsuariosLogin.Infrastructure;
using UsuariosLogin.Models;

namespace UsuariosLogin.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [ActionName("me")]
        public async Task<IActionResult> UserData()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogError("Error: Retrieve user.");
                return BadRequest(new ErrorDetails()
                {
                    Message = Startup.StaticConfig["MessagesError:MissingFields"],
                    ErrorCode = Convert.ToInt32(Startup.StaticConfig["MessagesError:MissingFieldsErrorCode"])
                });
            }

            return Ok(Utils.MapperApplicationUserToUserDataResponse(user));
        }

        [HttpPost]
        [ActionName("signup")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterEntity model)
        {
            if (ModelState.IsValid)
            {
                var user = Utils.MapperApplicationUserToUserDataResponse(model);
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    var token = AuthenticationHelper.GenerateJwtToken(model.Email, user, _configuration);

                    var rootData = new RegisterResponse(token, user.UserName, user.Email);
                    return Created("user/register", rootData);
                }
                return BadRequest(result.Errors.ReturnErrorDetailsHandlingIdentityErrors());
            }
            return BadRequest(new ErrorDetails()
            {
                Message = Startup.StaticConfig["MessagesError:MissingFields"],
                ErrorCode = Convert.ToInt32(Startup.StaticConfig["MessagesError:MissingFieldsErrorCode"])
            });
        }

        [HttpPost]
        [ActionName("signin")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginEntity model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
                if (result.Succeeded)
                {
                    var appUser = _userManager.Users.SingleOrDefault(r => r.Email == model.Email);
                    await _userManager.UpdateLastLoginDateFromApplicationUserIfSucceeded(appUser);
                    var token = AuthenticationHelper.GenerateJwtToken(model.Email, appUser, _configuration);

                    var rootData = new LoginResponse(token, appUser.UserName, appUser.Email);
                    return Ok(rootData);
                }
                return StatusCode((int)HttpStatusCode.Unauthorized, new ErrorDetails()
                {
                    Message = Startup.StaticConfig["MessagesError:InvalidEmailPassword"],
                    ErrorCode = Convert.ToInt32(Startup.StaticConfig["MessagesError:InvalidEmailPasswordErrorCode"])
                });
            }
            return BadRequest(new ErrorDetails()
            {
                Message = Startup.StaticConfig["MessagesError:MissingFields"],
                ErrorCode = Convert.ToInt32(Startup.StaticConfig["MessagesError:MissingFieldsErrorCode"])
            });
        }

    }
}
