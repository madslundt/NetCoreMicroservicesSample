using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;

namespace Infrastructure.Core
{
    public class ExceptionFilter : Attribute, IExceptionFilter
    {
        protected virtual HttpStatusCode MapStatusCode(Exception ex)
        {
            // Status Codes
            if (ex is ArgumentNullException)
            {
                return HttpStatusCode.NotFound;
            }
            else if (ex is ValidationException)
            {
                return HttpStatusCode.BadRequest;
            }
            else if (ex is UnauthorizedAccessException)
            {
                return HttpStatusCode.Unauthorized;
            }
            else if (ex is DuplicateNameException)
            {
                return HttpStatusCode.Conflict;
            }
            else
            {
                return HttpStatusCode.InternalServerError;
            }
        }

        private readonly IWebHostEnvironment _env;
        private readonly ILogger _logger;

        public ExceptionFilter(
            IWebHostEnvironment env,
            ILogger<ExceptionFilter> logger)
        {
            _env = env;
            _logger = logger;
        }

        public virtual void OnException(ExceptionContext context)
        {
            if (context.Exception is Exception)
            {
                var content = new Dictionary<string, object>
                {
                    { "ErrorMessage", context.Exception.Message },
                };

                if (_env.IsDevelopment())
                {
                    content.Add("Exception", context.Exception);
                }

                var statusCode = (int)MapStatusCode(context.Exception);

                LogError(context, statusCode);

                context.Result = new ObjectResult(content);
                context.HttpContext.Response.StatusCode = statusCode;
                context.Exception = null;
            }
        }

        protected virtual void LogError(ExceptionContext context, int statusCode)
        {
            var logTitle = $"{context.HttpContext.Request.Path} :: [{statusCode}] {context.Exception.Message}";
            var logError = new
            {
                Context = context,
            };

            if (statusCode >= 500)
            {
                _logger.LogCritical(logTitle, logError);
            }
            else if (statusCode == 401)
            {
                _logger.LogInformation(logTitle, logError);
            }
            else
            {
                _logger.LogWarning(logTitle, logError);
            }
        }
    }
}
