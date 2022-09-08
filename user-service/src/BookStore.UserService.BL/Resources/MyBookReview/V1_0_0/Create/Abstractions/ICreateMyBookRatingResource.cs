using BookStore.Base.Implementations.Result;
using BookStore.UserService.Contracts.MyBookReview.V1_0_0.Create;

namespace BookStore.UserService.BL.Resources.MyBookReview.V1_0_0.Create.Abstractions;

public interface ICreateMyBookReviewResource
{
    Task<ResultModel> Create(CreateRequest request);
}