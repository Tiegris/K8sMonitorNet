using k8s;
using k8s.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using static k8s.WatchEventType;

namespace KubernetesSyncronizer.Services;
internal class PodMonitor : IDisposable {
    private readonly string serviceKey;
    private readonly IKubernetes client;
    private readonly ResourceRegistry resourceRegistry;
    private Watcher<V1Pod>? watch;

    internal PodMonitor(IKubernetes k8s, ResourceRegistry resourceRegistry, string serviceKey) {
        client = k8s;
        this.resourceRegistry = resourceRegistry;
        this.serviceKey = serviceKey;
    }

    internal void StartWatching(string selector) {
        var podlistResp = client.ListPodForAllNamespacesWithHttpMessagesAsync(watch: true, labelSelector: selector);
        watch = podlistResp.Watch<V1Pod, V1PodList>(WatchEventHandler);
    }

    private void WatchEventHandler(WatchEventType type, V1Pod item) {
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
