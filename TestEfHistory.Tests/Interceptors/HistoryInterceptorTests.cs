using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TestEfHistory.DataAccess.Interceptors;
using TestEfHistory.DataAccess.Model;
using TestEfHistory.DataAccess.Model.People;

namespace TestEfHistory.Tests.Interceptors
{
    public class HistoryInterceptorTests
    {
        private readonly ApplicationDbContext _context;
        public HistoryInterceptorTests()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(connection)
                .AddInterceptors(new HistoryInterceptor())
                .Options;
            _context = new ApplicationDbContext(options);
        }

        [Fact]
        public async Task AddPerson_AddsEntryInHistory()
        {
            await _context.Database.EnsureCreatedAsync();
            var person = new Person { Name = "Test" };
            var entry=_context.People.Add(person);
            await _context.SaveChangesAsync();
            var modifiedOn=entry.Entity.UpdatedOn;
            entry.Entity.Name="Test2";
            await _context.SaveChangesAsync();
            var history = await _context.PersonHistories.ToListAsync();
            Assert.Single(history);
            Assert.Equal("Test", history[0].Name);
            Assert.Equal(modifiedOn, history[0].ModifiedOn);
        }
    }
}
