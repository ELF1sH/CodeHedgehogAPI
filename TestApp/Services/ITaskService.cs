using Microsoft.AspNetCore.Mvc;
using TestApp.Models;
using TestApp.Models.DTO;

namespace TestApp.Services
{
    public interface ITaskService
    {
        TaskGetEnhancedDTO? PostTask(TaskPostDTO model);
        List<TaskGetDTO> GetAllTasks(string nameFilter, int topicFilter);
        TaskGetEnhancedDTO? GetTask(int id);
        TaskGetEnhancedDTO? PatchTask(int id, TaskPatchDTO model);
        bool DeleteTask(int id);
        Task<TaskGetEnhancedDTO?> PostTaskFile(int id, IFormFile file, bool isInput = true);
        bool? DeleteTaskFile(int id, bool isInput = true);
        string? GetTaskFilePath(int id, bool isInput = true);
    }

    public class TaskService : ITaskService
    {
        private readonly ApplicationContext _context;
        private readonly IWebHostEnvironment _appEnvironment;

        public TaskService(ApplicationContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        public List<TaskGetDTO> GetAllTasks(string nameFilter, int topicFilter)
        {
            var tasks = _context.Tasks.Select(x => new TaskGetDTO { Id = x.Id, Name = x.Name, TopicId = x.TopicId }).ToList();
            if (nameFilter != null)
            {
                tasks = tasks.Where(x => x.Name.Contains(nameFilter)).ToList();
            }
            if (topicFilter != 0)
            {
                tasks = tasks.Where(x => x.TopicId == topicFilter).ToList();
            }
            return tasks;
        }

        public TaskGetEnhancedDTO? GetTask(int id)
        {
            var task = _context.Tasks.FirstOrDefault(x => x.Id == id);
            if (task == null) return null;
            return GenerateTaskGetEnhancedDTO(task);
        }

        public TaskGetEnhancedDTO? PostTask(TaskPostDTO model)
        {
            var task = new Models.Task
            {
                Name = model.Name,
                TopicId = model.TopicId,
                Description = model.Description,
                Price = model.Price,
            };
            _context.Tasks.Add(task);
            _context.SaveChanges();
            return GenerateTaskGetEnhancedDTO(task);
        }

        public TaskGetEnhancedDTO? PatchTask(int id, TaskPatchDTO model)
        {
            var task = _context.Tasks.FirstOrDefault(x => x.Id == id);
            if (task == null) return null;
            if (model.Name != null) task.Name = model.Name;
            if (model.TopicId != null) task.TopicId = (int)model.TopicId;
            if (model.Description != null) task.Description = model.Description;
            if (model.Price != null) task.Price = (int)model.Price;
            _context.SaveChanges();
            return GenerateTaskGetEnhancedDTO(task);
        }

        public bool DeleteTask(int id)
        {
            var task = _context.Tasks.FirstOrDefault(x => x.Id == id);
            if (task == null) return false;
            _context.Tasks.Remove(task);
            _context.SaveChanges();
            return true;
        }

        public async Task<TaskGetEnhancedDTO?> PostTaskFile(int id, IFormFile file, bool isInput = true)
        {
            var task = _context.Tasks.FirstOrDefault(x => x.Id == id);
            if (task == null) return null;

            string path = GeneratePath(id, isInput);
            using var fileStream = new FileStream(_appEnvironment.ContentRootPath + path, FileMode.Create);
            await file.CopyToAsync(fileStream);

            if (isInput) task.Input = path;
            else task.Output = path;
            _context.SaveChanges();
            return GenerateTaskGetEnhancedDTO(task);
        }

        public bool? DeleteTaskFile(int id, bool isInput = true)
        {
            var task = _context.Tasks.FirstOrDefault(x => x.Id == id);
            if (task == null) return null;

            var path = isInput ? task.Input : task.Output;
            if (path == null) return false;

            File.Delete(_appEnvironment.ContentRootPath + path);
            if (isInput) task.Input = null;
            else task.Output = null;
            _context.SaveChanges();
            return true;
        }

        public string? GetTaskFilePath(int id, bool isInput = true)
        {
            var task = _context.Tasks.FirstOrDefault(x => x.Id == id);
            if (task == null) return null;

            var path = isInput ? task.Input : task.Output;
            if (path == null) return null;

            path = _appEnvironment.ContentRootPath + path;
            return path;
        }

        // functions-helpers
        private TaskGetEnhancedDTO GenerateTaskGetEnhancedDTO(Models.Task task)
        {
            return new TaskGetEnhancedDTO
            {
                Id = task.Id,
                Name = task.Name,
                TopicId = task.TopicId,
                Description = task.Description,
                Price = task.Price,
                IsDraft = task.IsDraft,
            };
        }
        
        private static string GeneratePath(int id, bool isInput = true)
        {
            string timeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
            return $"Files\\{timeStamp}_{new Random().Next(0, 100000)}_{(isInput ? "input" : "output")}_{id}.txt";
        }
    }
}
