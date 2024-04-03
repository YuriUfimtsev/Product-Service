using Api.GrpcServices;
using Api.Interceptors;
using Application;
using FluentValidation;

namespace Api;

public class Startup
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ConfigureServices(builder.Services);
        var app = builder.Build();
        Configure(app, builder.Environment);
        app.Run();
    }

    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddGrpc(options =>
        {
            options.Interceptors.Add<ValidationInterceptor>();
            options.Interceptors.Add<ErrorHandlingInterceptor>();
            options.Interceptors.Add<LoggingInterceptor>();
        }).AddJsonTranscoding();

        services.AddGrpcSwagger();
        services.AddSwaggerGen();
        services.AddValidatorsFromAssemblyContaining(typeof(ValidationInterceptor));
        services.AddApplicationServices();
    }

    public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGrpcService<ProductServiceGrpc>();
        });
    }
}