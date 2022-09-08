using BookStore.Base.Implementations.Result;
using BookStore.OrderService.Contracts.MyCart.V1_0_0.Book.Create;

namespace BookStore.OrderService.BL.Resources.MyCart.V1_0_0.Book.Create.Abstractions;

public interface ICreateBookInMyCartResource
{
    Task<ResultModel> CreateBook(CreateBookRequest request);
}