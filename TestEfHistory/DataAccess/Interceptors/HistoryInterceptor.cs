using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TestEfHistory.DataAccess.Interceptors.Attributes;

namespace TestEfHistory.DataAccess.Interceptors
{
    public class HistoryInterceptor : SaveChangesInterceptor
    {
        private static readonly ILoggerFactory LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder => builder.AddConsole());
        private static readonly ILogger<HistoryInterceptor> Logger = LoggerFactory.CreateLogger<HistoryInterceptor>();
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            HandleChanges(eventData);

            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
            CancellationToken cancellationToken = new())
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
                var type= entry.Entity.GetType();
                if(type.GetCustomAttribute(typeof(HistoryAttribute)) is not HistoryAttribute historyAttribute)
                    continue;
                var properties= type.GetProperties().Where(t => t.GetCustomAttribute(typeof(HistoryFieldAttribute)) is not null);
                var history = Activator.CreateInstance(historyAttribute.HistoryType);
                if (history == null) continue;
                foreach (var property in properties)
                {
                    if (property.GetCustomAttribute(typeof(HistoryFieldAttribute)) is not HistoryFieldAttribute historyFieldAttribute)
                        continue;
                    Logger.LogInformation($"HistoryFieldAttribute found for {type.Name}.{property.Name}: {historyFieldAttribute.PropertyName}");
                    var originalValue = entry.Property(property.Name).OriginalValue;
                    var currentValue = entry.Property(property.Name).CurrentValue;
                    if (originalValue == null && currentValue == null)
                        continue;
                    
                    var historyProperty = historyAttribute.HistoryType.GetProperty(historyFieldAttribute.PropertyName);
                    if (historyProperty == null)
                        continue;

                    historyProperty.SetValue(history, originalValue);

                    if (historyFieldAttribute.IsTimestamp)
                    {
                        if(historyProperty.PropertyType == typeof(DateTime))
                            entry.Property(property.Name).CurrentValue = DateTime.UtcNow;
                        else if (historyProperty.PropertyType == typeof(DateTime?))
                            entry.Property(property.Name).CurrentValue = DateTime.UtcNow;
                        else if (historyProperty.PropertyType == typeof(DateTimeOffset))
                            entry.Property(property.Name).CurrentValue = DateTime.UtcNow;
                        else if (historyProperty.PropertyType == typeof(DateTimeOffset?))
                            entry.Property(property.Name).CurrentValue = DateTime.UtcNow;
                    }

                    eventData.Context?.Add(history);
                }
            }
        }
    }
}
