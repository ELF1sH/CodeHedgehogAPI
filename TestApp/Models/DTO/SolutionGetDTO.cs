namespace TestApp.Models.DTO
{
    public class SolutionGetDTO
    {
        public int Id { get; set; }

        public string SourceCode { get; set; }

        public string ProgrammingLanguage { get; set; }

        public string Verdict { get; set; }

        public int AuthorId { get; set; }

        public int TaskId { get; set; }
    }
}
