using Microsoft.Extensions.Configuration;

namespace BookStore.Base.Implementations.ExtendedConfigurationBinder;

public class ExtendedBinderOptions : BinderOptions
{
    public CasesSupport CasesSupport { get; } = new();
    public bool IncludeRootConfigClassAsConfigProperty { get; set; } = false;
}