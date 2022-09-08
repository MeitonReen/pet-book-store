namespace BookStore.Base.InterserviceContracts.BookService.V1_0_0.Book.V1_0_0.Delete;

public static class DeleteBookCommandActivityContracts
{
    public static class ExecuteEndpoint
    {
        public static readonly string Name = Helpers.Activities.Base.GetActivityNameKebabCaseFromTypeFullName(
            typeof(DeleteBookCommand).FullName ??
            throw new ArgumentNullException(nameof(Type.FullName)));

        public static readonly Uri Uri = new($"exchange:{Name}");
    }
}