using k8s.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KubernetesSyncronizer
{
    public class ResourceRegistry
    {

        private readonly ConcurrentDictionary<string, KubernetesEndpoint> map = new();

        public void Add(string name, V1Service v1Service) {
            
        }

        public void Delete(string name) {

        }
    }
}
