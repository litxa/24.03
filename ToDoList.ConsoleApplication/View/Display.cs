using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.Text.RegularExpressions;
using ToDoList.Data;
using ToDoList.Models;
using ToDoList.Service;

namespace ToDoList.ConsoleApplication.View
{
    public class Display
    {
        private int closeOperationId = 9;
        private readonly ChoreService choreService;
        private readonly UserService userService;
        private readonly ToDoListDbContext db;

        public Display()
        {
            db = new ToDoListDbContext();
            db.Database.EnsureCreated();
            choreService = new ChoreService(db);
            userService = new UserService(db);
            Input();
        }

        private void ShowMenu()
        {
            Console.WriteLine(' ');
            Console.WriteLine(new string('─', 32));
            Console.WriteLine('▒' + new string('░', 12) + " MENU " + new string('░', 12) + '▒');
            Console.WriteLine(new string('─', 32));
            Console.WriteLine("▒░─ 1. List all users");
            Console.WriteLine("▒░─ 2. Add new user");
            Console.WriteLine("▒░─ 3. Edit user");
            Console.WriteLine("▒░─ 4. Delete user");
            Console.WriteLine("▒░─ 5. Add new chore");
            Console.WriteLine("▒░─ 6. Edit chore");
            Console.WriteLine("▒░─ 7. Delete chore");
            Console.WriteLine("▒░─ 8. List all chores by user");
            Console.WriteLine("▒░─ 9. Exit");
            Console.WriteLine(new string('─', 32));
            Console.Write("Enter your command: ");
        }

        private void Input()
        {
            var operation = -1;
            do
            {
                ShowMenu();
                operation = int.Parse(Console.ReadLine());
                switch (operation)
                {
                    case 1:
                        ListAllUsers();
                        break;
                    case 2:
                        AddUser();
                        break;
                    case 3:
                        UpdateUser();
                        break;
                    case 4:
                        DeleteUser();
                        break;
                    case 5:
                        CreateChore();
                        break;
                    case 6:
                        UpdateChore();
                        break;
                    case 7:
                        DeleteChore();
                        break;
                    case 8:
                        ListAllChoresByUser();
                        break;
                    default:
                        break;
                }
            } while (operation != closeOperationId);
        }

        private void DeleteChore()
        {
            Console.Write("Enter username to search in their chores: ");
            var username = Console.ReadLine();
            IsValid(username);

            User searchUser = choreService.FindUserByUsername(username);
            if (searchUser == null)
            {
                Console.WriteLine($"User {username} was not found.");
            }
            else
            {
                Console.Write("Enter chore title to delete chore: ");
                string chore = Console.ReadLine();
                Console.Write(choreService.Delete(chore));
                Console.WriteLine(' ');
            }
        }

        private void UpdateChore()
        {
            Console.Write("Enter username to search in their chores: ");
            var username = Console.ReadLine();
            IsValid(username);
            User searchedUser = choreService.FindUserByUsername(username);
            if (searchedUser != null)
            {
                Console.Write("Enter chore title to update: ");
                string oldTitle = Console.ReadLine().Trim();
                Chore searchedChore = searchedChore = searchedUser.Chores.FirstOrDefault(x => x.Title == oldTitle);
                if (searchedChore != null)
                {
                    Console.WriteLine(' ');
                    Console.Write("Enter new title: ");
                    searchedChore.Title = Console.ReadLine();
                    string newTitle = searchedChore.Title;
                    Console.Write("Enter new description: ");
                    searchedChore.Description = Console.ReadLine();
                    string description = searchedChore.Description;
                    Console.Write("Is chore important? ");
                    var inputIsImportant = Console.ReadLine();
                    bool isImportant = false;
                    switch (inputIsImportant.Trim().ToLower())
                    {
                        case "da":
                        case "yes":
                            searchedChore.IsImportant = true;
                            isImportant = searchedChore.IsImportant;
                            break;
                        default:
                            isImportant = false;
                            break;
                    }
                    Console.Write("Is it finished? ");
                    var inputFinished = Console.ReadLine();
                    bool isFinished = false;
                    switch (inputFinished.Trim().ToLower())
                    {
                        case "da":
                        case "yes":
                            searchedChore.IsFinished = true;
                            isFinished = searchedChore.IsFinished;
                            break;
                        default:
                            isFinished = false;
                            break;
                    }
                    choreService.Update(oldTitle, newTitle, description, isImportant, isFinished);
                    Console.WriteLine(' ');
                    Console.WriteLine("Chore was updated successfully!");
                }
                else { Console.WriteLine("Chore was not found."); }
            }
            else { Console.WriteLine($"User {searchedUser} was not found."); }
        }

