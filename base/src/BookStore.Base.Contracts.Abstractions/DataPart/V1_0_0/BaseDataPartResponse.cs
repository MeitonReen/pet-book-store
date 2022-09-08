namespace BookStore.Base.Contracts.Abstractions.DataPart.V1_0_0
{
    public abstract class BaseDataPartResponse<TDataPartItem>
    {
        public IEnumerable<TDataPartItem> Items { get; set; } =
            Enumerable.Empty<TDataPartItem>();

        public int PartLength { get; set; }
        public int PartNumber { get; set; }
        public int LastPartNumber { get; set; }
    }
}