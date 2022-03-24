﻿namespace KubernetesSyncronizer.Settings
{
    public class Defaults
    {
        public Defaults(int timeout, int period, int failureThreshold, int port, Hpa hpa, string scheme) {
            Timeout = timeout;
            Period = period;
            FailureThreshold = failureThreshold;
            Port = port;
            Hpa = hpa;
            Scheme = scheme;
        }

        public int Timeout { get; set; }
        public int Period { get; set; }
        public int FailureThreshold { get; set; }
        public int Port { get; set; }
        public Hpa Hpa { get; set; }
        public string Scheme { get; set; }
    }


    public class Hpa
    {
        public bool Enabled { get; set; }
        public int Percentage { get; set; }
    }
}
