using BookStore.Base.Contracts.Implementations;
using BookStore.Base.Implementations.__Obsolete;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BookStore.Base.Implementations.OpenApi.OperationFilters
{
    public class OpenApiSetFromBodyContentType : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var isExistsFromBodyAttribute =
                context.ApiDescription.ParameterDescriptions
                    .Any(param => param.Source.Id == Constants.BindingSource.Body);

            if (isExistsFromBodyAttribute)
            {
                operation.RequestBody.Content = operation.RequestBody.Content
                    .Where(request =>
                        request.Key == MimeTypes.Application.Json)
                    .ToDictionary(pair => pair.Key, pair => pair.Value);
            }
        }
    }
}