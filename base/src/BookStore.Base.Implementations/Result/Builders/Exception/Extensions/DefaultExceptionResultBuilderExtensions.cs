using BookStore.Base.Implementations.Result.Builders.Exception.Builders;
using BookStore.Base.Implementations.Result.Builders.Exception.Builders.Rfc7807;

namespace BookStore.Base.Implementations.Result.Builders.Exception.Extensions
{
    public static class DefaultExceptionResultBuilderExtensions
    {
        public static ExceptionResultBuilderRfc7807 ApplyDefaultSettings(
            this ExceptionResultBuilderRoot targetResultBuilder)
        {
            return targetResultBuilder
                .Configurator.UseRfc7807
                .Builder;
        }

        public static ExceptionResultBuilderRfc7807 ApplyDefaultSettings(
            this ExceptionResultBuilderRoot targetResultBuilder, string rewriteDetail)
        {
            return targetResultBuilder
                .Configurator
                .UseRfc7807
                .Detail(rewriteDetail)
                .Builder;
        }
    }
}