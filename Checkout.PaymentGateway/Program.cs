using System.Collections.Generic;
using System.IO;
using System.Linq;
using Checkout.PaymentGateway.Middleware;
using Checkout.PaymentGateway.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using System.Reflection;
using System.Text.Json.Serialization;
using Checkout.PaymentGateway.Dto;
using Checkout.PaymentGateway.Infrastructure;
using Checkout.PaymentGateway.Simulator;
using Checkout.PaymentGateway.Validator;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.HttpLogging;

string AppName = "PaymentGateway";

var Configuration = GetConfiguration();

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.AspNetCore.HttpLogging.HttpLoggingMiddleware", LogEventLevel.Information)
    .Enrich.WithProperty("ApplicationContext", AppName)
    .Enrich.WithEnvironmentName()
    .Enrich.WithProperty("Service", "Customer")
    .Enrich.WithMachineName()
    .Enrich.WithExceptionDetails()
    .WriteTo.Console()
    //.WriteTo.Seq(Configuration[AppConstants.SeqUrl])
    .CreateLogger();

Log.Information("Starting web application");
builder.Services.AddSingleton<Serilog.ILogger>(Log.Logger);
builder.Host.UseSerilog();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition
            = JsonIgnoreCondition.WhenWritingNull; //to remove null value from the response body.
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState.Values.Where(v => v.Errors.Count > 0)
                .SelectMany(v => v.Errors)
                .Select(v => v.ErrorMessage)
                .ToList();
            
            var response =
                new ResponseDto<string>(ResponseCode.Unsuccessful, string.Join('\n', errors));

            return new JsonResult(response)
            {
                StatusCode = 200
            };
        };
    })
    .AddFluentValidation(config => { config.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly()); });

builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.ResponseStatusCode | HttpLoggingFields.RequestQuery |
                            HttpLoggingFields.RequestBody | HttpLoggingFields.RequestPath |
                            HttpLoggingFields.ResponseBody;
    logging.MediaTypeOptions.AddText("application/javascript");
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IPaymentService, PaymentService>();
builder.Services.AddSingleton<CKOSimulator>();
builder.Services.AddSingleton<IMessagingService, MessagingService>();
builder.Services.AddSingleton<IMerchantService, MerchantService>();

var merchantIps = new List<IpDto>();
Configuration.GetSection(AppConstants.MerchantIPAddress).Bind(merchantIps);

builder.Services.AddScoped(_ => new IPValidator(merchantIps));

var app = builder.Build();

app.ConfigureExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


IConfiguration GetConfiguration()
{
    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile($"appSettings.{Utility.GetEnvironment()}.json", false, true)
        .AddJsonFile("appSettings.json", false, true)
        .AddEnvironmentVariables();

    return builder.Build();
}