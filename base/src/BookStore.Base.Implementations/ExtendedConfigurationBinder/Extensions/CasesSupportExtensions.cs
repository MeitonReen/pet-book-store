using BookStore.Base.NonAttachedExtensions;

namespace BookStore.Base.Implementations.ExtendedConfigurationBinder.Extensions;

public static class CasesSupportExtensions
{
    public static CasesSupport AddUpperSnakeCase(this CasesSupport casesSupport)
        => casesSupport.Add(classOption => classOption.ToUpperSnakeCase());
}