using K8sMonitorCore.Domain;
using System.Collections.Generic;
using System.Linq;

namespace K8sMonitorCore.Aggregation.Service;

public partial class AggregationService
{
    public void AggregateByNamespace() {
        var statusInfos = pingerManager.Scrape();
        var resources = resourceRegistry.Values;

    }

    public IList<ServiceInfoDto> PlainList() {
        var statusInfos = pingerManager.Scrape();
        var resources = resourceRegistry.Values;

        return resources.Select(service => new ServiceInfoDto(
            service.Name,
            service.Errors,
            statusInfos.Where(endpoint => endpoint.Name.Contains(service.Name)).FirstOrDefault().ToDto(),
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

