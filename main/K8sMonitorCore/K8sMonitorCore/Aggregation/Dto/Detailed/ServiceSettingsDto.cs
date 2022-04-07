﻿using KubernetesSyncronizer.Data;
using System;

namespace K8sMonitorCore.Aggregation.Dto.Detailed;

public class ServiceSettingsDto
{
    public ServiceSettingsDto(int failureThreshold, TimeSpan timeout, TimeSpan period, Uri? uri, Hpa? hpa) {
        FailureThreshold = failureThreshold;
        Timeout = timeout;
        Period = period;
        Uri = uri;
        Hpa = hpa;
    }

    public int FailureThreshold { get; init; }
    public TimeSpan Timeout { get; init; }
    public TimeSpan Period { get; init; }
    public Uri? Uri { get; init; }
    public Hpa? Hpa { get; init; }
}


