using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndpointPinger
{
    public enum StatusType
    {
        Unknown = 0,
        Healthy = 1,
        Dieing = 2,
        Dead = 3,
        Recovering = 4,
    }
}
