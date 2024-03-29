using Grpc.Core;
using Api;
using Api.GrpcServices;
using Xunit.Abstractions;

namespace IntegrationTests.GrpcClient;

public class ProductServiceTests : IntegrationTestBase
{
    public ProductServiceTests(GrpcTestFixture<IntegrationTestsHelper> fixture, ITestOutputHelper outputHelper)
        : base(fixture, outputHelper)
    {
    }

    [Fact]
    public async Task SayHelloUnaryTest()
    {
        // Arrange
        var client = new Api.ProductService.ProductServiceClient(Channel);

        // Act
        // var response = await client.SayHelloUnaryAsync(new HelloRequest { Name = "Joe" });
        //
        // // Assert
        // Assert.Equal("Hello Joe", response.Message);
    }

    [Fact]
    public async Task SayHelloClientStreamingTest()
    {
        // Arrange
        var client = new ProductService.ProductServiceClient(Channel);

        var names = new[] { "James", "Jo", "Lee" };
        HelloReply response;

        // Act
        using var call = client.SayHelloClientStreaming();
        foreach (var name in names)
        {
            await call.RequestStream.WriteAsync(new HelloRequest { Name = name });
        }
        await call.RequestStream.CompleteAsync();

        response = await call;

        // Assert
        Assert.Equal("Hello James, Jo, Lee", response.Message);
    }

    [Fact]
    public async Task SayHelloServerStreamingTest()
    {
        // Arrange
        var client = new ProductService.ProductServiceClient(Channel);

        var cts = new CancellationTokenSource();
        var hasMessages = false;
        var callCancelled = false;

        // Act
        using var call = client.SayHelloServerStreaming(new HelloRequest { Name = "Joe" }, cancellationToken: cts.Token);
        try
        {
            await foreach (var message in call.ResponseStream.ReadAllAsync())
            {
                hasMessages = true;
                cts.Cancel();
            }
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
        {
            callCancelled = true;
        }

        // Assert
        Assert.True(hasMessages);
        Assert.True(callCancelled);
    }

    [Fact]
    public async Task SayHelloBidirectionStreamingTest()
    {
        // Arrange
        var client = new ProductService.ProductServiceClient(Channel);

        var names = new[] { "James", "Jo", "Lee" };
        var messages = new List<string>();

        // Act
        using var call = client.SayHelloBidirectionalStreaming();
        foreach (var name in names)
        {
            await call.RequestStream.WriteAsync(new HelloRequest { Name = name });

            Assert.True(await call.ResponseStream.MoveNext());
            messages.Add(call.ResponseStream.Current.Message);
        }

        await call.RequestStream.CompleteAsync();

        // Assert
        Assert.Equal(3, messages.Count);
        Assert.Equal("Hello James", messages[0]);
    }
}