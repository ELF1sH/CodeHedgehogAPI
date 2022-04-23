using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestApp.Models;
using TestApp.Models.DTO;

namespace TestApp.Services
{
    public interface ITopicService
    {
        List<TopicGetDTO> GetAllTopics(int parentFilter, string nameFilter);
        TopicGetEnhancedDTO? GetTopic(int id);
        Task<TopicGetDTO> PostTopic(TopicPostDTO model);
        void DeleteCascadeTopic(int id);
        bool PatchTopic(int id, [FromBody] TopicPatchDTO model);
        List<TopicGetDTO> GetChilds(int id);
        bool PostChilds(int id, List<int> childIndices);
        bool DeleteChilds(int id, List<int> childIndices);
    }

    public class TopicService : ITopicService
    {
        private readonly ApplicationContext _context;

        public TopicService(ApplicationContext context)
        {
            _context = context;
        }

        public List<TopicGetDTO> GetAllTopics(int parentFilter, string nameFilter)
        {
            var topics = _context.Topics.Select(x => new TopicGetDTO
            {
                Id = x.Id,
                Name = x.Name,
                ParentId = x.ParentId
            }).ToList();
            if (parentFilter != 0)
            {
                topics = topics.Where(x => x.ParentId == parentFilter).ToList();
            }
            if (nameFilter != null)
            {
                topics = topics.Where(x => x.Name.Contains(nameFilter)).ToList();
            }
            return topics;
        }

        public TopicGetEnhancedDTO? GetTopic(int id)
        {
            var topic = _context.Topics.FirstOrDefault(x => x.Id == id);
            if (topic == null) return null;
            var childs = _context.Topics.Where(x => x.ParentId == id).Select(x => new TopicGetDTO
            {
                Id = x.Id,
                Name = x.Name,
                ParentId = x.ParentId
            }).ToList();
            var topicDTO = new TopicGetEnhancedDTO
            {
                Id = topic.Id,
                Name = topic.Name,
                ParentId = topic.ParentId,
                Childs = childs
            };
            return topicDTO;
        }

        public async Task<TopicGetDTO> PostTopic(TopicPostDTO model)
        {
            var topic = new Topic
            {
                Name = model.Name,
                ParentId = model.ParentId
            };
            await _context.Topics.AddAsync(topic);
            await _context.SaveChangesAsync();
            return new TopicGetDTO
            {
                Id = topic.Id,
                Name = topic.Name,
                ParentId = topic.ParentId
            };
        }

        public void DeleteCascadeTopic(int id)
        {
            var topicToDelete = _context.Topics.Where(x => x.Id == id).FirstOrDefault();
            if (topicToDelete != null)
            {
                if (topicToDelete.ParentId == id)
                {
                    _context.Topics.Remove(topicToDelete);
                    _context.SaveChanges();
                    return;
                }
                var children = _context.Topics.Where(x => x.ParentId == id).ToList();
                foreach (var child in children)
                {
                    DeleteCascadeTopic(child.Id);
                }
                _context.Topics.Remove(topicToDelete);
                _context.SaveChanges();
            }
        }

        public bool PatchTopic(int id, TopicPatchDTO model)
        {
            var topic = _context.Topics.FirstOrDefault(x => x.Id == id);
            if (topic == null) return false;
            if (model.Name != null)
                topic.Name = model.Name;
            if (model.ParentId != null)
                topic.ParentId = model.ParentId;
            _context.SaveChanges();
            return true;
        }

        public List<TopicGetDTO> GetChilds(int id)
        {
            return _context.Topics.Where(x => x.ParentId == id).Select(x => new TopicGetDTO
            {
                Id = x.Id,
                Name = x.Name,
                ParentId = x.ParentId
            }).ToList();
        }

        public bool PostChilds(int id, List<int> childIndices)
        {
            var topic = _context.Topics.FirstOrDefault(x => x.Id == id);
            if (topic == null) return false;
            foreach (var childIndex in childIndices)
            {
                var child = _context.Topics.FirstOrDefault(x => x.Id == childIndex);
                if (child != null)
                {
                    child.ParentId = id;
                }
                _context.SaveChanges();
            }
            return true;
        }

        public bool DeleteChilds(int id, List<int> childIndices)
        {
            var topic = _context.Topics.FirstOrDefault(x => x.Id == id);
            if (topic == null) return false;
            foreach (var childIndex in childIndices)
            {
                var child = _context.Topics.FirstOrDefault(x => x.Id == childIndex);
                if (child != null && child.ParentId == id)
                {
                    child.ParentId = null;
                }
                _context.SaveChanges();
            }
            return true;
        }
    }
}
