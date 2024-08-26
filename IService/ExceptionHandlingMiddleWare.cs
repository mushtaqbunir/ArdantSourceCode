using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace ArdantOffical.IService
{
    public class ExceptionHandlingMiddleWare
    {

        public class ErrorHandlerMiddleware
        {
            private readonly RequestDelegate _next;

            public ErrorHandlerMiddleware(RequestDelegate next)
            {
                _next = next;
            }

            public async Task Invoke(HttpContext context)
            {
                try
                {
                    await _next(context);
                }
                catch (Exception error)
                {
                    HttpResponse response = context.Response;
                    response.ContentType = "application/json";

                    switch (error)
                    {
                        case AppException e:
                            // custom application error
                            response.StatusCode = (int)HttpStatusCode.BadRequest;
                            break;
                        case KeyNotFoundException e:
                            // not found error
                            response.StatusCode = (int)HttpStatusCode.NotFound;
                            break;
                        default:
                            // unhandled error
                            response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            break;
                    }

                    string result = JsonSerializer.Serialize(new { message = error?.Message });
                    await response.WriteAsync(result);
                }
            }
        }
    }
}
