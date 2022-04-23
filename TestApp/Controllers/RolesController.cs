using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using TestApp.Services;

namespace TestApp.Controllers
{
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IDeactivateTokenService _deactivateTokenService;

        public RolesController(IRoleService roleService, IDeactivateTokenService deactivateToken)
        {
            _roleService = roleService;
            _deactivateTokenService = deactivateToken;
        }

        [HttpGet]
        [Route("roles")]
        [Authorize]
        public IActionResult GetAll()
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];
            if (_deactivateTokenService.IsTokenDeactivated(accessToken))
            {
                return StatusCode(401, new { message = "Token has been deactivated" });
            }
            return new JsonResult(_roleService.GetAllRoles());
        }

        [HttpGet]
        [Route("roles/{id:int}")]
        [Authorize]
        public IActionResult GetRole(int id)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];
            if (_deactivateTokenService.IsTokenDeactivated(accessToken))
            {
                return StatusCode(401, new { message = "Token has been deactivated" });
            }

            var role = _roleService.GetRole(id);
            if (role == null)
            {
                return StatusCode(400, new { message = "Role does not exist" });
            }
            return Ok(role);
        }
    }
}
