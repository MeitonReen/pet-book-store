using BookStore.Base.Implementations.Result;

namespace BookStore.OrderService.BL.Resources.MyCart.V1_0_0.Create.Abstractions;

public interface ICreateMyCartResource
{
    Task<ResultModel> Create();
}