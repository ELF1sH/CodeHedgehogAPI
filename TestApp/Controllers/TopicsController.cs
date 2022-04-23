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
    public class TopicsController : ControllerBase
    {
        private readonly ITopicService _topicService;
        private readonly IDeactivateTokenService _deactivateTokenService;

        public TopicsController(ITopicService topicService, IDeactivateTokenService deactivateToken)
        {
            _topicService = topicService;
            _deactivateTokenService = deactivateToken;
        }


        [HttpGet]
        [Route("topics")]
        public IActionResult Get()
        {
            try
            {
                int parentFilter = Convert.ToInt32(Request.Query["parent"]);
                string nameFilter = Request.Query["name"];
                return new JsonResult(_topicService.GetAllTopics(parentFilter, nameFilter));
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
        [Route("topics/{id:int}")]
        public IActionResult GetEnhanced(int id)
        {
            try
            {
                var topic = _topicService.GetTopic(id);
                if (topic == null) return StatusCode(500, new { message = "topic does not exist" });
                return new JsonResult(topic);
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
        [Route("topics")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post([FromBody] TopicPostDTO model)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];
            if (_deactivateTokenService.IsTokenDeactivated(accessToken))
            {
                return StatusCode(401, new { message = "Token has been deactivated" });
            }

            try
            {
                var topic = await _topicService.PostTopic(model);
                return new JsonResult(topic);
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
        [Route("topics/{id:int}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];
            if (_deactivateTokenService.IsTokenDeactivated(accessToken))
            {
                return StatusCode(401, new { message = "Token has been deactivated" });
            }

            try
            {
                _topicService.DeleteCascadeTopic(id);
                return new JsonResult(new
                {
                    message = "OK"
                });
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
        [Route("topics/{id:int}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Patch(int id, [FromBody] TopicPatchDTO model)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];
            if (_deactivateTokenService.IsTokenDeactivated(accessToken))
            {
                return StatusCode(401, new { message = "Token has been deactivated" });
            }

            try
            {
                var res = _topicService.PatchTopic(id, model);
                if (res)
                {
                    return new JsonResult(_topicService.GetTopic(id));
                }
                else
                {
                    return StatusCode(500, new
                    {
                        message = "Topic does not exist"
                    });
                }
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
        [Route("topics/{id:int}/childs")]
        public IActionResult GetChilds(int id)
        {
            try
            {
                return new JsonResult(_topicService.GetChilds(id));
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
        [Route("topics/{id:int}/childs")]
        [Authorize(Roles = "Admin")]
        public IActionResult PostChilds(int id, [FromBody] List<int> childList)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];
            if (_deactivateTokenService.IsTokenDeactivated(accessToken))
            {
                return StatusCode(401, new { message = "Token has been deactivated" });
            }

            try
            {
                var res = _topicService.PostChilds(id, childList);
                if (res)
                {
                    return new JsonResult(_topicService.GetTopic(id));
                }
                else
                {
                    return StatusCode(500, new
                    {
                        message = "Topic does not exist"
                    });
                }
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
        [Route("topics/{id:int}/childs")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteChilds(int id, [FromBody] List<int> childList)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];
            if (_deactivateTokenService.IsTokenDeactivated(accessToken))
            {
                return StatusCode(401, new { message = "Token has been deactivated" });
            }

            try
            {
                var res = _topicService.DeleteChilds(id, childList);
                if (res)
                {
                    return new JsonResult(_topicService.GetTopic(id));
                }
                else
                {
                    return StatusCode(500, new
                    {
                        message = "Topic does not exist"
                    });
                }
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
    }
}
