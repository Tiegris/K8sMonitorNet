using System;

namespace KubernetesSyncronizer.Util;

public static class Utils
{

    public static Uri BuildFqdnUri(string scheme, string ns, string srv, int port, string path, string? podIp = null) {
        path = path.TrimStart('/');

        if (!string.IsNullOrWhiteSpace(podIp)) {
            int dots = 0;
            foreach (var item in podIp)
                if (item == '.')
                    dots++;
            if (dots != 3)
                throw new K8sFqdnException($"Provided PodIP: '{podIp}' is nt valid.");
            podIp = podIp.Replace('.', '-');
            return new Uri($"{scheme}://{podIp}.{srv}.{ns}.svc.cluster.local:{port}/{path}");
        } else {
            return new Uri($"{scheme}://{srv}.{ns}.svc.cluster.local:{port}/{path}");
        }
    }

}
