using System.ComponentModel.DataAnnotations;
using System.Reflection;
using TestApp.Models;
using TestApp.Models.DTO;

namespace TestApp.Services
{
    public interface ISolutionService
    {
        SolutionGetDTO PostSolution(string sourceCode, string programmingLanguage, int taskId, int authorId);
        string? GetProgrammingLanguage(int id);
        List<SolutionGetDTO> GetAllSolutions();
        string? GetVerdict(int id);
        SolutionGetDTO? UpdateVerdict(int solutionId, string verdict);
    }

    public class SolutionService : ISolutionService
    {
        private readonly ApplicationContext _context;

        public SolutionService(ApplicationContext context)
        {
            _context = context;
        }

        public SolutionGetDTO PostSolution(string sourceCode, string programmingLanguage, int taskId, int authorId)
        {
            var solution = new Solution
            {
                SourceCode = sourceCode,
                ProgrammingLanguage = programmingLanguage,
                AuthorId = authorId,
                TaskId = taskId
            };
            _context.Solutions.Add(solution);
            _context.SaveChanges();
            return GetSolutionGetDTO(solution);
        }

        public string? GetProgrammingLanguage(int id)
        {
            var lang = (ProgrammingLanguage)Enum.ToObject(typeof(ProgrammingLanguage), id);
            if (int.TryParse(lang.ToString(), out _)) return null;

            var res = lang.GetType().GetMember(lang.ToString()).First().GetCustomAttribute<DisplayAttribute>();
            if (res == null) return null;

            return res.GetName();
        }

        public List<SolutionGetDTO> GetAllSolutions()
        {
            return _context.Solutions.Select(solution => new SolutionGetDTO
            {
                Id= solution.Id,
                SourceCode= solution.SourceCode,
                ProgrammingLanguage= solution.ProgrammingLanguage,
                Verdict = solution.Verdict,
                AuthorId= solution.AuthorId,
                TaskId= solution.TaskId
            }).ToList();
        }

        public string? GetVerdict(int id)
        {
            var verdict = (Verdict)Enum.ToObject(typeof(Verdict), id);
            if (int.TryParse(verdict.ToString(), out _)) return null;
            return verdict.ToString();
        }

        public SolutionGetDTO? UpdateVerdict(int solutionId, string verdict)
        {
            var solution = _context.Solutions.FirstOrDefault(solution => solution.Id == solutionId);
            if (solution == null) return null;
            solution.Verdict = verdict;
            _context.SaveChanges();
            return GetSolutionGetDTO(solution);
        }


        private SolutionGetDTO GetSolutionGetDTO(Solution solution)
        {
            return new SolutionGetDTO
            {
                Id = solution.Id,
                SourceCode = solution.SourceCode,
                ProgrammingLanguage = solution.ProgrammingLanguage,
                Verdict = solution.Verdict,
                AuthorId = solution.AuthorId,
                TaskId = solution.TaskId
            };
        }
    }
}
