namespace TestApp.Models.DTO
{
    public class TopicGetEnhancedDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int? ParentId { get; set; }

        public List<TopicGetDTO> Childs { get; set; } = new List<TopicGetDTO>();
    }
}

