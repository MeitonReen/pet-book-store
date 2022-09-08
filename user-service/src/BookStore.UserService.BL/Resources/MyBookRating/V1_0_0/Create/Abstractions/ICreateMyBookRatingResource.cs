using BookStore.Base.Implementations.Result;
using BookStore.UserService.Contracts.MyBookRating.V1_0_0.Create;

namespace BookStore.UserService.BL.Resources.MyBookRating.V1_0_0.Create.Abstractions;

public interface ICreateMyBookRatingResource
{
    Task<ResultModel> Create(CreateRequest request);
}