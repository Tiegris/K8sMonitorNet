using EndpointPinger;
using System;

namespace KubernetesSyncronizer.Data;
public class K8sKey : IKey, IComparable<K8sKey>
{
    public bool NsEquals(IKey rhs) {
        if (rhs is K8sKey rhsKey) {
            return Ns == rhsKey.Ns;
        }
        return false;
    }

    public bool SvcEquals(IKey rhs) {
        if (rhs is K8sKey rhsKey) {
            return Ns == rhsKey.Ns && Svc == rhsKey.Svc;
        }
        return false;
    }

    public string Ns { get; init; }
    public string Svc { get; init; }

    public string? Pod { get; init; }

    public K8sKey(string ns, string svc) {
        Ns = ns;
        Svc = svc;
    }

    private string? _toString = null;
    public override string ToString() => _toString ??= $"{Ns}::{Svc}";

    public int CompareTo(K8sKey? other) {
        int ns = Ns.CompareTo(other?.Ns);
        if (ns == 0)
            return Svc.CompareTo(other?.Svc);
        return ns;
    }

    public override bool Equals(object? obj) {
        return obj is K8sKey key &&
               Ns == key.Ns &&
               Svc == key.Svc &&
               Pod == key.Pod;
    }

    public override int GetHashCode() {
        return HashCode.Combine(Ns, Svc, Pod);
    }
}
