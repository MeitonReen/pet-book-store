using BookStore.Base.Implementations.Result.Builders.NotFound.Builders;
using BookStore.Base.Implementations.Result.Builders.NotFound.Builders.Rfc7807;

namespace BookStore.Base.Implementations.Result.Builders.NotFound.Extensions
{
    public static class DefaultNotFoundResultBuilderExtensions
    {
        public static NotFoundResultBuilderRfc7807 ApplyDefaultSettings(
            this NotFoundResultBuilderRoot targetResultBuilder)
        {
            return targetResultBuilder
                .Configurator.UseRfc7807
                .Builder;
        }

        public static NotFoundResultBuilderRfc7807 ApplyDefaultSettings(
            this NotFoundResultBuilderRoot targetResultBuilder, string rewriteDetail)
        {
            return targetResultBuilder
                .Configurator.UseRfc7807
                .Detail(rewriteDetail)
                .Builder;
        }
    }
}