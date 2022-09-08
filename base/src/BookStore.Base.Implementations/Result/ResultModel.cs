using BookStore.Base.Contracts.Abstractions.Result;

namespace BookStore.Base.Implementations.Result
{
    public class ResultModel
    {
        public ResultModel()
        {
        }

        public ResultModel(object? result)
        {
            Result = result;
        }

        public ResultModel(object? result, ResultStatus resultStatus)
        {
            Result = result;
            ResultStatus = resultStatus;
        }

        public object? Result { get; set; }
        public ResultStatus ResultStatus { get; set; } = ResultStatus.Init;

        public bool IsCreated => ResultStatus == ResultStatus.Created;
        public bool IsRead => ResultStatus == ResultStatus.Read;
        public bool IsUpdated => ResultStatus == ResultStatus.Updated;
        public bool IsDeleted => ResultStatus == ResultStatus.Deleted;
        public bool IsSuccess => ResultStatus == ResultStatus.Success;
        public bool IsConflict => ResultStatus == ResultStatus.Conflict;
        public bool IsNotFound => ResultStatus == ResultStatus.NotFound;
        public bool IsBadRequest => ResultStatus == ResultStatus.BadRequest;
        public bool IsException => ResultStatus == ResultStatus.Exception;
        public bool IsInit => ResultStatus == ResultStatus.Init;
    }
}