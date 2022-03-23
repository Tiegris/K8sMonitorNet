using System;

namespace KubernetesSyncronizer.Util
{
    public static class Utils
    {

        public static Uri BuildFqdnUri(string ns, string srv, int port, string path, string podIp = null) {
            path = path.TrimStart('/');

            if (podIp.IsNotNullOrWhiteSpace()) {
                int dots = 0;
                foreach (var item in podIp)
                    if (item == '.')
                        dots++;
                if (dots != 3)
                    throw new K8sFqdnException($"Provided PodIP: '{podIp}' is nt valid.");
                podIp = podIp.Replace('.', '-');
                return new Uri($"http://{podIp}.{srv}.{ns}.svc.cluster.local:{port}/{path}");
            }
            else {
                return new Uri($"http://{srv}.{ns}.svc.cluster.local:{port}/{path}");
            }
        }

    }
}