using BookStore.Base.Implementations.Result;

namespace BookStore.OrderService.BL.Resources.Book.V1_0_0.Delete.Abstractions;

public interface IDeleteBookResource
{
    Task<ResultModel> Delete(Guid bookId);
}