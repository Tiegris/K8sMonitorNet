using k8s.Models;
using KubernetesSyncronizer.Data;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KubernetesSyncronizer.Util.ExtractorHelper;

namespace KubernetesSyncronizer.Util;

internal class Extractor
{
    const string PORT = "mnet.uri.port";
    const string PATH = "mnet.uri.path";
    const string SCHEME = "mnet.uri.scheme";
    const string TIMEOUT = "mnet.timeout";
    const string PERIOD = "mnet.period";
    const string FT = "mnet.failureThreshold";
    const string HPA_ENABLED = "mnet.hpa.enabled";
    const string HPA_PERCENTAGE = "mnet.hpa.percentage";

    private readonly Defaults defaults;

    public Extractor(IOptions<Defaults> options) {
        defaults = options.Value;
    }

    public MonitoredService? TryExtractMonitoredService(V1Service it) {
        var dict = it.Metadata.Annotations;
        var errors = new ServiceConfigurationError();

        if (dict is null || !dict.TryGetValue(PATH, out string? path))
            return null;

        errors.AddIfNotNull(TryExtract(it, PORT, defaults.Port, out int port));
        if (port is <= 0 or > 65535)
            errors.Add(new ConfigurationErrorEntry(
                PORT,
                port.ToString(),
                ConfigurationErrorType.OutOfRangeError,
                "Port must be between 0 and 65535."
            ));

        dict.TryGetValue(SCHEME, out string? scheme);
        scheme ??= defaults.Scheme;
        if (scheme is not ("http" or "https"))
            errors.Add(new ConfigurationErrorEntry(
                SCHEME,
                scheme,
                ConfigurationErrorType.OutOfRangeError,
                "Scheme must be either http or https."
            ));

        string ns = it.Namespace();
        string srv = it.Name();

        errors.AddIfNotNull(TryExtract(it, TIMEOUT, defaults.Timeout, out int timeout));
        errors.AddIfNotNull(TryExtract(it, PERIOD, defaults.Period, out int period));
        errors.AddIfNotNull(TryExtract(it, FT, defaults.FailureThreshold, out int failureThreshold));
        errors.AddIfNotNull(TryExtract(it, HPA_ENABLED, defaults.Hpa.Enabled, out bool hpaEnabled));
        errors.AddIfNotNull(TryExtract(it, HPA_PERCENTAGE, defaults.Hpa.Percentage, out int hpaPercentage));
        if (hpaPercentage is <= 0 or > 100)
            errors.Add(new ConfigurationErrorEntry(
                PORT,
                port.ToString(),
                ConfigurationErrorType.OutOfRangeError,
                "Hpa percentage must be between 1 and 100."
            ));


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
