using System.ComponentModel.DataAnnotations;

namespace TestApp.Models
{
    public class DeactivatedToken
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Value { get; set; }
    }
}
