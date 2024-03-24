using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using ToDoList.Data;
using ToDoList.Models;

namespace ToDoList.Service
{
    public class ChoreService : IChoreService
    {
        private ToDoListDbContext dbContext;

        public ChoreService(ToDoListDbContext db)
        {
            this.dbContext = db;
        }

        public void Create(string username, Chore c)
        {
            var user = this.FindUserByUsername(username);
            if (user != null)
            {
                c.UserId = user.Id;
                if (string.IsNullOrWhiteSpace(c.Description))
                {
                    c.Description = null;
                }
                dbContext.Chores.Add(c);
                dbContext.SaveChanges();
            }
            else
            {
                throw new Exception($"Cannot create chore! User {username} does not exist.");
            }
        }

        public string Delete(string title)
        {
            var chore = this.FindChoreByTitle(title);
            if (chore.IsFinished== false)
            {
                return $"Chore '{chore.Title}' is not finished and can't be deleted!";
            }
            else if (chore != null)
            {
                dbContext.Chores.Remove(chore);
            }
            else
            {
                return $"Chore '{title}' doesn't exist.";
            }
            dbContext.SaveChanges();
            return $"Chore '{title}' was deleted successfully!";
        }

        public ICollection<Chore> GetAllChoresByUser(string username)
        {
            User user = this.FindUserByUsername(username);
            return dbContext.Chores.Where(x => x.UserId == user.Id).ToList();
        }

        public void Update(string oldTitle, string newTitle, string Description, bool isImportant, bool isFinished)
        {
            var chore = this.FindChoreByTitle(oldTitle);
            if (chore != null)
            {
                chore.Title = newTitle.Trim();
                chore.Description = Description;
                chore.IsImportant = isImportant;
                chore.IsFinished = isFinished;
            }
            else
            {
                throw new Exception($"Chore '{oldTitle}' was not found.");
            }
            dbContext.SaveChanges();
        }

        public User FindUserByUsername(string username)
        {
            return dbContext.Users.FirstOrDefault(x => x.Username == username);
        }

        public Chore FindChoreByTitle(string title)
        {
            return dbContext.Chores.FirstOrDefault(x => x.Title == title);
        }
    }
}
