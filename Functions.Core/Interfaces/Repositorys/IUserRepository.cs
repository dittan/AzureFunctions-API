using Functions.Core.Entities;
using System.Threading.Tasks;

namespace Functions.Core.Interfaces.Repositorys
{
    public interface IUserRepository
    {
        Task<Users> GetUserById(int id);
        Task<int> CreateUser(Users user);
        Task<bool> DeleteUser(int id);
        Task<bool> UpdateUser(Users user);
    }
}
