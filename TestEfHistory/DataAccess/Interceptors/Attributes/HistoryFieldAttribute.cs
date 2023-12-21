namespace TestEfHistory.DataAccess.Interceptors.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class HistoryFieldAttribute(string propertyName, bool isTimestamp = false) : Attribute
    {
        public string PropertyName { get; } = propertyName;
        public bool IsTimestamp { get; } = isTimestamp;
    }
}

