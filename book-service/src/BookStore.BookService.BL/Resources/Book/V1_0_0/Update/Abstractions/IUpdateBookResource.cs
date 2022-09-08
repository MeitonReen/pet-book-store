using BookStore.Base.Implementations.Result;
using BookStore.BookService.Contracts.Book.V1_0_0.Update;
using UpdateBookCommand = BookStore.Base.InterserviceContracts.BookService.V1_0_0.Book.V1_0_0.Update.UpdateBookCommand;

namespace BookStore.BookService.BL.Resources.Book.V1_0_0.Update.Abstractions;

public interface IUpdateBookResource
{
    Task<ResultModel> Update(UpdateRequest request);
    Task<ResultModel> Update(UpdateBookCommand request);
}