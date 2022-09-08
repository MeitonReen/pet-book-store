namespace BookStore.Base.Contracts.Abstractions.Result
{
    public enum ResultStatus
    {
        Created,
        Read,
        Updated,
        Deleted,

        Success,
        Conflict,
        NotFound,
        BadRequest,
        Exception,

        Init
    }
}