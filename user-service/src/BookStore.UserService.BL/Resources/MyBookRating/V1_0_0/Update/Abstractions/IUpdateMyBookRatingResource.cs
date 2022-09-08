using BookStore.Base.Implementations.Result;
using BookStore.UserService.Contracts.MyBookRating.V1_0_0.Update;

namespace BookStore.UserService.BL.Resources.MyBookRating.V1_0_0.Update.Abstractions;

public interface IUpdateMyBookRatingResource
{
    Task<ResultModel> Update(UpdateRequest request);
}