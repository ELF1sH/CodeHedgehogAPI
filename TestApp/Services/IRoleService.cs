using TestApp.Models;
using TestApp.Models.DTO;

namespace TestApp.Services
{
    public interface IRoleService
    {
        List<RoleDTO> GetAllRoles();
        RoleDTO? GetRole(int id);
    }

    public class RoleService : IRoleService
    {
        public List<RoleDTO> GetAllRoles()
        {
            var rolesList = new List<RoleDTO>();
            foreach (var role in Enum.GetValues(typeof(Role)))
            {
                rolesList.Add(new RoleDTO
                {
                    Id = (int)role,
                    Name = role.ToString()
                });
            }
            return rolesList;
        }

        public RoleDTO? GetRole(int id)
        {
            var name = Enum.GetName(typeof(Role), id);
            if (name == null)
            {
                return null;
            }
            return new RoleDTO
            {
                Id = id,
                Name = name
            };
        }
    }
}
