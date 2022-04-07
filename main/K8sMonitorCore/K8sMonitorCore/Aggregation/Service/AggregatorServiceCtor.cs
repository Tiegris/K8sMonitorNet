using EndpointPinger;
using KubernetesSyncronizer.Services;

namespace K8sMonitorCore.Aggregation.Service;

public partial class AggregationService
{
    private readonly ResourceRegistry resourceRegistry;
    private readonly PingerManager pingerManager;

    public AggregationService(ResourceRegistry resourceRegistry, PingerManager pingerManager) {
        this.resourceRegistry = resourceRegistry;
        this.pingerManager = pingerManager;
    }
}
