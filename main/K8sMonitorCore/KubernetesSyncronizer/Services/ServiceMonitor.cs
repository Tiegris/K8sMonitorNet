using k8s;
using k8s.Models;
using Microsoft.Extensions.Logging;
using System;
using static k8s.WatchEventType;

namespace KubernetesSyncronizer.Services;

public class ServiceMonitor : IDisposable
{
    private readonly IKubernetes client;
    private Watcher<V1Service>? watch;

    private readonly ResourceRegistry resourceRegistry;
    private readonly ILogger<ServiceMonitor> logger;

    public ServiceMonitor(IKubernetes k8s, ResourceRegistry resourceRegistry, ILogger<ServiceMonitor> logger) {
        client = k8s;
        this.resourceRegistry = resourceRegistry;
        this.logger = logger;
    }

    public void StartWatching() {
        watch?.Dispose();
        var podlistResp = client.ListServiceForAllNamespacesWithHttpMessagesAsync(watch: true);
        watch = podlistResp.Watch<V1Service, V1ServiceList>(WatchEventHandler);
    }


    private void WatchEventHandler(WatchEventType type, V1Service item) {
        try {
            logger.LogTrace("Service: {name} {type}", item.Name(), type);
            switch (type) {
                case Added:
                    resourceRegistry.AddService(item);
                    break;
                case Modified:
                    resourceRegistry.DeleteService(item);
                    resourceRegistry.AddService(item);
                    break;
                case Deleted:
                    resourceRegistry.DeleteService(item);
                    break;
                case Error:
                    logger.LogError("Kubernetes watch error: {type} {item}", type, item?.Name());
                    StartWatching();
                    break;
            }
        } catch (Exception ex) {
            logger.LogError("Exception occured in service watch callback: {message}", ex.Message);
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
