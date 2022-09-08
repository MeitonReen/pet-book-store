using Microsoft.AspNetCore.Mvc;

namespace BookStore.Base.Implementations.__Caching.Invalidation.Interface
{
    public interface ICacheInvalidator
    {
        Task<bool> InvalidateAsync(ControllerBase targetController,
            Type targetControllerType, string readResourceMethodName, object? readResourceRequest);
    }
}