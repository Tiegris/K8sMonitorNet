using k8s;
using KubernetesSyncronizer;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace K8sMonitorCore.Services
{
    public class AutodiscoveryHostedService : IHostedService
    {
        K8sServiceConfigReader? discovery;
        //IKubernetes client;


        public Task StartAsync(CancellationToken cancellationToken) {
            //discovery = new K8sServiceConfigReader(
            //    );
            //discovery.StartAndForget();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) {
            discovery?.Dispose();
            return Task.CompletedTask;
        }
    }
}
