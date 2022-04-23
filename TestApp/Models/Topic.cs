using System.ComponentModel.DataAnnotations;

namespace TestApp.Models
{
    public class Topic
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int? ParentId { get; set; }
        public Topic? Parent { get; set; }

        public List<Topic>? Childs { get; set; }

        public List<Task>? Tasks { get; set; }
    }
}
