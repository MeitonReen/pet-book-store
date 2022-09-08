using BookStore.OrderService.Contracts.MyOrders.V1_0_0.Read;
using FluentValidation;

namespace BookStore.OrderService.Settings.RequestToResourceValidators.MyOrders.V1_0_0;

public class ReadMyOrdersPart : AbstractValidator<ReadPartRequest>
{
    public ReadMyOrdersPart()
    {
        RuleFor(input => input.PartLength).GreaterThan(0);
        RuleFor(input => input.PartNumber).GreaterThan(0);
    }
}