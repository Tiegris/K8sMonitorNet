using KubernetesSyncronizer.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace KubernetesSyncronizer.Util;
static public class K8sKeyExtensions
{
    public static K8sKey GetSrvNs(this K8sKey value) {
        return new K8sKey(value.Ns, value.Srv);
    }

}
