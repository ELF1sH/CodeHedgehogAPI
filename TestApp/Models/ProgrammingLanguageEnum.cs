using System.ComponentModel.DataAnnotations;

namespace TestApp.Models
{
    public enum ProgrammingLanguage
    {
        [Display(Name = "Python")]
        Python = 1,

        [Display(Name = "C++")]
        CPP = 2,

        [Display(Name = "C#")]
        CSharp = 3,

        [Display(Name = "Java")]
        Java = 4
    }
}
