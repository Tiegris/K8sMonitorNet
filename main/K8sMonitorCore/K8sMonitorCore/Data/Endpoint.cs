using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace K8sMonitorCore.Data
{
        


    public class MonitoredSrv
    {
        public string Name { get; init; }
        public string NsName { get; init; }

        public bool MonitorPods { get; private set; }




        public void Ping() {

        }
    }


    //public class MonitoredPod : IMonitoredEndpoint
    //{
    //    public string Ip { get; init; }
    //    public MonitoredSrv Srv { get; private set; }

    //    public Uri Uri { get; set; }

    //    public void Ping()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
