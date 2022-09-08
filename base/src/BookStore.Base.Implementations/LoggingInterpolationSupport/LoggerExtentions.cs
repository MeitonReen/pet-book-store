using System.Runtime.CompilerServices;
using BookStore.Base.Implementations.LoggingInterpolationSupport.Helpers;
using Microsoft.Extensions.Logging;

namespace BookStore.Base.Implementations.LoggingInterpolationSupport
{
    public static class LoggerExtensions
    {
        #region Trace

        public static void LogTrace(this ILogger logger,
            [InterpolatedStringHandlerArgument("logger")]
            ref StructuredLoggingTraceInterpolatedStringHandler handler)
        {
            if (handler.IsEnabled)
            {
                var (template, arguments) = handler;
                logger.LogTrace(template, arguments);
            }
        }

        public static void LogTrace(this ILogger logger, EventId eventId,
            [InterpolatedStringHandlerArgument("logger")]
            ref StructuredLoggingTraceInterpolatedStringHandler handler)
        {
            if (handler.IsEnabled)
            {
                var (template, arguments) = handler;
                logger.LogTrace(eventId, template, arguments);
            }
        }

        public static void LogTrace(this ILogger logger, Exception? exception,
            [InterpolatedStringHandlerArgument("logger")]
            ref StructuredLoggingTraceInterpolatedStringHandler handler)
        {
            if (handler.IsEnabled)
            {
                var (template, arguments) = handler;
                logger.LogTrace(exception, template, arguments);
            }
        }

        public static void LogTrace(this ILogger logger, EventId eventId, Exception? exception,
            [InterpolatedStringHandlerArgument("logger")]
            ref StructuredLoggingTraceInterpolatedStringHandler handler)
        {
            if (handler.IsEnabled)
            {
                var (template, arguments) = handler;
                logger.LogTrace(eventId, exception, template, arguments);
            }
        }

        #endregion

        #region Debug

        public static void LogDebug(this ILogger logger,
            [InterpolatedStringHandlerArgument("logger")]
            ref StructuredLoggingDebugInterpolatedStringHandler handler)
        {
            if (handler.IsEnabled)
            {
                var (template, arguments) = handler;
                logger.LogDebug(template, arguments);
            }
        }

        public static void LogDebug(this ILogger logger, EventId eventId,
            [InterpolatedStringHandlerArgument("logger")]
            ref StructuredLoggingDebugInterpolatedStringHandler handler)
        {
            if (handler.IsEnabled)
            {
                var (template, arguments) = handler;
                logger.LogDebug(eventId, template, arguments);
            }
        }

        public static void LogDebug(this ILogger logger, Exception? exception,
            [InterpolatedStringHandlerArgument("logger")]
            ref StructuredLoggingDebugInterpolatedStringHandler handler)
        {
            if (handler.IsEnabled)
            {
                var (template, arguments) = handler;
                logger.LogDebug(exception, template, arguments);
            }
        }

        public static void LogDebug(this ILogger logger, EventId eventId, Exception? exception,
            [InterpolatedStringHandlerArgument("logger")]
            ref StructuredLoggingDebugInterpolatedStringHandler handler)
        {
            if (handler.IsEnabled)
            {
                var (template, arguments) = handler;
                logger.LogDebug(eventId, exception, template, arguments);
            }
        }

        #endregion

        #region Information

        public static void LogInformation(this ILogger logger,
            [InterpolatedStringHandlerArgument("logger")]
            ref StructuredLoggingInformationInterpolatedStringHandler handler)
        {
            if (handler.IsEnabled)
            {
                var (template, arguments) = handler;
                logger.LogInformation(template, arguments);
            }
        }

        public static void LogInformation(this ILogger logger, EventId eventId,
            [InterpolatedStringHandlerArgument("logger")]
            ref StructuredLoggingInformationInterpolatedStringHandler handler)
        {
            if (handler.IsEnabled)
            {
                var (template, arguments) = handler;
                logger.LogInformation(eventId, template, arguments);
            }
        }

        public static void LogInformation(this ILogger logger, Exception? exception,
            [InterpolatedStringHandlerArgument("logger")]
            ref StructuredLoggingInformationInterpolatedStringHandler handler)
        {
            if (handler.IsEnabled)
            {
                var (template, arguments) = handler;
                logger.LogInformation(exception, template, arguments);
            }
        }

