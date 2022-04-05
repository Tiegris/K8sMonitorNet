using k8s.Models;
using KubernetesSyncronizer.Data;
using System;
using static KubernetesSyncronizer.Util.ExtractorHelper;

namespace KubernetesSyncronizer.Util;
internal static class ExtractorHelper {
    #region TryExtract
    internal static ConfigurationErrorEntry? TryExtract(V1Service service, string key, bool tDefault, out bool value) {
        var dict = service.Metadata.Annotations;
        if (dict.TryGetValue(key, out string? strValue)) {
            if (bool.TryParse(strValue, out bool tValue)) {
                value = tValue;
                return null;
            } else {
                value = tDefault;
                return new ConfigurationErrorEntry(key, strValue,
                    ConfigurationErrorType.ParseError,
                    $"Could not parse the value of {key} as boolean.");
            }
        } else {
            value = tDefault;
            return null;
        }
    }

    internal static ConfigurationErrorEntry? TryExtract(V1Service service, string key, int tDefault, out int value) {
        var dict = service.Metadata.Annotations;
        if (dict.TryGetValue(key, out string? strValue)) {
            if (int.TryParse(strValue, out int tValue)) {
                value = tValue;
                return null;
            } else {
                value = tDefault;
                return new ConfigurationErrorEntry(key, strValue,
                    ConfigurationErrorType.ParseError,
                    $"Could not parse the value of {key} as integer.");
            }
        } else {
            value = tDefault;
            return null;
        }
    }
    #endregion

    internal static Uri BuildFqdnUri(string scheme, string ns, string srv, int port, string path) {
        path = path.TrimStart('/');
        scheme = scheme.Replace("://", "");

        return new Uri($"{scheme}://{srv}.{ns}.svc.cluster.local:{port}/{path}");
    }

}
