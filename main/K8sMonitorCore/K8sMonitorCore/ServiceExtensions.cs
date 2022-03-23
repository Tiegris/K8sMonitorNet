using k8s;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace K8sMonitorCore
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddK8sClient(this IServiceCollection services) {
            KubernetesClientConfiguration config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            IKubernetes client = new Kubernetes(config);
            services.AddSingleton<IKubernetes>(client);
            return services;
        }
    }
}
