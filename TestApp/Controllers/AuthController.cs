using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TestApp.Models.DTO;
using TestApp.Models;
using TestApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Net.Http.Headers;

namespace TestApp.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly IRegisterService _registerService;
        private readonly IDeactivateTokenService _deactivateTokenService;

        public AuthController(ILoginService loginService, IRegisterService register, IDeactivateTokenService deactivateToken)
        {
            _loginService = loginService;
            _registerService = register;
            _deactivateTokenService = deactivateToken;
        }


        [HttpPost]
        [Route("login")]
        public IActionResult Post([FromBody] UserLoginDTO model)
        {
            User? user;
            try
            {
                user = _loginService.LoginUser(model);
            }
            catch (Exception ex)
            {
                var response = new
                {
                    message = ex.InnerException == null ? ex.Message : ex.InnerException.Message
                };
                return StatusCode(500, response);
            }
            if (user != null)
            {
                return new JsonResult(_loginService.GetToken(user));
            }
            else
            {
                var response = new
                {
                    message = "Invalid login or password"
                };
                return StatusCode(403, response);
            }
        }
        

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Post([FromBody] UserRegistrationDTO model)
        {
            try
            {
                var token = await _registerService.RegistrateUser(model);
                return new JsonResult(token);
            }
            catch (Exception ex)
            {
                var response = new
                {
                    message = ex.InnerException == null ? ex.Message : ex.InnerException.Message
                };
                return StatusCode(500, response);
            }
        }


        [HttpPost]
        [Authorize]
        [Route("logout")]
        public IActionResult Post()
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];
            if (_deactivateTokenService.IsTokenDeactivated(accessToken))
            {
                return StatusCode(401, new { message = "Token has been deactivated" });
            }
            _deactivateTokenService.DeactivateToken(accessToken);
            return new JsonResult(new
            {
                message = "OK"
            });
        }
    }
}