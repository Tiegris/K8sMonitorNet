using K8sMonitorCore.Util;
using System;

public static class Utils
{

    public static Uri BuildFqdnUri(string ns, string srv, string podIp = null)
    {
        if (podIp.IsNotNullOrWhiteSpace()) {
            int dots = 0;
            foreach (var item in podIp)
                if (item == '.')
                    dots++;
            if (dots != 4)
                throw new K8sFqdnException($"Provided PodIP: '{podIp}' is nt valid.");
            podIp = podIp.Replace('.', '-');
            return new Uri($"{podIp}.{srv}.{ns}.svc.cluster.local");
        } else {
            return new Uri($"{srv}.{ns}.svc.cluster.local");
        }
    }

}
