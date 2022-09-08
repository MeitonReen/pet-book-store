using BookStore.Base.Implementations.FluentApi;
using BookStore.Base.Implementations.Result.Builders.BadRequest.Builders;
using BookStore.Base.Implementations.Result.Builders.Conflict.Builders;
using BookStore.Base.Implementations.Result.Builders.Created.Builders;
using BookStore.Base.Implementations.Result.Builders.Deleted;
using BookStore.Base.Implementations.Result.Builders.Exception.Builders;
using BookStore.Base.Implementations.Result.Builders.NotFound.Builders;
using BookStore.Base.Implementations.Result.Builders.Read;
using BookStore.Base.Implementations.Result.Builders.Success;
using BookStore.Base.Implementations.Result.Builders.Updated;

namespace BookStore.Base.Implementations.Result
{
    public class ResultModelBuilder
    {
        public static CreatedResultBuilderRoot<TResult> Created<TResult>(TResult result) where TResult : class =>
            new(FluentApiBase.ObjectResolverFirst, result);

        public static ReadResultBuilderRoot<TResult> Read<TResult>(TResult result) =>
            new(FluentApiBase.ObjectResolverFirst, result);

        public static UpdatedResultBuilderRoot<TResult> Updated<TResult>(TResult result) =>
            new(FluentApiBase.ObjectResolverFirst, result);

        public static DeletedResultBuilderRoot<TResult> Deleted<TResult>(TResult result) =>
            new(FluentApiBase.ObjectResolverFirst, result);

        public static SuccessResultBuilderRoot<TResult> Success<TResult>(TResult result) =>
            new(FluentApiBase.ObjectResolverFirst, result);

        public static ConflictResultBuilderRoot Conflict() =>
            new(FluentApiBase.ObjectResolverFirst);

        public static NotFoundResultBuilderRoot NotFound() =>
            new(FluentApiBase.ObjectResolverFirst);

        public static BadRequestResultBuilderRoot BadRequest() =>
            new(FluentApiBase.ObjectResolverFirst);

        public static ExceptionResultBuilderRoot Exception() =>
            new(FluentApiBase.ObjectResolverFirst);

        public NotFoundResultBuilderRoot NotFound<TResult>(TResult result) =>
            throw new NotImplementedException();
    }
}