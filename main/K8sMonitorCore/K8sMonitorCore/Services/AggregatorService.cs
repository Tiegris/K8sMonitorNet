﻿using K8sMonitorCore.Domain;
using KubernetesSyncronizer;
using Pinger;
using System.Collections.Generic;
using System.Linq;

namespace K8sMonitorCore.Services
{
    public class AggregatorService
    {
        private readonly ResourceRegistry resourceRegistry;
        private readonly PingerManager pingerManager;

        public AggregatorService(ResourceRegistry resourceRegistry, PingerManager pingerManager) {
            this.resourceRegistry = resourceRegistry;
            this.pingerManager = pingerManager;
        }


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


}
