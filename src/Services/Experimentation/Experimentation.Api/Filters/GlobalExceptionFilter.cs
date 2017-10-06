using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Experimentation.Api.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter, IDisposable
    {
        private readonly ILogger _logger;

        private bool _disposed;

        public GlobalExceptionFilter(ILoggerFactory logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _logger = logger.CreateLogger("Global Exception Filter");
        }

        public void OnException(ExceptionContext context)
        {
            var response = new ErrorResponse { Message = context.Exception.Message };

#if DEBUG
            response.StackTrace = context.Exception.StackTrace;
#endif

            context.Result = new ObjectResult(response)
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                DeclaredType = typeof(ErrorResponse)
            };

            _logger.LogError(context.Exception, "An unhandled exception has been detected and caught.");
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
        }
    }
}