using System.ComponentModel.DataAnnotations;

namespace TestApp.Models
{
    public class Solution
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string SourceCode { get; set; }

        [Required]
        public string ProgrammingLanguage { get; set; }

        [Required]
        public string Verdict { get; set; } = Models.Verdict.Pending.ToString();

        [Required]
        public int AuthorId { get; set; }
        public User User { get; set; }

        [Required]
        public int TaskId { get; set; }
        public Task Task { get; set; }
    }
}
