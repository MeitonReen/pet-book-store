using BookStore.Base.Implementations.Result;

namespace BookStore.OrderService.BL.Resources.MyOrder.V1_0_0.Create.Abstractions;

public interface ICreateMyOrderResourceFromMyCart
{
    Task<ResultModel> CreateFromMyCart();
}