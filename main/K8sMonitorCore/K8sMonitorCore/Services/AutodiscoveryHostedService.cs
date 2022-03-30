using k8s;
using KubernetesSyncronizer;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace K8sMonitorCore.Services
{
    public class AutodiscoveryHostedService : IHostedService
    {
        private readonly K8sServiceConfigReader discovery;

        public AutodiscoveryHostedService(IKubernetes client, ResourceRegistry resourceRegistry, ILoggerFactory loggerFactory) {
            this.discovery = new K8sServiceConfigReader(
                client, resourceRegistry, loggerFactory.CreateLogger<K8sServiceConfigReader>());
        }

        public Task StartAsync(CancellationToken cancellationToken) {
            discovery.StartAndForget();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) {
            discovery?.Dispose();
            return Task.CompletedTask;
        }
    }
}
