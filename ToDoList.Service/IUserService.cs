using ToDoList.Models;

namespace ToDoList.Service
{
    public interface IUserService
    {
        void Create(string username, string bio);

        void Update(string oldUsername, string newUsername, string bio);

        void Delete(string username);

        ICollection<User> GetAllUsers();
    }
}