        private void ListAllUsers()
        {
            var usersAll = userService.GetAllUsers();
            if (usersAll.IsNullOrEmpty())
            {
                Console.WriteLine("No users yet. Maybe you should add one? ;)");
            }
            else
            {
                Console.WriteLine(' ');
                Console.WriteLine(new string('─', 23));
                Console.WriteLine('▒' + new string('░', 7) + " USERS " + new string('░', 7) + '▒');
                Console.WriteLine(new string('─', 23));
                foreach (var item in usersAll)
                {
                    Console.WriteLine(' ');
                    Console.WriteLine("▒░─ " + item.Username);
                    if (item.Bio.IsNullOrEmpty())
                    {
                        Console.WriteLine("[No Bio Yet]");
                        Console.WriteLine(' ');
                    }
                    else
                    {
                        Console.Write("Bio: " + item.Bio);
                        Console.WriteLine(' ');
                    }
                }
                Console.WriteLine(new string('─', 23));
            }
        }

        private void AddUser()
        {
            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            if (userService.UserExists(username))
            {
                Console.WriteLine($"User '{username}' already exists. Please choose a different username.");
            }
            else
            {
                if (IsValid(username) == true)
                {
                    Console.WriteLine("Add a bio if you want.");
                    Console.Write("─ ");
                    string bio = Console.ReadLine();
                    if (bio.Length <= 255)
                    {
                        userService.Create(username, bio);
                        Console.WriteLine($"User was created successfully!");
                    }
                    else { Console.WriteLine("Bio should be up to 255 characters!"); }
                    //userService.GetUser(username);
                }
            }


        }

        private bool IsValid(string username)
        {
            string pathern = "^[a-zA-Z0-9]+$";
            Regex regex = new Regex(pathern);
            if (username.IsNullOrEmpty())
            {
                Console.WriteLine("Username cannot be empty!");
                return false;
            }
            else if (username.Length < 6 || username.Length > 18)
            {
                Console.WriteLine("Username should be between 6 and 18 chracters!"); 
                return false;
            }
            else if (!regex.IsMatch(username))
            {
                Console.WriteLine("Username can only contains numbers, letters, _ and - ");
                return false;
            }
            return true;
        }

        private void UpdateUser()
        {
            Console.Write("Enter username to update: ");
            string oldName = Console.ReadLine();
            if (IsValid(oldName) == true)
            {
                Console.Write("Enter new username: ");
                string newName = Console.ReadLine();
                if (IsValid(newName) == true)
                {
                    if (oldName == newName)
                    { Console.WriteLine("Username is the same. Make sure you change at least one character!"); }
                    else
                    {
                        Console.Write("Enter new bio: ");
                        string bio = Console.ReadLine();
                        if (bio.Length <= 255)
                        {
                            userService.Update(oldName, newName, bio);
                            Console.WriteLine($"User was updated successfully!");
                        }
                        else { Console.WriteLine("Bio should be up to 255 characters!"); }

                    }

                }
            }
        }

        private void DeleteUser()
        {
            Console.Write("Enter username to delete: ");
            string username = Console.ReadLine();
            IsValid(username);
            userService.Delete(username);
            Console.WriteLine($"User {username} was deleted successfully!");
            Console.WriteLine(' ');

        }

        private void CreateChore()
        {
            Console.Write("Enter username to add chore: ");
            string username = Console.ReadLine();
            IsValid(username);
            Chore c = new()
            {
                Title = "DEFAULT"
            };

            choreService.Create(username, c);

            Console.Write("Enter chore title: ");
            var title = Console.ReadLine().Trim();
            if (title.IsNullOrEmpty())
            {
                Console.WriteLine("Title cannot be empty!");
            }
            else if (title.Length > 60)
            {
                Console.WriteLine("Title should be up to 60 characters!");
            }
            else
            {
                c.Title = title;
                Console.Write("Enter description: ");
                c.Description = Console.ReadLine();

                Console.Write("Is chore important? ");
                var input = Console.ReadLine();
                switch (input.Trim())
                {
                    case "da":
                    case "yes":
                    case "Da":
                    case "Yes":
                        c.IsImportant = true;
                        break;
                    default:
                        c.IsImportant = false;
                        break;
                }
                c.IsFinished = false;
                Console.WriteLine("Chore was created successfully!");
            }
        }

        private void ListAllChoresByUser()
        {
            Console.Write("Enter username to show chores: ");
            var username = Console.ReadLine();
            IsValid(username);

            var choresAll = choreService.GetAllChoresByUser(username);
            if (choresAll.Count != 0)
            {
                Console.WriteLine(' ');
                Console.WriteLine(new string('─', 12) + $" {username}'s");
                Console.WriteLine('▒' + new string('░', 11) + $" CHORES " + new string('░', 11) + '▒');
                Console.WriteLine(new string('─', 32));

                foreach (var item in choresAll)
                {
                    if (item.Description.IsNullOrEmpty())
                    {
                        Console.WriteLine(' ');
                        Console.WriteLine($"▒░─ {item.Title}\n{new string('─', 20)}\n[ No Description ]\nIs chore important? {item.IsImportant}\nIs it finished? {item.IsFinished}");
                        Console.WriteLine(' ');
                    }
                    else
                    {
                        Console.WriteLine(' ');
                        Console.WriteLine($"▒░─ {item.Title}\n{new string('─', 20)}\n{item.Description}\nIs chore important? {item.IsImportant}\nIs it finished? {item.IsFinished}");
                        Console.WriteLine(' ');
                    }
                }
            }
            else { Console.WriteLine($"{username} has no chores."); }
        }
    }
}
