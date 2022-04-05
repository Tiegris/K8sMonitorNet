using k8s.Models;
using KubernetesSyncronizer.Services;
using Pinger;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using static KubernetesSyncronizer.Util.ExtractorExtensions;


namespace KubernetesSyncronizer.Data;

public class MonitoredService : IDisposable
{
    internal PodMonitor? PodMonitor { get; init; }

    public MonitoredService(string name, ServiceConfigurationError errors) {
        Name = name;
        Errors = errors;
    }

    public string Name { get; init; }
    public int FailureThreshold { get; init; }
    public TimeSpan Timeout { get; init; }
    public TimeSpan Period { get; init; }
    public Uri? Uri { get; init; }
    public Hpa? Hpa { get; init; }
    public ServiceConfigurationError Errors { get; init; }

    public string GetPodFullName(V1Pod pod) => Name + "::" + pod.Name();

    public bool TryGetEndpointForPod(V1Pod pod, [MaybeNullWhen(false)] out string name, [MaybeNullWhen(false)] out Endpoint endpoint) {
        if (Errors.HasErrors || Uri is null) {
            name = null;
            endpoint = null;
            return false;
        }

        Uri podUri = Uri.ExtendUriWithPodIp(pod.ExtractPodIp());
        name = GetPodFullName(pod);
        endpoint = new Endpoint(FailureThreshold, Timeout, Period, podUri);
        return true;
    }

    public bool TeyGetEndpoint([MaybeNullWhen(false)] out string name, [MaybeNullWhen(false)] out Endpoint endpoint) {
        if (Errors.HasErrors || Uri is null || Hpa is { Enabled: false}) {
            name = null;
            endpoint = null;
            return false;
        }            

        name = Name;
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
