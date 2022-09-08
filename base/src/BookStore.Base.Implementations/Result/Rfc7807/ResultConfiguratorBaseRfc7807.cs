using Microsoft.AspNetCore.Mvc;

namespace BookStore.Base.Implementations.Result.Rfc7807
{
    public class ResultConfiguratorBaseRfc7807
    {
        private readonly ProblemDetails _problemDetails = new();
        public ProblemDetails ConfigureResult() => _problemDetails;

        public ResultConfiguratorBaseRfc7807 Type(string? type)
        {
            _problemDetails.Type = type;
            return this;
        }

        public ResultConfiguratorBaseRfc7807 Title(string? title)
        {
            _problemDetails.Title = title;
            return this;
        }

        public ResultConfiguratorBaseRfc7807 Status(int? status)
        {
            _problemDetails.Status = status;
            return this;
        }

        public ResultConfiguratorBaseRfc7807 Detail(string? detail)
        {
            _problemDetails.Detail = detail;
            return this;
        }

        public ResultConfiguratorBaseRfc7807 Instance(string? instance)
        {
            _problemDetails.Instance = instance;
            return this;
        }

        public ResultConfiguratorBaseRfc7807 Extensions(params
            (string paramName, object? value)[] extensions)
        {
            Array.ForEach(extensions, item =>
                _problemDetails.Extensions.Add(item.paramName, item.value));

            return this;
        }
    }
}