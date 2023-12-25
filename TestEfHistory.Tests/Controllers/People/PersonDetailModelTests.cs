using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TestEfHistory.DataAccess.Model;
using TestEfHistory.DataAccess.Model.People;
using TestEfHistory.Pages.People;

namespace TestEfHistory.Tests.Controllers.People
{
    public class PersonDetailModelTests
    {
        [Fact]
        public async Task OnGet_ValidId_ReturnsPersonDetail()
        {
            // Arrange
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseSqlite(connection)
                    .Options;

                await using var context = new ApplicationDbContext(options);
                await context.Database.EnsureCreatedAsync();

                var person = new Person { Id = 1, Name = "John Doe" };
                var personHistory = new List<PersonHistory>
                {
                    new() { Id = 1, Name="Test1", ModifiedOn = DateTime.Now.AddDays(-1) },
                    new() { Id = 1, Name = "Test2", ModifiedOn = DateTime.Now.AddHours(-1) }
                };

                context.People.Add(person);
                context.PersonHistories.AddRange(personHistory);
                await context.SaveChangesAsync();

                var model = new PersonDetailModel(context)
                {
                    Id = 1
                };

                // Act
                await model.OnGet();

                // Assert
                Assert.Equal("John Doe", model.Name);
                Assert.Equal(2, model.PersonHistory.Count());
            }
            finally
            {
                connection.Close();
            }
        }

        [Fact]
        public async Task OnGet_InvalidId_RedirectsToIndex()
        {
            // Arrange
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseSqlite(connection)
                    .Options;

                await using var context = new ApplicationDbContext(options);
                await context.Database.EnsureCreatedAsync();
                var model = new PersonDetailModel(context)
                {
                    Id = 1
                };

                // Act
                var result = await model.OnGet();
                Assert.IsType<RedirectToPageResult>(result);

                var redirectToPageResult = result as RedirectToPageResult;

                Assert.Equal("/People/Index", redirectToPageResult?.PageName);
            }
            finally
            {
                connection.Close();
            }
        }

        [Fact]
        public async Task OnGet_NullId_RedirectsToIndex()
        {
            // Arrange
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseSqlite(connection)
                    .Options;

                await using var context = new ApplicationDbContext(options);
                await context.Database.EnsureCreatedAsync();
                var model = new PersonDetailModel(context)
                {
                    Id = null
                };

                // Act
                var result = await model.OnGet();
                Assert.IsType<RedirectToPageResult>(result);

                var redirectToPageResult = result as RedirectToPageResult;

                Assert.Equal("/People/Index", redirectToPageResult?.PageName);
            }
            finally
            {
                connection.Close();
            }
        }

        [Fact]
        public async Task OnGet_WithHistory_ReturnsHistoryInDescendingOrder()
        { 
            // Arrange
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseSqlite(connection)
                    .Options;

                await using var context = new ApplicationDbContext(options);
                await context.Database.EnsureCreatedAsync();

                var person = new Person { Id = 1, Name = "John Doe" };
                var personHistory = new List<PersonHistory>
                {
                    new() { Id = 1, Name = "Test1", ModifiedOn = DateTime.Now.AddDays(-1) },
                    new() { Id = 1, Name = "Test2", ModifiedOn = DateTime.Now.AddHours(-1) }
                };

                context.People.Add(person);
                context.PersonHistories.AddRange(personHistory);
                await context.SaveChangesAsync();

                var model = new PersonDetailModel(context)
                {
                    Id = 1
                };

                // Act
                await model.OnGet();

                // Assert
                Assert.Equal("John Doe", model.Name);
                Assert.Equal(2, model.PersonHistory.Count());
                Assert.Equal("Test2", model.PersonHistory.First().Name);
                Assert.Equal("Test1", model.PersonHistory.Last().Name);
            }
            finally
            {
                connection.Close();
            }
        }

        [Fact]
        public void OnPost_ValidModel_AddsPersonAndRedirectsToPersonDetail()
        {
            // Arrange
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseSqlite(connection)
                    .Options;

                using (var context = new ApplicationDbContext(options))
                {
                    context.Database.EnsureCreated();
                }

                using (var context = new ApplicationDbContext(options))
                {
                    var model = new PersonDetailModel(context)
                    {
                        Name = "Jane Doe" 
                    };

                    // Act
                    var result = model.OnPost().Result as RedirectToPageResult;

                    // Assert
                    Assert.Equal("/People/PersonDetail", result?.PageName);
                    Assert.Equal(1, result?.RouteValues?["Id"]);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [Fact]
        public async Task OnPost_InvalidModel_ReturnsPage()
        {
            // Arrange
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseSqlite(connection)
                    .Options;

                await using var context = new ApplicationDbContext(options);
                await context.Database.EnsureCreatedAsync();

                var model = new PersonDetailModel(context)
                {
                    Name = "",
                };
                model.ModelState.AddModelError("Name", "Name is required");

                // Act
                var actionResult = await model.OnPost();

                // Assert
                Assert.IsType<PageResult>(actionResult);
            }
            finally
            {
                connection.Close();
            }
        }

        [Fact]
        public async Task OnPost_ExistingId_UpdatesPersonAndRedirectsToIndex()
        {
            // Arrange
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseSqlite(connection)
                    .Options;

                await using (var context = new ApplicationDbContext(options))
                {
                    await context.Database.EnsureCreatedAsync();

                    var person = new Person { Id = 1, Name = "John Doe" };
                    context.People.Add(person);
                    await context.SaveChangesAsync();
                }

                await using (var context = new ApplicationDbContext(options))
                {
                    var model = new PersonDetailModel(context)
                    {
                        Id = 1,
                        Name = "Jane Doe"
                    };

                    // Act
                    var result = await model.OnPost();
                    Assert.IsType<RedirectToPageResult>(result);

                    var redirectToPageResult= result as RedirectToPageResult;

                    // Assert
                    
                    Assert.Null(redirectToPageResult?.PageName);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [Fact]
        public void OnPost_NonExistingId_RedirectsToIndex()
        {
            // Arrange
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseSqlite(connection)
                    .Options;

                using (var context = new ApplicationDbContext(options))
                {
                    context.Database.EnsureCreated();
                }

                using (var context = new ApplicationDbContext(options))
                {
                    var model = new PersonDetailModel(context)
                    {
                        Id = 1,
                        Name = "Jane Doe"
                    };

                    // Act
                    var result = model.OnPost().Result as RedirectToPageResult;

                    // Assert
                    Assert.Equal("/People/Index", result?.PageName);
                }
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
