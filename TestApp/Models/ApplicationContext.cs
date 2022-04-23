using Microsoft.EntityFrameworkCore;

namespace TestApp.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<DeactivatedToken> DeactivatedTokens { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Solution> Solutions { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options): base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(x => x.Id);
            modelBuilder.Entity<User>().HasIndex(x => x.Username).IsUnique();

            modelBuilder.Entity<Topic>().HasKey(x => x.Id);
            modelBuilder.Entity<Topic>().Property(x => x.ParentId).HasDefaultValue(null);
            modelBuilder.Entity<Topic>().Property(x => x.ParentId).IsRequired(false);
            modelBuilder.Entity<Topic>()
                .HasOne(x => x.Parent)
                .WithMany(x => x.Childs)
                .OnDelete(DeleteBehavior.NoAction)
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<DeactivatedToken>().HasKey(x => x.Id);

            modelBuilder.Entity<Task>().HasKey(x => x.Id);
            modelBuilder.Entity<Task>()
                .HasOne(x => x.Topic)
                .WithMany(x => x.Tasks)
                .HasForeignKey(x => x.TopicId);

            modelBuilder.Entity<Solution>().HasKey(x => x.Id);
            modelBuilder.Entity<Solution>()
                .HasOne(x => x.Task)
                .WithMany(x => x.Solutions)
                .HasForeignKey(x => x.TaskId);
            modelBuilder.Entity<Solution>()
                .HasOne(x => x.User)
                .WithMany(x => x.Solutions)
                .HasForeignKey(x => x.AuthorId);
        }
    }
}