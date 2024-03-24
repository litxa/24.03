using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ToDoList.Data;
using ToDoList.Models;

namespace ToDoList.Service
{
    public class UserService : IUserService
    {
        private ToDoListDbContext dbContext;

        public UserService(ToDoListDbContext db)
        {
            this.dbContext = db;
        }

        public void Create(string username, string bio)
        {
            
            User u = new User
            {
                Username = username,
                Bio = bio,
                Chores = new List<Chore>()
            };
            u.Username = username;
            u.Bio = bio;
            dbContext.Users.Add(u);
            dbContext.SaveChanges();
        }

        public void Delete(string username)
        {
            var user = dbContext.Users.FirstOrDefault(x => x.Username == username);
            if (user != null)
            {
                if (!dbContext.Chores.Any(x => x.UserId == user.Id))
                {
                    dbContext.Users.Remove(user);
                }
                else
                {
                    throw new Exception("You cannot delete user who has chores. Maybe go delete the chores first? ;0");
                }
            }
            else
            {
                throw new Exception($"User {username} doesn't exist.");
            }
            dbContext.SaveChanges();

        }

        public ICollection<User> GetAllUsers()
        {
            return dbContext.Users.ToList();
        }

        public void Update(string oldUsername, string newUsername, string bio)
        {
            var user = dbContext.Users.FirstOrDefault(x => x.Username == oldUsername);
            if (user != null)
            {
                user.Username = newUsername;
                user.Bio = bio;
            }
            else
            {
                throw new Exception($"User {oldUsername} was not found.");
            }
            dbContext.SaveChanges();
        }

        public void GetUser(string username)
        {
            var user = dbContext.Users.FirstOrDefault(x => x.Username == username);
            if (user != null)
            {
                var userFound = dbContext.Users.Where(x => x.Username == username);
            }
            else { throw new Exception("User doesn't exist!"); }
        }

        public bool UserExists(string username)
        {
            return dbContext.Users.Any(u => u.Username == username);
        }
    }
}
