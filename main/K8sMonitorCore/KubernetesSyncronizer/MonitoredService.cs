using k8s.Models;
using KubernetesSyncronizer.Settings;
using KubernetesSyncronizer.Util;
using Pinger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KubernetesSyncronizer
{
    public class MonitoredService {
        public MonitoredService(string name, ServiceConfigurationError errors) {
            Name = name;
            Errors = errors;
        }

        public string Name { get; init; }
        public int FailureThreshold { get; init; }
        public TimeSpan Timeout { get; init; }
        public TimeSpan Period { get; init; }
        public Uri? Uri { get; init; }
        public Hpa? Hpa { get; init; }
        public ServiceConfigurationError Errors { get; init; }


        public IEnumerable<(string, Endpoint)> GetEndpoints() {
            if (Errors.HasErrors || Uri is null)
                yield break;

            if (Hpa is { Enabled: false }) {
                yield return (Name, new Endpoint(FailureThreshold, Timeout, Period, Uri));
            } else {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<string> GetEndpointNames() {
            if (Errors.HasErrors || Uri is null)
                yield break;

            if (Hpa is { Enabled: false }) {
                yield return Name;
            } else {
                throw new NotImplementedException();
            }
        }

    }
}
