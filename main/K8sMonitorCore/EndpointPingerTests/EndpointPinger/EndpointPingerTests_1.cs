using EndpointPinger;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Net.Http;

namespace EndpointPingerTests.EndpointPinger;

class EndpointPingerTests_1
{
    Endpoint e;
    IHttpClientFactory hcf;
    ILoggerFactory loggerFactory;


    public void Init() {
        e = new Endpoint {
            Period = new TimeSpan(),
            Timeout = new TimeSpan(),
            FailureThreshold = 5,
            Uri = new Uri(""),
        };
        hcf = new HttpClientFactoryStub();
        loggerFactory = new LoggerFactoryStub();
    }


    [Test]
    public void Test1() {
        var ep = new Pinger.EndpointPinger(e, hcf, loggerFactory);


    }

}
