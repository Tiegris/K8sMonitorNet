using k8s;
using K8sMonitorCore.Services;
using KubernetesSyncronizer.Services;
using Microsoft.Extensions.DependencyInjection;

namespace K8sMonitorCore;

public static class StartupExtensions
{
    public static IServiceCollection AddK8sClient(this IServiceCollection services) {
        KubernetesClientConfiguration config = KubernetesClientConfiguration.IsInCluster() ?
             KubernetesClientConfiguration.InClusterConfig() :
             KubernetesClientConfiguration.BuildConfigFromConfigFile();

        IKubernetes client = new Kubernetes(config);
        services.AddSingleton<IKubernetes>(client);
        return services;
    }

    public static IServiceCollection AddK8sListening(this IServiceCollection services) {
        services.AddSingleton<ResourceRegistry>();
        services.AddSingleton<AutodiscoveryHostedService>();
        services.AddHostedService(provider => provider.GetService<AutodiscoveryHostedService>()!);
        return services;
    }
}
