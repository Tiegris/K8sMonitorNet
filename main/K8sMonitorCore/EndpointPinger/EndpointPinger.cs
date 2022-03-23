using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Pinger
{
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
            this.logger = loggerFactory.CreateLogger<EndpointPinger>();
        }

        internal void StartAndForget() {
            _ = StartAsync();
        }

        private async Task StartAsync() {
            var ct = cancelObject.Token;
            while (!ct.IsCancellationRequested) {
                using CancellationTokenSource cancelCycle = new();
                cancelCycle.CancelAfter(endpoint.Period);
                try {
                    var delay = Task.Delay(endpoint.Period, ct);
                    var cycle = PingCycleAsync(cancelCycle.Token);
                    await Task.WhenAll(cycle, delay);
                }
                catch (TaskCanceledException) {
                    cancelCycle?.Cancel();
                }
            }
            logger.LogInformation("Stopped pinging {endpoint}", endpoint.Uri);
        }

        private async Task PingCycleAsync(CancellationToken cancelCycle) {
            logger.LogInformation("Pinging cycle started for {enbdpoint}", endpoint.Uri);

            using var client = hcf.CreateClient();
            client.Timeout = endpoint.Timeout;
            try {
                var response = await client.GetAsync(endpoint.Uri, cancelCycle);
                if (response.IsSuccessStatusCode)
                    endpoint.Success();
                else
                    endpoint.Fail();
            }
            catch (Exception ex) when (ex is TaskCanceledException || ex is HttpRequestException) {
                endpoint.Fail();
            }

            logger.LogInformation("Pinging cycle ended for {enbdpoint}", endpoint.Uri);
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
                    cancelObject?.Cancel();
                    cancelObject?.Dispose();
                }

                disposed = true;
            }
        }
        #endregion

    }
}
