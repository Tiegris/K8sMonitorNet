using k8s.Models;
using KubernetesSyncronizer.Settings;
using Pinger;
using System;
using System.Collections.Generic;
using static KubernetesSyncronizer.Util.Utils;

namespace KubernetesSyncronizer.Util
{
    public static class ExtractorExtensions
    {
        const string PORT = "mnet.uri.port";
        const string PATH = "mnet.uri.path";
        const string SCHEME = "mnet.uri.scheme";
        const string TIMEOUT = "mnet.timeout";
        const string PERIOD = "mnet.period";
        const string FT = "mnet.failureThreshold";
        const string HPA_ENABLED = "mnet.hpa.enabled";
        const string HPA_PERCENTAGE = "mnet.hpa.percentage";

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

        #region TryExtract
        private static ConfigurationErrorEntry? TryExtract(V1Service service, string key, bool tDefault, out bool value) {
            var dict = service.Metadata.Annotations;
            if (dict.TryGetValue(key, out string? strValue)) {
                if (bool.TryParse(strValue, out bool tValue)) {
                    value = tValue;
                    return null;
                } else {
                    value = tDefault;
                    return new ConfigurationErrorEntry(key, strValue, ConfigurationErrorType.ParseError);
                }
            } else {
                value = tDefault;
                return null;
            }
        }

        private static ConfigurationErrorEntry? TryExtract(V1Service service, string key, int tDefault, out int value) {
            var dict = service.Metadata.Annotations;
            if (dict.TryGetValue(key, out string? strValue)) {
                if (int.TryParse(strValue, out int tValue)) {
                    value = tValue;
                    return null;
                } else {
                    value = tDefault;
                    return new ConfigurationErrorEntry(key, strValue, ConfigurationErrorType.ParseError);
                }
            } else {
                value = tDefault;
                return null;
            }
        }
        #endregion

        public static string ExtractPodIp(this V1Pod it) {
            return it.Status.PodIP;
        }

        public static MonitoredService ExtractMonitoredService(this V1Service it, Defaults defaults) {
            var dict = it.Metadata.Annotations;
            var errors = new ServiceConfigurationError();

            if (!dict.TryGetValue(PATH, out string? path)) {
                errors.Add(new ConfigurationErrorEntry(
                                    PATH,
                                    "",
                                    ConfigurationErrorType.RequiredValueNotFound,
                                    "You must define a Path!"
                                ));
                return new MonitoredService(it.ExtractFullName(), errors);
            }

            errors.AddIfNotNull(TryExtract(it, PORT, defaults.Port, out int port));
            if (port <= 0 && port > 65535)
                errors.Add(new ConfigurationErrorEntry(
                    PORT, 
                    port.ToString(), 
                    ConfigurationErrorType.OutOfRangeError, 
                    "Port must be between 0 and 65535"
                ));

            dict.TryGetValue(SCHEME, out string? scheme);
            scheme ??= defaults.Scheme;
            scheme = scheme.Replace("://", "");
            if (scheme is not ("http" or "https"))
                errors.Add(new ConfigurationErrorEntry(
                    SCHEME,
                    scheme,
                    ConfigurationErrorType.OutOfRangeError,
                    "Scheme must be either http or https"
                ));

            string ns = it.Namespace();
            string srv = it.Name();

            errors.AddIfNotNull(TryExtract(it, TIMEOUT, defaults.Timeout, out int timeout));
            errors.AddIfNotNull(TryExtract(it, PERIOD, defaults.Period, out int period));
            errors.AddIfNotNull(TryExtract(it, FT, defaults.FailureThreshold, out int failureThreshold));
            errors.AddIfNotNull(TryExtract(it, HPA_ENABLED, defaults.Hpa.Enabled, out bool hpaEnabled));
            errors.AddIfNotNull(TryExtract(it, HPA_PERCENTAGE, defaults.Hpa.Percentage, out int hpaPercentage));

            return new(it.ExtractFullName(), errors) {
                Timeout = new TimeSpan(0, 0, timeout),
                Period = new TimeSpan(0, 0, period),
                FailureThreshold = failureThreshold,
                Uri = BuildFqdnUri(scheme, ns, srv, port, path),
                Hpa = new Hpa {
                    Enabled = hpaEnabled,
                    Percentage = hpaPercentage
                }
            };
        }

    }
}
