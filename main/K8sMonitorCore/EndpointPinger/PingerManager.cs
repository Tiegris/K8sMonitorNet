﻿using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;

namespace Pinger;

public class PingerManager
{
    private readonly ConcurrentDictionary<string, EndpointPinger> map = new();

    private readonly IHttpClientFactory hcf;
    private readonly ILoggerFactory lf;

    public PingerManager(IHttpClientFactory hcf, ILoggerFactory lf) {
        this.hcf = hcf;
        this.lf = lf;
    }
    public ICollection<string> EndpointNames => map.Keys;

    public string RegisterEndpoint(string name, Endpoint endpoint) {
        var epp = new EndpointPinger(endpoint, hcf, lf);
        epp.StartAndForget();
        map.TryAdd(name, epp);
        return name;
    }

    public void UnregisterEndpoint(string name) {
        map.TryRemove(name, out EndpointPinger? epp);
        epp?.Dispose();
    }

    public List<EndpointStatusInfo> Scrape() {
        List<EndpointStatusInfo> ret = new();
        foreach (var item in map) {
            ret.Add(new EndpointStatusInfo(
                name: item.Key,
                endpoint: item.Value.Endpoint
            ));
        }
        return ret;
    }

}
