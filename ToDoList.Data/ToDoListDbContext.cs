using Microsoft.EntityFrameworkCore;
using ToDoList.Models;

namespace ToDoList.Data
{
    public class ToDoListDbContext : DbContext
    {

        public ToDoListDbContext()
        {
        }

        public ToDoListDbContext(DbContextOptions options)
            : base(options)
        {
        }
        public DbSet<Chore> Chores { get; set; }


        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.\\SqlExpress;DataBase=ToDoList;Integrated Security=true;TrustServerCertificate=True;");
            }
            optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>()
                .HasMany(x => x.Chores) 
                .WithOne(x => x.User) 
                .OnDelete(DeleteBehavior.Restrict); 
        }
        
    }
}