using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Pinger
{
    public class EndpointPinger
    {
        public Endpoint Endpoint => endpoint;


        private readonly CancellationTokenSource cancelObject = new();
        private readonly IHttpClientFactory hcf;
        private readonly Endpoint endpoint;
        private readonly ILogger logger;

        public EndpointPinger(Endpoint endpoint, IHttpClientFactory hcf, ILoggerFactory loggerFactory) {
            this.endpoint = endpoint;
            this.hcf = hcf;
            this.logger = loggerFactory.CreateLogger<EndpointPinger>();
        }

        public void Cancel() {
            cancelObject.Cancel();
        }

        public void StartAndForget() {
            _ = StartAsync();
        }

        private async Task StartAsync() {
            var ct = cancelObject.Token;
            while (!ct.IsCancellationRequested) {
                CancellationTokenSource cancelCycle = new();
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

        private async Task PingCycleAsync(CancellationToken ct) {
            logger.LogInformation("Pinging cycle started for {enbdpoint}", endpoint.Uri);

            var client = hcf.CreateClient();
            client.Timeout = endpoint.Timeout;
            try {
                var response = await client.GetAsync(endpoint.Uri, ct);
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

        ~EndpointPinger() {
            Cancel();
        }

    }
}
