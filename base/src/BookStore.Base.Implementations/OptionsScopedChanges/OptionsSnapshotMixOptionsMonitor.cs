using BookStore.Base.Abstractions.OptionsScopedChanges;
using Microsoft.Extensions.Options;

namespace BookStore.Base.Implementations.OptionsScopedChanges;

public class OptionsSnapshotMixOptionsMonitor<TOptions> :
    IOptionsSnapshotMixOptionsMonitor<TOptions> where TOptions : class

{
    private readonly IOptionsMonitor<TOptions> _targetOptions;

    public OptionsSnapshotMixOptionsMonitor(
        IOptionsMonitor<TOptions> targetOptions)
    {
        Value = targetOptions.CurrentValue;
        _targetOptions = targetOptions;
    }

    public TOptions Value { get; }

    public IDisposable OnChange(Action<TOptions, string> listener) =>
        _targetOptions.OnChange(listener);
}