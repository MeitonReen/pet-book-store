using BookStore.Base.Implementations.Result;
using BookStore.UserService.Contracts.MyBookReview.V1_0_0.Update;

namespace BookStore.UserService.BL.Resources.MyBookReview.V1_0_0.Update.Abstractions;

public interface IUpdateMyBookReviewResource
{
    Task<ResultModel> Update(UpdateRequest request);
}