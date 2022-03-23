using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KubernetesSyncronizer.Settings
{
    public class Defaults
    {
        public int Timeout { get; set; }
        public int Period { get; set; }
        public int FailureThreshold { get; set; }
        public int Port { get; set; }
        public Hpa Hpa { get; set; }
    }


    public class Hpa
    {
        public bool Enabled { get; set; }
        public int Percentage { get; set; }
    }
}