        public static void LogInformation(this ILogger logger, EventId eventId, Exception? exception,
            [InterpolatedStringHandlerArgument("logger")]
            ref StructuredLoggingInformationInterpolatedStringHandler handler)
        {
            if (handler.IsEnabled)
            {
                var (template, arguments) = handler;
                logger.LogInformation(eventId, exception, template, arguments);
            }
        }

        #endregion

        #region Warning

        public static void LogWarning(this ILogger logger,
            [InterpolatedStringHandlerArgument("logger")]
            ref StructuredLoggingWarningInterpolatedStringHandler handler)
        {
            if (handler.IsEnabled)
            {
                var (template, arguments) = handler;
                logger.LogWarning(template, arguments);
            }
        }

        public static void LogWarning(this ILogger logger, EventId eventId,
            [InterpolatedStringHandlerArgument("logger")]
            ref StructuredLoggingWarningInterpolatedStringHandler handler)
        {
            if (handler.IsEnabled)
            {
                var (template, arguments) = handler;
                logger.LogWarning(eventId, template, arguments);
            }
        }

        public static void LogWarning(this ILogger logger, Exception? exception,
            [InterpolatedStringHandlerArgument("logger")]
            ref StructuredLoggingWarningInterpolatedStringHandler handler)
        {
            if (handler.IsEnabled)
            {
                var (template, arguments) = handler;
                logger.LogWarning(exception, template, arguments);
            }
        }

        public static void LogWarning(this ILogger logger, EventId eventId, Exception? exception,
            [InterpolatedStringHandlerArgument("logger")]
            ref StructuredLoggingWarningInterpolatedStringHandler handler)
        {
            if (handler.IsEnabled)
            {
                var (template, arguments) = handler;
                logger.LogWarning(eventId, exception, template, arguments);
            }
        }

        #endregion

        #region Error

        public static void LogError(this ILogger logger,
            [InterpolatedStringHandlerArgument("logger")]
            ref StructuredLoggingErrorInterpolatedStringHandler handler)
        {
            if (handler.IsEnabled)
            {
                var (template, arguments) = handler;
                logger.LogError(template, arguments);
            }
        }

        public static void LogError(this ILogger logger, EventId eventId,
            [InterpolatedStringHandlerArgument("logger")]
            ref StructuredLoggingErrorInterpolatedStringHandler handler)
        {
            if (handler.IsEnabled)
            {
                var (template, arguments) = handler;
                logger.LogError(eventId, template, arguments);
            }
        }

        public static void LogError(this ILogger logger, Exception? exception,
            [InterpolatedStringHandlerArgument("logger")]
            ref StructuredLoggingErrorInterpolatedStringHandler handler)
        {
            if (handler.IsEnabled)
            {
                var (template, arguments) = handler;
                logger.LogError(exception, template, arguments);
            }
        }

        public static void LogError(this ILogger logger, EventId eventId, Exception? exception,
            [InterpolatedStringHandlerArgument("logger")]
            ref StructuredLoggingErrorInterpolatedStringHandler handler)
        {
            if (handler.IsEnabled)
            {
                var (template, arguments) = handler;
                logger.LogError(eventId, exception, template, arguments);
            }
        }

        #endregion

        #region Critical

        public static void LogCritical(this ILogger logger,
            [InterpolatedStringHandlerArgument("logger")]
            ref StructuredLoggingCriticalInterpolatedStringHandler handler)
        {
            if (handler.IsEnabled)
            {
                var (template, arguments) = handler;
                logger.LogCritical(template, arguments);
            }
        }

        public static void LogCritical(this ILogger logger, EventId eventId,
            [InterpolatedStringHandlerArgument("logger")]
            ref StructuredLoggingCriticalInterpolatedStringHandler handler)
        {
            if (handler.IsEnabled)
            {
                var (template, arguments) = handler;
                logger.LogCritical(eventId, template, arguments);
            }
        }

        public static void LogCritical(this ILogger logger, Exception? exception,
            [InterpolatedStringHandlerArgument("logger")]
            ref StructuredLoggingCriticalInterpolatedStringHandler handler)
        {
            if (handler.IsEnabled)
            {
                var (template, arguments) = handler;
                logger.LogCritical(exception, template, arguments);
            }
        }

        public static void LogCritical(this ILogger logger, EventId eventId, Exception? exception,
            [InterpolatedStringHandlerArgument("logger")]
            ref StructuredLoggingCriticalInterpolatedStringHandler handler)
        {
            if (handler.IsEnabled)
            {
                var (template, arguments) = handler;
                logger.LogCritical(eventId, exception, template, arguments);
            }
        }

        #endregion
    }
}