using System.Collections.Generic;
using System.Threading.Tasks;
using MarriageApp.API.Models;

namespace MarriageApp.API.Data
{
    public interface IMarriageRepository
    {
         void Add<T>(T entity) where T:class;
         void Delete<T>(T entity) where T:class;
         Task<bool> SaveAll();
         Task<IEnumerable<User>> GetUsers();
         Task<User> GetUser(int id);
         Task<Photo> GetPhoto(int id);
         Task<Photo> GetMainPhotoForUser(int userId);
         
    }
}