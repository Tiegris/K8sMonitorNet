using K8sMonitorCore.Aggregation.Dto;
using K8sMonitorCore.Aggregation.Dto.Detailed;
using System;
using System.Collections.Generic;
using System.Linq;

namespace K8sMonitorCore.Aggregation.Service;

public partial class AggregationService
{

    public IList<ServiceInfoDto> PlainList() {
        var statusInfos = pingerManager.Scrape();
        var resources = resourceRegistry.Values;

        return resources.Select(service => new ServiceInfoDto(
            service.Key.ToString(),
            service.Errors,
            statusInfos.Where(endpoint => service.Key.SrvEquals(endpoint.Key)).FirstOrDefault()?.ToDto(),
            service.Errors.HasErrors ?
            null :
            new ServiceSettingsDto(
                service.FailureThreshold,
                service.Timeout,
                service.Period,
                service.Uri,
                service.Hpa
            )
        )).ToList();
    }

}

