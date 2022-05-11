using EndpointPinger;
using k8s.Models;
using KubernetesSyncronizer.Services;
using System;
using System.Diagnostics.CodeAnalysis;
using static KubernetesSyncronizer.Util.ExtractorExtensions;


namespace KubernetesSyncronizer.Data;

public class MonitoredService : IDisposable
{
    internal PodMonitor? PodMonitor { get; init; }

    public MonitoredService(K8sKey key, ServiceConfigurationError errors) {
        Key = key;
        Errors = errors;
    }

    public K8sKey Key { get; init; }
    public int FailureThreshold { get; init; }
    public TimeSpan Timeout { get; init; }
    public TimeSpan Period { get; init; }
    public Uri? Uri { get; init; }
    public Hpa? Hpa { get; init; }
    public ServiceConfigurationError Errors { get; init; }

    public K8sKey GetPodFullName(V1Pod pod) => new(Key.Ns, Key.Svc) { Pod = pod.Name() };

    public bool TryGetEndpointForPod(V1Pod pod, [MaybeNullWhen(false)] out K8sKey key, [MaybeNullWhen(false)] out Endpoint endpoint) {
        if (Errors.HasErrors || Uri is null || pod is { Status.PodIP: null }) {
            key = null;
            endpoint = null;
            return false;
        }

        Uri podUri = Uri.ExtendUriWithPodIp(pod.ExtractPodIp());
        key = GetPodFullName(pod);
        endpoint = new Endpoint(FailureThreshold, Timeout, Period, podUri);
        return true;
    }

    public bool TryGetEndpoint([MaybeNullWhen(false)] out K8sKey key, [MaybeNullWhen(false)] out Endpoint endpoint) {
        if (Errors.HasErrors || Uri is null || Hpa is { Enabled: true }) {
            key = null;
            endpoint = null;
            return false;
        }

        key = Key;
        endpoint = new Endpoint(FailureThreshold, Timeout, Period, Uri);
        return true;
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
                PodMonitor?.Dispose();
            }
            disposed = true;
        }
    }
    #endregion

}
