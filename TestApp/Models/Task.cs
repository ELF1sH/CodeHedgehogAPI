using System.ComponentModel.DataAnnotations;

namespace TestApp.Models
{
    public class Task
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int TopicId { get; set; }
        public Topic Topic { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int Price { get; set; }

        [Required]
        public bool IsDraft { get; set; } = false;

        public string? Input { get; set; } = null;

        public string? Output { get; set; } = null;

        public List<Solution> Solutions { get; set; }
    }
}
