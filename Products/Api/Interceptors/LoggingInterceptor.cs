using Grpc.Core;
using Grpc.Core.Interceptors;
using ApplicationException = Application.Exceptions.ApplicationException;

namespace Api.Interceptors;

public class LoggingInterceptor : Interceptor
{
    private readonly ILogger<LoggingInterceptor> _logger;

    public LoggingInterceptor(ILogger<LoggingInterceptor> logger)
    {
        _logger = logger;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request, ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        _logger.LogInformation($"\n{typeof(TRequest)}: {request}.");
        try
        {
            var response = await continuation(request, context);
            _logger.LogInformation($"\n{typeof(TResponse)}: {response}.");
            return response;
        }
        catch (ApplicationException exception)
        {
            _logger.LogError($"\n{typeof(TResponse)}: {exception.Message}");
            throw;
        }
    }
}