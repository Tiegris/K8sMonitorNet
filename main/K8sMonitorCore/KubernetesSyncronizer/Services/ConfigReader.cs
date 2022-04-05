using k8s;
using k8s.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using static k8s.WatchEventType;

namespace KubernetesSyncronizer.Services;

public class ConfigReader : IDisposable
{
    private readonly IKubernetes client;
    private Watcher<V1Service>? watch;

    private readonly ResourceRegistry resourceRegistry;
    private readonly ILogger<ConfigReader> logger;

    public ConfigReader(IKubernetes k8s, ResourceRegistry resourceRegistry, ILogger<ConfigReader> logger) {
        client = k8s;
        this.resourceRegistry = resourceRegistry;
        this.logger = logger;
    }

    public void StartWatching() {
        var podlistResp = client.ListServiceForAllNamespacesWithHttpMessagesAsync(watch: true);
        watch = podlistResp.Watch<V1Service, V1ServiceList>(WatchEventHandler);
    }


    private void WatchEventHandler(WatchEventType type, V1Service item) {
        logger.LogInformation("Service: {name} {type}", item.Name(), type);
        switch (type) {
            case Added:
                resourceRegistry.Add(item);
                break;
            case Modified:
                resourceRegistry.Delete(item);
                resourceRegistry.Add(item);
                break;
            case Deleted:
                resourceRegistry.Delete(item);
                break;
            case Error:
                logger.LogError("Kubernetes watch error: {type} {item}", type, item.Name());
                break;
        }
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
                watch?.Dispose();
            }
            disposed = true;
        }
    }
    #endregion
}
