using System.Runtime.CompilerServices;
using System.Text;
using BookStore.Base.NonAttachedExtensions;
using Microsoft.Extensions.Logging;

namespace BookStore.Base.Implementations.LoggingInterpolationSupport
{
    [InterpolatedStringHandler]
    public ref struct StructuredLoggingInterpolatedStringHandler
    {
        private readonly StringBuilder _template = default!;
        private readonly ArgumentList _arguments = default!;

        private readonly OtherMutableProperties _otherMutableProperties = default!;
        public bool IsEnabled { get; }

        public StructuredLoggingInterpolatedStringHandler(int literalLength,
            int formattedCount, ILogger logger, LogLevel logLevel, out bool isEnabled)
        {
            IsEnabled = isEnabled = logger.IsEnabled(logLevel);
            if (!isEnabled) return;

            _template = new StringBuilder(literalLength + 20 * formattedCount);
            _arguments = new ArgumentList(formattedCount);
            _otherMutableProperties = new OtherMutableProperties();
        }

        public void AppendLiteral(string stringPart)
        {
            if (!IsEnabled) return;

            if (stringPart.EndsWith('@'))
                _otherMutableProperties.ExpandNextParamAsJson = true;

            _template.Append(string
                .Concat(stringPart
                    .Select(item => item switch
                    {
                        '{' => "{{",
                        '}' => "}}",
                        '@' => "",
                        _ => item.ToString()
                    }))
            );
        }

        public void AppendFormatted<T>(T value, [CallerArgumentExpression("value")] string name = "")
        {
            if (!IsEnabled) return;

            if (name.StartsWith("nameof"))
            {
                _template.Append(value);
                return;
            }

            _arguments.Add(value);
            _template.Append($"{{{(_otherMutableProperties.ExpandNextParamAsJson ? '@' : "")}" +
                             $"{name.GetDefaultLastMemberName()}_{++_otherMutableProperties.UniqueVariablePart}}}");

            _otherMutableProperties.ExpandNextParamAsJson = false;
        }

        public void Deconstruct(out string template, out object?[] arguments)
        {
            template = _template.ToString();
            arguments = _arguments.Arguments;
        }

        private class OtherMutableProperties
        {
            public uint UniqueVariablePart { get; set; }
            public bool ExpandNextParamAsJson { get; set; }
        }

        private class ArgumentList
        {
            private int _index;

            public ArgumentList(int formattedCount) => Arguments = new object?[formattedCount];
            public object?[] Arguments { get; }
            public void Add(object? value) => Arguments[_index++] = value;
        }
    }
}