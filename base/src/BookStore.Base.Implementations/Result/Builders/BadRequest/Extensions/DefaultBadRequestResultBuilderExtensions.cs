using BookStore.Base.Implementations.Result.Builders.BadRequest.Builders;
using BookStore.Base.Implementations.Result.Builders.BadRequest.Builders.Rfc7807;

namespace BookStore.Base.Implementations.Result.Builders.BadRequest.Extensions
{
    public static class DefaultBadRequestResultBuilderExtensions
    {
        public static BadRequestResultBuilderRfc7807 ApplyDefaultSettings(
            this BadRequestResultBuilderRoot targetResultBuilder, string rewriteDetail)
        {
            return targetResultBuilder
                .Configurator.UseRfc7807
                .Detail(rewriteDetail)
                .Builder;
        }
    }
}