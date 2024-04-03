using Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace IntegrationTests.GrpcEndpoint;

public class GrpcTestFixture<TStartup> : IDisposable where TStartup : class
{
    private TestServer? _server;
    private IHost? _host;
    private HttpMessageHandler? _handler;
    private Action<IWebHostBuilder>? _configureWebHost;
    
    public GrpcTestFixture()
    {
    }

    public Mock<IProductRepository> ProductRepositoryFake { get; } = new();

    public void ConfigureWebHost(Action<IWebHostBuilder> configure)
    {
        _configureWebHost = configure;
    }

    private void EnsureServer()
    {
        if (_host == null)
        {
            var builder = new HostBuilder()
                .ConfigureWebHostDefaults(webHost =>
                {
                    webHost
                        .UseTestServer()
                        .UseStartup<TStartup>();

                    _configureWebHost?.Invoke(webHost);
                })
                .ConfigureServices(services =>
                {
                    services.Replace(new ServiceDescriptor(typeof(IProductRepository), ProductRepositoryFake.Object));
                });
            _host = builder.Start();
            _server = _host.GetTestServer();
            _handler = _server.CreateHandler();
        }
    }
    
    public HttpMessageHandler Handler
    {
        get
        {
            EnsureServer();
            return _handler!;
        }
    }

    public void Dispose()
    {
        _handler?.Dispose();
        _host?.Dispose();
        _server?.Dispose();
    }

    public IDisposable GetTestContext(ITestOutputHelper outputHelper)
    {
        return new GrpcTestContext<TStartup>(this, outputHelper);
    }
}