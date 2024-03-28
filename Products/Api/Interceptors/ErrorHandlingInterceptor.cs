using Grpc.Core;
using Grpc.Core.Interceptors;
using System;
using Domain.Exceptions;
using ApplicationException = Application.Exceptions.ApplicationException;

namespace Api.Interceptors;

public class ErrorHandlingInterceptor : Interceptor 
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request, ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (ProductAlreadyExistsException exception)
        {
            throw new RpcException(new Status(StatusCode.Aborted, exception.Message));
        }
        catch (ProductNotFoundException exception)
        {
            throw new RpcException(new Status(StatusCode.NotFound, exception.Message));
        }
    }
}