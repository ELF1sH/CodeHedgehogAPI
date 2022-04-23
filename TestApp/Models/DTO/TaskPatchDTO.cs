namespace TestApp.Models.DTO
{
    public class TaskPatchDTO
    {
        public string? Name { get; set; }

        public int? TopicId { get; set; }

        public string? Description { get; set; }

        public int? Price { get; set; }
    }
}
