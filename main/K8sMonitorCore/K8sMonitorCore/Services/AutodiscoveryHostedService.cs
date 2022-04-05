using k8s;
using KubernetesSyncronizer.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace K8sMonitorCore.Services;

public class AutodiscoveryHostedService : IHostedService
{
    private readonly ServiceMonitor discovery;

    public AutodiscoveryHostedService(IKubernetes client, ResourceRegistry resourceRegistry, ILoggerFactory loggerFactory) {
        this.discovery = new ServiceMonitor(
            client, resourceRegistry, loggerFactory.CreateLogger<ServiceMonitor>());
    }

    public Task StartAsync(CancellationToken cancellationToken) {
        discovery.StartWatching();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        discovery?.Dispose();
        return Task.CompletedTask;
    }
}
