namespace BookStore.Base.Abstractions.OptionsScopedChanges;

public interface IOptionsSnapshotMixOptionsMonitor<TOptions> where TOptions : class
{
    TOptions Value { get; }
    IDisposable OnChange(Action<TOptions, string> listener);
}