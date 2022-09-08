using BookStore.Base.Implementations.Result;
using BookStore.Base.InterserviceContracts.BookService.V1_0_0.Book.V1_0_0.Delete;
using BookStore.BookService.Contracts.Book.V1_0_0.Delete;

namespace BookStore.BookService.BL.Resources.Book.V1_0_0.Delete.Abstractions;

public interface IDeleteBookResource
{
    Task<ResultModel> Delete(DeleteRequest request);
    Task<ResultModel> Delete(DeleteBookCommand request);
}