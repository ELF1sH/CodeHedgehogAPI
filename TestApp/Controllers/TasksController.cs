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
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly IDeactivateTokenService _deactivateTokenService;

        public TasksController(ITaskService taskService, IDeactivateTokenService deactivateToken)
        {
            _taskService = taskService;
            _deactivateTokenService = deactivateToken;
        }

        [HttpGet]
        [Route("tasks")]
        public IActionResult GetAllTasks()
        {
            try
            {
                int topicFilter = Convert.ToInt32(Request.Query["topic"]);
                string nameFilter = Request.Query["name"];
                return new JsonResult(_taskService.GetAllTasks(nameFilter, topicFilter));
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
        [Route("tasks/{taskId:int}")]
        public IActionResult GetTask([FromRoute] int taskId)
        {
            var task = _taskService.GetTask(taskId);
            if (task == null)
            {
                return StatusCode(500, new { message = "Task does not exist" });
            }
            return Ok(task);
        }

        [HttpPost]
        [Route("tasks")]
        [Authorize(Roles = "Admin")]
        public IActionResult PostTask(TaskPostDTO model)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];
            if (_deactivateTokenService.IsTokenDeactivated(accessToken))
            {
                return StatusCode(401, new { message = "Token has been deactivated" });
            }

            try
            {
                var task = _taskService.PostTask(model);
                return new JsonResult(task);
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
        [Route("tasks/{id:int}")]
        [Authorize(Roles = "Admin")]
        public IActionResult PatchTask([FromRoute]int id, [FromBody]TaskPatchDTO model)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];
            if (_deactivateTokenService.IsTokenDeactivated(accessToken))
            {
                return StatusCode(401, new { message = "Token has been deactivated" });
            }

            try
            {
                var task = _taskService.PatchTask(id, model);
                if (task == null) return StatusCode(500, new { message = $"Task with id {id} does not exist" });
                return Ok(task);
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

        [HttpDelete]
        [Route("tasks/{id:int}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteTask([FromRoute]int id)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];
            if (_deactivateTokenService.IsTokenDeactivated(accessToken))
            {
                return StatusCode(401, new { message = "Token has been deactivated" });
            }

            try
            {
                var res = _taskService.DeleteTask(id);
                if (!res) return StatusCode(400, new { message = "Task does not exist" });
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

        [HttpGet]
        [Route("tasks/{id:int}/input")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetInput([FromRoute] int id)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];
            if (_deactivateTokenService.IsTokenDeactivated(accessToken))
            {
                return StatusCode(401, new { message = "Token has been deactivated" });
            }

            var path = _taskService.GetTaskFilePath(id, true);
            if (path == null) return StatusCode(400, new { message = "Task or file don't exist" });
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            return File(bytes, "application/octet-stream", $"input{id}.txt");
        }

        [HttpPost]
        [Route("tasks/{id:int}/input")]
        [Authorize(Roles = "Admin")]
        public IActionResult PostInput([FromRoute] int id, [FromForm] IFormFile file)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];
            if (_deactivateTokenService.IsTokenDeactivated(accessToken))
            {
                return StatusCode(401, new { message = "Token has been deactivated" });
            }

            if (Path.GetExtension(file.FileName) != ".txt")
            {
                return StatusCode(400, new { message = "Wrong file extension. Consider uploading txt file" });
            }
            var task = _taskService.PostTaskFile(id, file, true).Result;
            if (task == null)
            {
                return StatusCode(400, new { message = "Task does not exist" });
            }
            return new JsonResult(task);
        }

        [HttpDelete]
        [Route("tasks/{id:int}/input")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteInput([FromRoute] int id)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];
            if (_deactivateTokenService.IsTokenDeactivated(accessToken))
            {
                return StatusCode(401, new { message = "Token has been deactivated" });
            }

            var res = _taskService.DeleteTaskFile(id, true);
            if (res == null) return StatusCode(500, new { message = "Task does not exist" });
            if (res == false) return StatusCode(500, new { message = "Task has no Input file" });
            return StatusCode(200, new { message = "OK" });
        }

        [HttpGet]
        [Route("tasks/{id:int}/output")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetOutput([FromRoute] int id)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];
            if (_deactivateTokenService.IsTokenDeactivated(accessToken))
            {
                return StatusCode(401, new { message = "Token has been deactivated" });
            }

            var path = _taskService.GetTaskFilePath(id, false);
            if (path == null) return StatusCode(400, new { message = "Task or file don't exist" });
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            return File(bytes, "application/octet-stream", $"output{id}.txt");
        }

        [HttpPost]
        [Route("tasks/{id:int}/output")]
        [Authorize(Roles = "Admin")]
        public IActionResult PostOutput([FromRoute] int id, [FromForm] IFormFile file)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];
            if (_deactivateTokenService.IsTokenDeactivated(accessToken))
            {
                return StatusCode(401, new { message = "Token has been deactivated" });
            }

            if (Path.GetExtension(file.FileName) != ".txt")
            {
                return StatusCode(400, new { message = "Wrong file extension. Consider uploading txt file" });
            }
            var task = _taskService.PostTaskFile(id, file, false).Result;
            if (task == null)
            {
                return StatusCode(400, new { message = "Task does not exist" });
            }
            return new JsonResult(task);
        }

        [HttpDelete]
        [Route("tasks/{id:int}/output")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteOutput([FromRoute] int id)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];
            if (_deactivateTokenService.IsTokenDeactivated(accessToken))
            {
                return StatusCode(401, new { message = "Token has been deactivated" });
            }

            var res = _taskService.DeleteTaskFile(id, false);
            if (res == null) return StatusCode(500, new { message = "Task does not exist" });
            if (res == false) return StatusCode(500, new { message = "Task has no Output file" });
            return StatusCode(200, new { message = "OK" });
        }
    }
}
