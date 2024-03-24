using System.ComponentModel.DataAnnotations;

namespace ToDoList.Models
{
    public class Chore
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(60)]
        public required string Title { get; set; }

        [MaxLength(255)]
        public string? Description { get; set; }

        public bool IsImportant { get; set; }

        public bool IsFinished { get; set; }

        public int UserId { get; set; }

        public virtual User User {get;set;}
    }
}
