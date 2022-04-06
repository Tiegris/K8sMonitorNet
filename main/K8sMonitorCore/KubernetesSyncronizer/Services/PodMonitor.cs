using k8s;
using k8s.Models;
using Microsoft.Extensions.Logging;
using System;
using static k8s.WatchEventType;

namespace KubernetesSyncronizer.Services;
internal class PodMonitor : IDisposable
{
    private readonly string serviceKey;
    private readonly string selector;
    private readonly ILogger<PodMonitor> logger;
    private readonly IKubernetes client;
    private readonly ResourceRegistry resourceRegistry;
    private Watcher<V1Pod>? watch;

    internal PodMonitor(IKubernetes k8s, ResourceRegistry resourceRegistry, string serviceKey, string selector, ILogger<PodMonitor> logger) {
        client = k8s;
        this.resourceRegistry = resourceRegistry;
        this.serviceKey = serviceKey;
        this.selector = selector;
        this.logger = logger;
    }

    internal void StartWatching() {
        watch?.Dispose();
        var podlistResp = client.ListPodForAllNamespacesWithHttpMessagesAsync(watch: true, labelSelector: selector);
        watch = podlistResp.Watch<V1Pod, V1PodList>(WatchEventHandler);
    }

    private void WatchEventHandler(WatchEventType type, V1Pod item) {
        try {
            logger.LogTrace("Pod: {name} {type}", item.Name(), type);
            switch (type) {
                case Added:
                    resourceRegistry.AddPod(serviceKey, item);
                    break;
                case Modified:
                    resourceRegistry.DeletePod(serviceKey, item);
                    resourceRegistry.AddPod(serviceKey, item);
                    break;
                case Deleted:
                    resourceRegistry.DeletePod(serviceKey, item);
                    break;
                case Error:
                    StartWatching();
                    logger.LogError("Kubernetes watch error: {type} {item}", type, item?.Name());
                    break;
            }
        } catch (Exception ex) {
            logger.LogError("Exception occured in pod watch callback: {message}", ex.Message);
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
