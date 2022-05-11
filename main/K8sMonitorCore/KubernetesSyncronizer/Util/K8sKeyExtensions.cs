using KubernetesSyncronizer.Data;

namespace KubernetesSyncronizer.Util;
static public class K8sKeyExtensions
{
    public static K8sKey GetSvcNs(this K8sKey value) {
        return new K8sKey(value.Ns, value.Svc);
    }

}
