using BookStore.Base.Implementations.Result.Builders.Conflict.Builders;
using BookStore.Base.Implementations.Result.Builders.Conflict.Builders.Rfc7807;

namespace BookStore.Base.Implementations.Result.Builders.Conflict.Extensions
{
    public static class DefaultConflictResultBuilderExtensions
    {
        public static ConflictResultBuilderRfc7807 ApplyDefaultSettings(
            this ConflictResultBuilderRoot targetResultBuilder, string rewriteDetail)
            => targetResultBuilder
                .Configurator.UseRfc7807
                .Detail(rewriteDetail)
                .Builder;
    }
}