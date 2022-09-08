namespace BookStore.Base.InterserviceContracts.OrderService.V1_0_0.Book.V1_0_0.Update;

public static class UpdateBookCommandActivityContracts
{
    public static class ExecuteEndpoint
    {
        public static readonly string Name = Helpers.Activities.Base.GetActivityNameKebabCaseFromTypeFullName(
            typeof(UpdateBookCommand).FullName ??
            throw new ArgumentNullException(nameof(Type.FullName)));

        public static readonly Uri Uri = new($"exchange:{Name}");
    }
}