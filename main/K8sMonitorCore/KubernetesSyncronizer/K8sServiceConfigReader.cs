using k8s;
using k8s.Models;
using KubernetesSyncronizer.Settings;
using Microsoft.Extensions.Options;
using Pinger;
using System;
using System.Threading.Tasks;
using static k8s.WatchEventType;

namespace KubernetesSyncronizer
{
    public class K8sServiceConfigReader : IDisposable
    {
        private readonly IKubernetes client;
        private Watcher<V1Service>? watch;
        private readonly PingerManager pingerManager;
        private readonly Defaults defaults;

        private readonly ResourceRegistry resourceRegistry;

        public K8sServiceConfigReader(IKubernetes k8s, PingerManager pingerManager, IOptions<Defaults> options, ResourceRegistry resourceRegistry) {
            this.client = k8s;
            this.pingerManager = pingerManager;
            this.defaults = options.Value;
            this.resourceRegistry = resourceRegistry;
        }

        public void StartAndForget() => _ = StartWathcingAsync();

        private async Task StartWathcingAsync() {
            var nss = await client.ListNamespaceAsync();
            var podlistResp = client.ListServiceForAllNamespacesWithHttpMessagesAsync(watch: true);
            watch = podlistResp.Watch<V1Service, V1ServiceList>(WatchEventHandler);
        }


        private void WatchEventHandler(WatchEventType type, V1Service item) {
            switch (type) {
                case Added:
                    resourceRegistry.Add(item);
                    break;
                case Modified:
                    resourceRegistry.Delete(item);
                    resourceRegistry.Add(item);
                    break;
                case Deleted:
                    resourceRegistry.Delete(item);
                    break;
                case Error:
                    //TODO
                    break;
            }
            Console.WriteLine("Service: {0} {1}", item.Metadata.Name, type);
        }

        #region Dispose
        private bool disposed;
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing) {
            if (!disposed) {
                if (disposing) {
                    watch?.Dispose();
                }
                disposed = true;
            }
        }
        #endregion
    }
}
