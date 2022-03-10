using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace EndpointPinger
{
    public class EndpointScheduler
    {
        private readonly CancellationTokenSource cts = new();
        private readonly IHttpClientFactory hcf;
        private readonly Endpoint endpoint;

        public EndpointScheduler(Endpoint endpoint, IHttpClientFactory hcf) {
            this.endpoint = endpoint;
            this.hcf = hcf;
        }

        public void Stop() {
            cts.Cancel();
        }

        public async Task RunAsync() {
            while (cts.IsCancellationRequested) {
                Console.WriteLine("pinging");

                var client = hcf.CreateClient();
                try {
                    cts.CancelAfter(endpoint.Timeout);
                    var response = await client.GetAsync(endpoint.Uri, cts.Token);
                    if (response.IsSuccessStatusCode)
                        endpoint.Success();
                    else
                        endpoint.Fail();
                }
                catch (TaskCanceledException tce) {

                }
                catch (HttpRequestException hre) {
                    endpoint.Fail();
                }


                Console.WriteLine("pinged");
                await Task.Delay(endpoint.Period);
            }
        }
    }
}
