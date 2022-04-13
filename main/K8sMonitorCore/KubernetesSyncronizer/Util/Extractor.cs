using k8s;
using k8s.Models;
using KubernetesSyncronizer.Data;
using KubernetesSyncronizer.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics.CodeAnalysis;
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
    private readonly IKubernetes k8s;
    private readonly ResourceRegistry resourceRegistry;
    private readonly ILoggerFactory loggerFactory;

    public Extractor(IOptions<Defaults> options, IKubernetes k8s, ResourceRegistry resourceRegistry, ILoggerFactory loggerFactory) {
        this.defaults = options.Value;
        this.k8s = k8s;
        this.resourceRegistry = resourceRegistry;
        this.loggerFactory = loggerFactory;
    }

    public bool TryExtractMonitoredService(V1Service it, [MaybeNullWhen(false)] out MonitoredService monitoredService) {
        var dict = it.Metadata.Annotations;
        var errors = new ServiceConfigurationError();

        if (dict is null || !dict.TryGetValue(PATH, out string? path)) {
            monitoredService = null;
            return false;
        }
        path = path.TrimStart('/');

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
        scheme = scheme.Replace("://", "");
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

        var srvName = it.ExtractKey();

        monitoredService = new(srvName, errors) {
            Timeout = new TimeSpan(0, 0, timeout),
            Period = new TimeSpan(0, 0, period),
            FailureThreshold = failureThreshold,
            Uri = BuildFqdnUri(scheme, ns, srv, port, path),
            Hpa = new Hpa {
                Enabled = hpaEnabled,
                Percentage = hpaPercentage
            },
            PodMonitor = hpaEnabled ? new(k8s, resourceRegistry, srvName, it.ExtractLabelString(), loggerFactory.CreateLogger<PodMonitor>()) : null
        };
        return true;
    }

}
