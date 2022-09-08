using BookStore.Base.Abstractions.BaseResources.Inner;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Base.Implementations.BaseResources.Inner;

public class DataFromResourceByEfCore<TData> : IReadableDataFromResource<TData>
    where TData : class
{
    private readonly IQueryable<TData> _dataFromResource;
    private Func<IQueryable<TData>, IQueryable<TData>> _readSettings;

    public DataFromResourceByEfCore(
        IQueryable<TData> dataFromResource)
    {
        _dataFromResource = dataFromResource;

        _readSettings = data => data;
    }

    public TData? Read() => _readSettings(_dataFromResource).SingleOrDefault();

    public async Task<TData?> ReadAsync() => await _readSettings(_dataFromResource)
        .SingleOrDefaultAsync();

    public IReadableDataFromResource<TData> ReadSettings(
        Func<IQueryable<TData>,
            IQueryable<TData>> readSettings)
    {
        _readSettings = readSettings;
        return this;
    }

    public IReadableDataFromResource<TData> AddReadSettings(
        Func<IQueryable<TData>, IQueryable<TData>> readSettings)
    {
        _readSettings = readQuery => readSettings(
            _readSettings(readQuery));
        return this;
    }

    public IReadableDataFromResource<TNewResultData>
        ReadSettings<TNewResultData>(Func<IQueryable<TData>,
            IQueryable<TNewResultData>> readSettings) where TNewResultData : class
        => new DataFromResourceByEfCore<TNewResultData>
        (readSettings(
            _readSettings(_dataFromResource)));

    public IReadableDataFromResource<TNewResultData>
        AddReadSettings<TNewResultData>(Func<IQueryable<TData>,
            IQueryable<TNewResultData>> readSettings) where TNewResultData : class
        => new DataFromResourceByEfCore<TNewResultData>
        (readSettings(
            _readSettings(_dataFromResource)));
}