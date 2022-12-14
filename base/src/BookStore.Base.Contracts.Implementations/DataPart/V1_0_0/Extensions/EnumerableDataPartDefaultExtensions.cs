using BookStore.Base.Contracts.Abstractions.DataPart.V1_0_0;

namespace BookStore.Base.Contracts.Implementations.DataPart.V1_0_0.Extensions;

public static class EnumerableDataPartDefaultExtensions
{
    public static IEnumerable<TDataPartItem> ReadPart<TDataPartItem>(
        this IEnumerable<TDataPartItem> dataSet,
        BaseDataPartRequest baseDataPartRequest)
        => dataSet
            .Skip((baseDataPartRequest.PartNumber - 1)
                  * baseDataPartRequest.PartLength)
            .Take(baseDataPartRequest.PartLength);

    public static IQueryable<TDataPartItem> ReadPart<TDataPartItem>(
        this IQueryable<TDataPartItem> query,
        BaseDataPartRequest baseDataPartRequest)
        => query
            .Skip((baseDataPartRequest.PartNumber - 1)
                  * baseDataPartRequest.PartLength)
            .Take(baseDataPartRequest.PartLength);

    public static BaseDataPartResponse<TDataPartItem> ToDataPartResponse<TDataPartItem>(
        this ICollection<TDataPartItem> dataSetPart,
        BaseDataPartRequest baseDataPartRequest,
        int dataTotalCount)
    {
        var length = dataSetPart.Count;

        return new DefaultDataPartResponse<TDataPartItem>
        {
            Items = dataSetPart.ToAsyncEnumerable(),
            PartLength = length,
            PartNumber = length == 0 ? 0 : baseDataPartRequest.PartNumber,
            LastPartNumber = (int) Math
                .Ceiling((double) dataTotalCount / baseDataPartRequest.PartLength)
        };
    }
}