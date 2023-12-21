using Microsoft.EntityFrameworkCore;
using TestEfHistory.DataAccess.Model.People;

namespace TestEfHistory.DataAccess.Model
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Person> People { get; set; } = null!;

        public DbSet<PersonHistory> PersonHistories { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>().HasMany(t => t.PersonHistory).WithOne(t => t.Person)
                .HasForeignKey(t => t.Id);
            modelBuilder.Entity<PersonHistory>().HasKey(t => new {t.Id, t.ModifiedOn});
            base.OnModelCreating(modelBuilder);
        }
    }
}
