using k8s;
using K8sMonitorCore.Services;
using KubernetesSyncronizer;
using Microsoft.Extensions.DependencyInjection;

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

        public static IServiceCollection AddK8sListening(this IServiceCollection services) {
            services.AddSingleton<ResourceRegistry>();
            services.AddHostedService<AutodiscoveryHostedService>();
            return services;
        }
    }
}
