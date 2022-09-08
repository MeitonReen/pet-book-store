using BookStore.Base.Contracts.Implementations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BookStore.Base.Implementations.Controller
{
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces(MimeTypes.Application.Json)]
    public class BookStoreControllerBase : ControllerBase
    {
        protected ILogger? _logger;

        public BookStoreControllerBase(ILogger? logger = default)
        {
            _logger = logger;
        }

        public ILogger? Logger => _logger;
    }
}