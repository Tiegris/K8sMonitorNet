using k8s.Models;
using KubernetesSyncronizer.Settings;
using KubernetesSyncronizer.Util;
using Microsoft.Extensions.Options;
using Pinger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static KubernetesSyncronizer.Util.Utils;

namespace KubernetesSyncronizer.Util
{
    public static class ExtractorExtensions
    {
        const string PORT = "mnet.port";
        const string PATH = "mnet.path";
        const string TIMEOUT = "mnet.timeout";
        const string PERIOD = "mnet.period";
        const string FT = "mnet.failureThreshold";
        const string HPA_ENABLED = "mnet.hpa.enabled";
        const string HPA_PERCENTAGE = "mnet.hpa.percentage";

        public static string? ExtractLabelString(this V1Service it) {
            if (it.Spec == null || it.Spec.Selector == null)
                return null;

            var labels = new List<string>();
            foreach (var key in it.Spec.Selector)
                labels.Add(key.Key + "=" + key.Value);

            var labelStr = string.Join(",", labels.ToArray());
            return labelStr;
        }

        public static string ExtractName(this V1Service it) {
            return $"{it.Namespace()}::{it.Name()}";
        }

        public static bool ExtractHpaEnabled(this V1Service it, Defaults defaults) {
            var dict = it.Metadata.Annotations;
            return dict.TryGetValue(HPA_ENABLED, out string? strHpaEnabled) ? 
                bool.Parse(strHpaEnabled) : 
                defaults.Hpa.Enabled;
        }
        public static int ExtractHpaPercentage(this V1Service it, Defaults defaults) {
            var dict = it.Metadata.Annotations;
            return dict.TryGetValue(HPA_PERCENTAGE, out string? strHpaEnabled) ?
                int.Parse(strHpaEnabled) :
                defaults.Hpa.Percentage;
        }

        public static string ExtractPodIp(this V1Pod it) {
            return it.Status.PodIP;
        }

        public static Endpoint? ExtractEndpoint(this V1Service it, Defaults defaults) {
            var dict = it.Metadata.Annotations;
            if (!dict.TryGetValue(PATH, out string? path))
                return null;

            int port = defaults.Port;
            if (dict.TryGetValue(PORT, out string? strPort))
                port = int.Parse(strPort);

            if (port <= 0 && port > 65535)
                throw new K8sFqdnException("Port must be between 0 and 65535");

            string ns = it.Namespace();
            string srv = it.Name();

            int timeout = defaults.Timeout;
            if (dict.TryGetValue(TIMEOUT, out string? strTimeout))
                timeout = int.Parse(strTimeout);

            int period = defaults.Period;
            if (dict.TryGetValue(PERIOD, out string? strPeriod))
                period = int.Parse(strPeriod);

            int failureThreshold = defaults.FailureThreshold;
            if (dict.TryGetValue(FT, out string? strFailureThreshold))
                failureThreshold = int.Parse(strFailureThreshold);

            return new Endpoint(
                    timeout: new TimeSpan(0, 0, timeout),
                    period: new TimeSpan(0, 0, period),
                    failureThreshold: failureThreshold,
                    uri: BuildFqdnUri(ns, srv, port, path)
                );
        }

    }
}
