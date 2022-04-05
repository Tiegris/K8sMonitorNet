using System;

namespace KubernetesSyncronizer.Util;

public class K8sException : Exception
{
    public K8sException() { }

    public K8sException(string message) : base(message) { }
}


public class K8sFqdnException : K8sException
{
    public K8sFqdnException() { }

    public K8sFqdnException(string message) : base(message) { }
}



