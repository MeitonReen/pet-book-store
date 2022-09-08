using BookStore.Base.Abstractions.UserClaimsProfile.Contracts.Profile;
using BookStore.Base.Implementations.CorrelationId.Accessor.Abstractions;
using BookStore.Base.Implementations.LoggingInterpolationSupport;
using BookStore.Base.Implementations.UserClaimsProfile.Contracts.Profile.Implementations.Default.Extensions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace BookStore.Base.Implementations.BookStoreDefaultLogging.ActionMethods;

public class BookStoreLoggingActionMethodFilter : IAsyncActionFilter
{
    private const string NotAddedUserClaimsProfileAccessorString =
        $"{nameof(IUserClaimsProfile)} not inject to {nameof(BookStoreLoggingActionMethodFilter)} from di; don't set services.{nameof(ServiceCollectionExtensions.AddDefaultUserClaimsProfile)}?";

    private const string NotAddedCorrelationIdAccessorString =
        $"{nameof(ICorrelationIdAccessor)} not inject to {nameof(BookStoreLoggingActionMethodFilter)} from di; don't set services.{nameof(CorrelationId.Accessor.Extensions.ServiceCollectionExtensions.AddCorrelationIdAccessor)}?";

    private readonly ICorrelationIdAccessor? _correlationIdAccessor;
    private readonly ILogger<BookStoreLoggingActionMethodFilter>? _logger;
    private readonly IUserClaimsProfile? _userClaimsProfileAccessor;

    public BookStoreLoggingActionMethodFilter(
        ICorrelationIdAccessor? correlationIdAccessor = default,
        IUserClaimsProfile? userClaimsProfileAccessor = default,
        ILogger<BookStoreLoggingActionMethodFilter>? logger = default)
    {
        _correlationIdAccessor = correlationIdAccessor;
        _userClaimsProfileAccessor = userClaimsProfileAccessor;
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        if (_logger == default) return;

        var actionName = (context.ActionDescriptor as ControllerActionDescriptor)
            ?.ActionName ?? "<action does not belong controller>";
        var actionParam = context.ActionArguments.FirstOrDefault().Value;

        var userName = _userClaimsProfileAccessor == default
            ? NotAddedUserClaimsProfileAccessorString
            : _userClaimsProfileAccessor.Name;
        var correlationId = _correlationIdAccessor == default
            ? NotAddedCorrelationIdAccessorString
            : _correlationIdAccessor.CorrelationId;

        _logger.LogDebug(
            $"\n{actionName} method was started;"
            + $"\nuser name: @{userName};"
            + $"\naction param: @{actionParam};"
            + $"\ncorrelationId: @{correlationId}.");

        var executedContext = await next();

        if (executedContext.Exception == default)
        {
            _logger.LogDebug(
                $"\n{actionName} method successfully completed;"
                + $"\ncorrelationId: @{correlationId}.");
        }
        else
        {
            _logger?.LogError(
                $"\n{actionName} method threw exception: {executedContext.Exception.Message};"
                + $"\nInner exception: {executedContext.Exception.InnerException?.Message}.");
        }
    }
}