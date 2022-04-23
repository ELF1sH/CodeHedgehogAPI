using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using TestApp.Models;
using TestApp.Models.DTO;
using TestApp.Services;

namespace TestApp.Controllers
{
    [ApiController]
    public class SolutionsController : ControllerBase
    {
        private readonly ISolutionService _solutionService;
        private readonly IUserService _userService;
        private readonly IDeactivateTokenService _deactivateTokenService;

        public SolutionsController(ISolutionService solution, IUserService user, IDeactivateTokenService deactivateToken)
        {
            _solutionService = solution;
            _userService = user;
            _deactivateTokenService = deactivateToken;
        }

        [HttpPost]
        [Route("tasks/{taskId:int}/solution")]
        [Authorize]
        public IActionResult PostSolution([FromRoute]int taskId, [FromBody]SolutionPostDTO model)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];
            if (_deactivateTokenService.IsTokenDeactivated(accessToken))
            {
                return StatusCode(401, new { message = "Token has been deactivated" });
            }

            string usernameClaim = User.Claims.ToList()[0].ToString();
            string username = usernameClaim.Substring(usernameClaim.IndexOf(" ") + 1);

            var programmingLanguage = _solutionService.GetProgrammingLanguage(model.ProgrammingLanguage);

            if (programmingLanguage == null)
            {
                return StatusCode(500, new { message = "Wrong programming language code" });
            }
            var sourceCode = model.SourceCode;

            var userId = _userService.GetIdByUsername(username);
            if (userId == null) return StatusCode(500, new { message = "User does not exist" });

            try
            {
                var solution = _solutionService.PostSolution(sourceCode, programmingLanguage, taskId, (int)userId);
                return Ok(solution);
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

        [HttpGet]
        [Route("solutions")]
        public IActionResult GetAllSolutions()
        {
            return Ok(_solutionService.GetAllSolutions());
        }

        [HttpPost]
        [Route("solutions/{solutionId:int}/postmoderation")]
        [Authorize(Roles = "Admin")]
        public IActionResult Postmoderate([FromRoute]int solutionId, [FromBody]SolutionVerdictDTO model)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];
            if (_deactivateTokenService.IsTokenDeactivated(accessToken))
            {
                return StatusCode(401, new { message = "Token has been deactivated" });
            }

            string? verdict = _solutionService.GetVerdict(model.Verdict);
            if (verdict == null)
            {
                return StatusCode(500, new { message = "Invalid verdict code" });
            }
            var solution = _solutionService.UpdateVerdict(solutionId, verdict);
            if (solution == null)
            {
                return StatusCode(500, new { message = "Solution does not exist" });
            }
            return Ok(solution);
        }
    }
}
