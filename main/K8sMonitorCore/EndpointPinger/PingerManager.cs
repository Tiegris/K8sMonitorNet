using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;

namespace EndpointPinger;

public class PingerManager
{
    private readonly ConcurrentDictionary<IKey, EndpointPinger> map = new();

    private readonly IHttpClientFactory hcf;
    private readonly ILoggerFactory lf;

    public PingerManager(IHttpClientFactory hcf, ILoggerFactory lf) {
        this.hcf = hcf;
        this.lf = lf;
    }
    public ICollection<IKey> EndpointNames => map.Keys;

    public void RegisterEndpoint(IKey key, Endpoint endpoint) {
        var epp = new EndpointPinger(endpoint, hcf, lf);
        epp.StartAndForget();
        map.TryAdd(key, epp);
    }

    public void UnregisterEndpoint(IKey name) {
        map.TryRemove(name, out EndpointPinger? epp);
        epp?.Dispose();
    }

    public List<EndpointStatusInfo> Scrape() {
        List<EndpointStatusInfo> ret = new();
        foreach (var item in map) {
            ret.Add(new EndpointStatusInfo(
                key: item.Key,
                endpoint: item.Value.Endpoint
            ));
        }
        return ret;
    }

}
