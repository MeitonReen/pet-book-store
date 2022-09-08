namespace BookStore.Base.Implementations.Result.Extensions
{
    public static class ResultModelGenericExtensions
    {
        public static ResultModelGeneric<TResult> ToResultModelGeneric<TResult>(
            this ResultModel resultModel)
        {
            return new ResultModelGeneric<TResult>((TResult) resultModel.Result,
                resultModel.ResultStatus);
        }
    }
}