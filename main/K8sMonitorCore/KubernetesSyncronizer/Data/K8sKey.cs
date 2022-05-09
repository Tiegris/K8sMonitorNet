using EndpointPinger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KubernetesSyncronizer.Data;
public class K8sKey : IKey, IComparable<K8sKey>
{
    public bool NsEquals(IKey rhs) {
        if (rhs is K8sKey rhsKey) {
            return Ns == rhsKey.Ns;
        }
        return false;
    }

    public bool SrvEquals(IKey rhs) {
        if (rhs is K8sKey rhsKey) {
            return Ns == rhsKey.Ns && Srv == rhsKey.Srv;
        }
        return false;
    }

    public string Ns { get; init; }
    public string Srv { get; init; }

    public string? Pod { get; init; }

    public K8sKey(string ns, string srv) {
        Ns = ns;
        Srv = srv;
    }

    private string? _toString = null;
    public override string ToString() => _toString ??= $"{Ns}::{Srv}";

    public int CompareTo(K8sKey? other) {
        int ns = Ns.CompareTo(other?.Ns);
        if (ns == 0)
            return Srv.CompareTo(other?.Srv);
        return ns;
    }

    public override bool Equals(object? obj) {
        return obj is K8sKey key &&
               Ns == key.Ns &&
               Srv == key.Srv &&
               Pod == key.Pod;
    }

    public override int GetHashCode() {
        return HashCode.Combine(Ns, Srv, Pod);
    }
}
