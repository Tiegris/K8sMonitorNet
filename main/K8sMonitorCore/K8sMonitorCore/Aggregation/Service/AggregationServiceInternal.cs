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

        return resources.Select(a => new ServiceInfoDto(
            a.Name,
            a.Errors,
            statusInfos.Where(b => a.Name == b.Name).FirstOrDefault().ToDto(),
            a.Errors.HasErrors ?
            null :
            new ServiceSettingsDto(
                a.FailureThreshold,
                a.Timeout,
                a.Period,
                a.Uri,
                a.Hpa
            )
        )).ToList();
    }

}

