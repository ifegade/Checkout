using System.Net;
using Checkout.PaymentGateway.Dto;
using Checkout.PaymentGateway.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;

namespace Checkout.PaymentGateway.Middleware;

public static class ExceptionMiddleware
{
    /***
     * Global exception handler to structured format
     */
    public static void ConfigureExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
                    Log.Error("Status Code : {@statusCode} \t Error Details: {@details}",
                        context.Response.StatusCode,
                        contextFeature.Error);
                    var result = new ResponseDto<string>
                    {
                        response_code = ResponseCode.Unsuccessful,
                        response_message = MessageStrings.TryAgain
                    };
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(result));
                }
            });
        });
    }
}