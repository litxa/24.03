using ToDoList.Models;

namespace ToDoList.Service
{
    public interface IChoreService
    {
        void Create(string username, Chore c);

        void Update(string oldTitle, string newTitle, string Description, bool isImportant, bool isFinished);

        string Delete(string title);

        ICollection<Chore> GetAllChoresByUser(string username);
    }
}
