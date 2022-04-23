using Microsoft.AspNetCore.Mvc;
using TestApp.Models;
using TestApp.Models.DTO;

namespace TestApp.Services
{
    public interface IUserService
    {
        List<UserGetDTO> GetAllUsers();
        UserGetEnhancedDTO GetUser(int id);
        int? GetIdByUsername(string username);
        bool DeleteUser(int id);
        void ChangeRole(int userId, int roleId);
        bool PatchUser(int id, UserPatchDTO model);
    }

    public class UserService : IUserService
    {
        private readonly ApplicationContext _context;

        public UserService(ApplicationContext context)
        {
            _context = context;
        }

        public List<UserGetDTO> GetAllUsers()
        {
            return _context.Users.Select(x => new UserGetDTO
            {
                UserId = x.Id,
                Username = x.Username,
                RoleId = x.Role
            }).ToList();
        }

        public UserGetEnhancedDTO GetUser(int id)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == id);
            if (user == null) return null;
            return new UserGetEnhancedDTO
            {
                UserId = user.Id,
                Username = user.Username,
                RoleId = user.Role,
                Name = user.Name,
                Surname = user.Surname
            };
        }

        public int? GetIdByUsername(string username)
        {
            var user = _context.Users.FirstOrDefault(x => x.Username == username);
            if (user == null) return null;
            return user.Id;
        }

        public bool DeleteUser(int id)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == id);
            if (user == null) return false;
            _context.Users.Remove(user);
            _context.SaveChanges();
            return true;
        }

        public void ChangeRole(int userId, int roleId)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == userId);
            if (user != null)
            {
                user.Role = roleId;
                _context.SaveChanges();
            }
        }

        public bool PatchUser(int id, UserPatchDTO model)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == id);
            if (user != null)
            {
                if (model.Password != null) user.Password = model.Password;
                if (model.Name != null) user.Name = model.Name;
                if (model.Surname != null) user.Surname = model.Surname;
                _context.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
