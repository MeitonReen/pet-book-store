namespace BookStore.AuthorizationService.Defaults;

public static class BookStoreDefaultScopes
{
    public const string DefaultBookStoreResourcesCRUD =
        $"{BookStoreDefaultResources.DefaultBookStoreResources}.crud";

    public const string DefaultUserResourcesCRUD =
        $"{BookStoreDefaultResources.DefaultUserResources}.crud";

    public const string DefaultAdminResourcesCRUD =
        $"{BookStoreDefaultResources.DefaultAdminResources}.crud";
}