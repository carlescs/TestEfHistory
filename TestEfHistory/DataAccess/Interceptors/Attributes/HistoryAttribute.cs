namespace TestEfHistory.DataAccess.Interceptors.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class HistoryAttribute(Type historyType) : Attribute
    {
        public Type HistoryType { get; } = historyType;
    }
}
