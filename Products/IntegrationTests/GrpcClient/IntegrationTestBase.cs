using Api;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace IntegrationTests.GrpcClient;

public class IntegrationTestBase : IClassFixture<GrpcTestFixture<IntegrationTestsHelper>>, IDisposable
{
    private GrpcChannel? _channel;
    private IDisposable? _testContext;

    protected GrpcTestFixture<IntegrationTestsHelper> Fixture { get; set; }

    protected ILoggerFactory LoggerFactory => Fixture.LoggerFactory;

    protected GrpcChannel Channel => _channel ??= CreateChannel();

    protected GrpcChannel CreateChannel()
    {
        return GrpcChannel.ForAddress("http://localhost", new GrpcChannelOptions
        {
            LoggerFactory = LoggerFactory,
            HttpHandler = Fixture.Handler
        });
    }

    public IntegrationTestBase(GrpcTestFixture<IntegrationTestsHelper> fixture, ITestOutputHelper outputHelper)
    {
        Fixture = fixture;
        _testContext = Fixture.GetTestContext(outputHelper);
    }

    public void Dispose()
    {
        _testContext?.Dispose();
        _channel = null;
    }
}
