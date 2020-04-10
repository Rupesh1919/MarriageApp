using System.Threading.Tasks;
using MarriageApp.API.Models;

namespace MarriageApp.API.Data
{
    public interface IAuthRepository
    {
         Task<User> Register(User user, string password);
         Task<User> Login(string username,string password);
         Task<bool> UserExits(string username);
    }
}