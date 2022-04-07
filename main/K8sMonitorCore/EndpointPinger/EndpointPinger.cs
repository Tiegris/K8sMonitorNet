using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace EndpointPinger;

internal class EndpointPinger : IDisposable
{
    internal Endpoint Endpoint => endpoint;

    private readonly CancellationTokenSource cancelObject = new();
    private readonly IHttpClientFactory hcf;
    private readonly Endpoint endpoint;
    private readonly ILogger logger;

    internal EndpointPinger(Endpoint endpoint, IHttpClientFactory hcf, ILoggerFactory loggerFactory) {
        this.endpoint = endpoint;
        this.hcf = hcf;
        logger = loggerFactory.CreateLogger<EndpointPinger>();
    }

    private bool started = false;
    internal void StartAndForget() {
        if (!started) {
            _ = StartAsync();
            started = true;
        }
    }

    private async Task StartAsync() {
        logger.LogInformation("Started pinging {endpoint}", endpoint.Uri);
        var ct = cancelObject.Token;
        while (!ct.IsCancellationRequested) {
            using CancellationTokenSource cancelCycle = new();
            cancelCycle.CancelAfter(endpoint.Period);
            try {
                var delay = Task.Delay(endpoint.Period, ct);
                var cycle = PingCycleAsync(cancelCycle.Token);
                await Task.WhenAll(cycle, delay);
            } catch (TaskCanceledException) {
                cancelCycle?.Cancel();
            }
        }
        logger.LogInformation("Stopped pinging {endpoint}", endpoint.Uri);
    }

    private async Task PingCycleAsync(CancellationToken cancelCycle) {
        logger.LogTrace("Pinging cycle started for {endpoint}", endpoint.Uri);

        using var client = hcf.CreateClient();
        client.Timeout = endpoint.Timeout;
        try {
            var response = await client.GetAsync(endpoint.Uri, cancelCycle);
            if (response.IsSuccessStatusCode)
                endpoint.Success();
            else
                endpoint.Fail($"StatusCode: {(int)response.StatusCode}, ReasonPhrase: {response.ReasonPhrase}");
        } catch (Exception ex) when (ex is TaskCanceledException or HttpRequestException) {
            endpoint.Fail(ex.Message);
        }

        logger.LogTrace("Pinging cycle ended for {endpoint}", endpoint.Uri);
    }

    #region Dispose
    private bool disposed;
    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing) {
        if (!disposed) {
            if (disposing) {
                cancelObject.Cancel();
                cancelObject.Dispose();
            }

            disposed = true;
        }
    }
    #endregion

}
