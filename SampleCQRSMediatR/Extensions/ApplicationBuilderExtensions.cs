using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace SampleCQRSMediatR.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseFluentValidationExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(config => {
                config.Run(async context => {
                    var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
                    var exception = errorFeature.Error;

                    if (!(exception is FluentValidation.ValidationException validationException))
                    {
                        // Throw an exception if it is not FluentValidation.ValidationException
                        throw exception;
                    }

                    var errors = validationException.Errors.Select(err => new
                    {
                        err.PropertyName,
                        err.ErrorMessage
                    });

                    var errorText = System.Text.Json.JsonSerializer.Serialize(errors);

                    context.Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                    context.Response.ContentType = System.Net.Mime.MediaTypeNames.Application.Json;
                    await context.Response.WriteAsync(errorText, System.Text.Encoding.UTF8);
                });
            });
        }
    }
}
