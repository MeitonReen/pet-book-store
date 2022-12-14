using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace BookStore.Base.Implementations.LoggingInterpolationSupport.Helpers
{
    [InterpolatedStringHandler]
    public ref struct StructuredLoggingTraceInterpolatedStringHandler
    {
        private readonly StructuredLoggingInterpolatedStringHandler _handler;

        public StructuredLoggingTraceInterpolatedStringHandler(int literalLength, int formattedCount, ILogger logger,
            out bool isEnabled)
        {
            _handler = new StructuredLoggingInterpolatedStringHandler(literalLength, formattedCount, logger,
                LogLevel.Trace, out isEnabled);
        }

        public bool IsEnabled => _handler.IsEnabled;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendLiteral(string s) => _handler.AppendLiteral(s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendFormatted<T>(T value, [CallerArgumentExpression("value")] string name = "") =>
            _handler.AppendFormatted(value, name);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Deconstruct(out string template, out object?[] arguments) => (template, arguments) = _handler;
    }
}