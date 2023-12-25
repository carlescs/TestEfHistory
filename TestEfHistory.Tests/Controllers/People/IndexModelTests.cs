using Microsoft.EntityFrameworkCore;
using TestEfHistory.DataAccess.Model;
using TestEfHistory.DataAccess.Model.People;
using TestEfHistory.Pages.People;

namespace TestEfHistory.Tests.Controllers.People
{
    public class IndexModelTests
    {
        [Fact]
        public void OnGet_Should_ReturnPeople()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;

            using var context = new ApplicationDbContext(options);

            context.Database.OpenConnection();
            context.Database.EnsureCreated();

            var model = new IndexModel(context);

            // Act
            model.OnGet();

            // Assert
            Assert.NotNull(model.People);
            Assert.Empty(model.People);
        }

        [Fact]
        public void OnGet_Should_ReturnPeopleWithOnePerson()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;

            using var context = new ApplicationDbContext(options);

            context.Database.OpenConnection();
            context.Database.EnsureCreated();

            context.People.Add(new Person { Name = "Test" });
            context.SaveChanges();

            var model = new IndexModel(context);

            // Act
            model.OnGet();

            // Assert
            Assert.NotNull(model.People);
            Assert.Single(model.People);
            Assert.Equal("Test", model.People.First().Name);
            Assert.Equal(1, model.People.First().Id);
            Assert.Equal(DateTime.Today, model.People.First().CreatedOn.Date);
        }

        [Fact]
        public void OnGet_Should_ReturnPeopleWithTwoPeople()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;

            using var context = new ApplicationDbContext(options);

            context.Database.OpenConnection();
            context.Database.EnsureCreated();

            context.People.Add(new Person { Name = "Test" });
            context.People.Add(new Person { Name = "Test2" });
            context.SaveChanges();

            var model = new IndexModel(context);

            // Act
            model.OnGet();

            // Assert
            Assert.NotNull(model.People);
            Assert.Equal(2, model.People.Count());
            Assert.Equal("Test", model.People.First().Name);
            Assert.Equal(1, model.People.First().Id);
            Assert.Equal(DateTime.Today, model.People.First().CreatedOn.Date);
            Assert.Equal("Test2", model.People.Last().Name);
            Assert.Equal(2, model.People.Last().Id);
            Assert.Equal(DateTime.Today, model.People.Last().CreatedOn.Date);
        }
    }
}
