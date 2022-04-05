using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

namespace EndpointPingerTests;

class HttpClientFactoryStub : IHttpClientFactory
{
    public HttpClient CreateClient(string name) {
        return new HttpClient();
    }
}


class LoggerFactoryStub : ILoggerFactory
{
    public void AddProvider(ILoggerProvider provider) {
        throw new NotImplementedException();
    }

    public ILogger CreateLogger(string categoryName) {
        throw new NotImplementedException();
    }

    public void Dispose() {
        throw new NotImplementedException();
    }
}

class LoggerStub : ILogger
{
    public IDisposable BeginScope<TState>(TState state) {
        throw new NotImplementedException();
    }

    public bool IsEnabled(LogLevel logLevel) {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) {
        return;
    }
}

