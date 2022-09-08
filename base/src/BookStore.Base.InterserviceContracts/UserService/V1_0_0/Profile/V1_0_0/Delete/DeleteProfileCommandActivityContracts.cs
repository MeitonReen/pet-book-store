namespace BookStore.Base.InterserviceContracts.UserService.V1_0_0.Profile.V1_0_0.Delete;

public static class DeleteProfileCommandActivityContracts
{
    public static class ExecuteEndpoint
    {
        public static readonly string Name = Helpers.Activities.Base.GetActivityNameKebabCaseFromTypeFullName(
            typeof(DeleteProfileCommand).FullName ??
            throw new ArgumentNullException(nameof(Type.FullName)));

        public static readonly Uri Uri = new($"exchange:{Name}");
    }
}