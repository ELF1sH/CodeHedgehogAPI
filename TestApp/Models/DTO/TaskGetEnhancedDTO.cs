namespace TestApp.Models.DTO
{
    public class TaskGetEnhancedDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }
        
        public int TopicId { get; set; }

        public string Description { get; set; }

        public int Price { get; set; }

        public bool IsDraft { get; set; }
    }
}
