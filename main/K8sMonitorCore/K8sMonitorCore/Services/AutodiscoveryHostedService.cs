using k8s;
using KubernetesSyncronizer;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace K8sMonitorCore.Services
{
    public class AutodiscoveryHostedService : IHostedService
    {
        private readonly K8sServiceConfigReader discovery;

        public AutodiscoveryHostedService(IKubernetes client, ResourceRegistry resourceRegistry) {
            this.discovery = new K8sServiceConfigReader(
                client, resourceRegistry);
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
