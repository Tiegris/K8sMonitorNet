using Pinger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KubernetesSyncronizer
{
    public class KubernetesEndpoint : Endpoint
    {
        public KubernetesEndpoint(int failureThreshold, TimeSpan timeout, TimeSpan period, Uri uri) : 
            base(failureThreshold, timeout, period, uri) {

        }
    }
}
