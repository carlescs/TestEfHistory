using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TestEfHistory.DataAccess.Model.People;

namespace TestEfHistory.DataAccess.Interceptors
{
    public class HistoryInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            HandleChanges(eventData);

            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            HandleChanges(eventData);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private static void HandleChanges(DbContextEventData eventData)
        {
            var entries = eventData.Context?.ChangeTracker.Entries();
            if (entries == null) return;
            foreach (var entry in entries.Where(t => t.State is EntityState.Modified))
            {
                if (entry.Entity is Person person)
                {
                    entry.Property(nameof(Person.UpdatedOn)).CurrentValue = DateTime.UtcNow;
                    var originalModifiedOn = entry.Property(nameof(Person.UpdatedOn)).OriginalValue;
                    if (originalModifiedOn == null)
                        continue;
                    var history = new PersonHistory
                    {
                        Id = person.Id,
                        ModifiedOn = (DateTime)originalModifiedOn,
                        Name = entry.Property(nameof(Person.Name)).OriginalValue?.ToString() ?? string.Empty
                    };
                    eventData.Context?.Add(history);
                }
            }
        }
    }
}
