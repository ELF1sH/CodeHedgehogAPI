using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestApp.Services;
using TestApp.Models.DTO;
using TestApp.Models;
using Microsoft.Net.Http.Headers;

namespace TestApp.Controllers
{
    [ApiController]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly IDeactivateTokenService _deactivateTokenService;

        public UsersController(IUserService userService, IDeactivateTokenService deactivateToken)
        {
            _userService = userService;
            _deactivateTokenService = deactivateToken;
        }

        [HttpGet]
        [Route("users")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAllUsers() 
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];
            if (_deactivateTokenService.IsTokenDeactivated(accessToken))
            {
                return StatusCode(401, new { message = "Token has been deactivated" });
            }

            return new JsonResult(_userService.GetAllUsers());
        }

        [HttpGet]
        [Route("users/{id:int}")]
        [Authorize]
        public IActionResult GetUser(int id)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];
            if (_deactivateTokenService.IsTokenDeactivated(accessToken))
            {
                return StatusCode(401, new { message = "Token has been deactivated" });
            }

            string usernameClaim = User.Claims.ToList()[0].ToString();
            string username = usernameClaim.Substring(usernameClaim.IndexOf(" ") + 1);

            string roleClaim = User.Claims.ToList()[1].ToString();
            string role = roleClaim.Substring(roleClaim.IndexOf(" ") + 1);

            if (role == "Admin" || _userService.GetIdByUsername(username) == id)
            {
                var user = _userService.GetUser(id);
                if (user == null)
                {
                    return StatusCode(400, new { message = "User with such id does not exist" });
                }
                else
                {
                    return new JsonResult(user);
                }
            }
            else
            {
                return StatusCode(403, new { message = "Only admin or data owner has access to this data" });
            }
        }

        [HttpDelete]
        [Route("users/{id:int}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteUser(int id)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];
            if (_deactivateTokenService.IsTokenDeactivated(accessToken))
            {
                return StatusCode(401, new { message = "Token has been deactivated" });
            }

            var res = _userService.DeleteUser(id);
            if (res)
            {
                return new JsonResult(new { message = "OK" });
            }
            return StatusCode(400, new { message = "User does not exist" });
        }

        [HttpPost]
        [Route("users/{id:int}/role")]
        [Authorize(Roles = "Admin")]
        public IActionResult ChangeRole([FromRoute]int id, [FromBody]UserChangeRoleDTO model)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];
            if (_deactivateTokenService.IsTokenDeactivated(accessToken))
            {
                return StatusCode(401, new { message = "Token has been deactivated" });
            }

            try
            {
                int roleId = model.RoleId;
                bool doesRoleExist = false;
                foreach (var role in Enum.GetValues(typeof(Role)))
                {
                    var key = (int)role;
                    if (key == roleId)
                    {
                        doesRoleExist = true;
                        break;
                    }
                }
                if (!doesRoleExist) return StatusCode(400, new { message = "Role does not exist" });
                _userService.ChangeRole(id, roleId);
                return StatusCode(200, new { message = "OK" });
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

        [HttpPatch]
        [Route("users/{id:int}")]
        [Authorize]
        public IActionResult PatchUser([FromRoute] int id, [FromBody] UserPatchDTO model)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];
            if (_deactivateTokenService.IsTokenDeactivated(accessToken))
            {
                return StatusCode(401, new { message = "Token has been deactivated" });
            }

            string usernameClaim = User.Claims.ToList()[0].ToString();
            string username = usernameClaim.Substring(usernameClaim.IndexOf(" ") + 1);

            string roleClaim = User.Claims.ToList()[1].ToString();
            string role = roleClaim.Substring(roleClaim.IndexOf(" ") + 1);

            if (role == "Admin" || _userService.GetIdByUsername(username) == id)
            {
                var res = _userService.PatchUser(id, model);
                if (res)
                {
                    return new JsonResult(_userService.GetUser(id));
                }
                return StatusCode(500, new { message = "User with this ID does not exist" });
            }
            else
            {
                return StatusCode(403, new { message = "Only admin or data owner has access to this data" });
            }
        }
    }
}
