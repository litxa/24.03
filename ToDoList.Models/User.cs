using System.ComponentModel.DataAnnotations;

namespace ToDoList.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(18), MinLength(6)]
        public required string Username { get; set; }

        [MaxLength(255)]
        public string? Bio { get; set; }

        public virtual ICollection<Chore> Chores { get; set; } = new List<Chore>();
    }
}
