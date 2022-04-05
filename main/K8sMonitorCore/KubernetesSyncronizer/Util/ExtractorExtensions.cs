using k8s.Models;
using KubernetesSyncronizer.Data;
using System;
using System.Collections.Generic;



namespace KubernetesSyncronizer.Util;

public static class ExtractorExtensions
{
    public static string? ExtractLabelString(this V1Service it) {
        if (it.Spec?.Selector is null)
            return null;

        var labels = new List<string>();
        foreach (var key in it.Spec.Selector)
            labels.Add(key.Key + "=" + key.Value);

        var labelStr = string.Join(",", labels.ToArray());
        return labelStr;
    }

    public static string ExtractFullName(this V1Service it) {
        return $"{it.Namespace()}::{it.Name()}";
    }

    public static string ExtractPodIp(this V1Pod it) {
        return it.Status.PodIP;
    }

    internal static Uri ExtendUriWithPodIp(this Uri serviceUri, string podIp) {
        podIp = podIp.Replace('.', '-');
        return new Uri($"{serviceUri.Scheme}://{podIp}.{serviceUri.Host}:{serviceUri.Port}/{serviceUri.AbsolutePath}");
    }

}
