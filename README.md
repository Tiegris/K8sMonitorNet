# K8sMonitorNet

## Setup

### Install using helm

Recommended installation method is [helm](https://helm.sh/). Tested with version: v3.5.4

Before installation, it is recommended to review the [configuration](#settings).

```bash
cd main/helm
kubectl create ns monitor-ns
helm install k8s-monitor-net k8s-monitor-net --namespace="monitor-ns"
```

### Settings

The app can be configured from the [values.yaml](main/helm/k8s-monitor-net/values.yaml), read the comments in that file for more info.

### Cluster configuration

The app monitors services with the `mnet.uri.path` annotation present.

This annotation is used to determine which path to ping of the monitored service.

List of all annotations

- mnet.uri.scheme: uri scheme, supported (http / https)
- mnet.uri.port: port to ping (valid port number)
- mnet.uri.path: path to ping (string)
- mnet.timeout: timeout in seconds (int)
- mnet.period: delay between the beginning pings in seconds (int)
- mnet.failureThreshold: after this many unsuccessful pings, the service is considered Dead (int)
- mnet.hpa.enabled: set to true to monitor pods, false to only the service (bool)
- mnet.hpa.percentage: At least this percent of the pods of the service must be Healthy to consider the service Healthy (int)

Example service:

```yaml
apiVersion: v1
kind: Service
metadata:
  name: example-svc
  annotations:
    mnet.uri.scheme: "http://"  # '://' suffix not necesarry
    mnet.uri.port: "80"
    mnet.uri.path: "/Echo"      # '/' prefix not necesarry
    mnet.timeout: "5"
    mnet.period: "10"
    mnet.failureThreshold: "3"
    mnet.hpa.enabled: "true"
    mnet.hpa.percentage: "60"
```

If you do not provide an annotation, the default value is used instead. Default values are defined in the values.yaml > appSettings > Defaults__

There is no default value for `mnet.uri.path`, if you omit this, the service will not be monitored.

## Public endpoint

There are 4 public endpoints of the app, all with the `/api/public/` prefix.

### status/{ns}

Get the health of a namespace.

Returns:

- 404 if there is no such monitored namespace
- 533 if there are Dead services in that namespace
- 200 if all monitored services are Healthy in that namespace

### status/{ns}/{svc}

Get the health of a service within a namespace.

Returns:

- 404 if there is no such monitored service
- 533 if the service is Dead
- 200 if the service is Healthy

### status/namespaces or status/nss

List all monitored namespaces.

Returns the list of namespaces in JSON format, example:

```json
[
    {
        "healthy": true,
        "name": "example-1-ns"
    },
    {
        "healthy": false,
        "name": "example-2-ns"
    }
]
```

### status/services or status/svcs

List all monitored services.

Returns the list of namespaces in JSON format, example:

```json
//Name format: "<namespace-name>::<service-name>"
[
    {
        "healthy": true,
        "name": "example-1-ns::app-1"
    },
    {
        "healthy": true,
        "name": "example-2-ns::app-1"
    },
    {
        "healthy": false,
        "name": "example-2-ns::app-2"
    }
]
```

## Tips

### Port forward

```bash
SELECTOR="monitor"
POD=$(k get pod --namespace="monitor-ns" -o name | grep $SELECTOR)
kubectl port-forward --namespace="monitor-ns" $POD 8080:80
```

On Windows PowerShell:

```PowerShell
$SELECTOR = "monitor"
$POD = (k get pod --namespace="monitor-ns" -o name | Select-String $SELECTOR).Line.Split('/')[1]
kubectl port-forward --namespace="monitor-ns" $POD 80:80
```

### Ingress controller

You will need an ingress controller in your cluster for the ingress to work. On local developer clusters, by default usually there aren't any. Install nginx IngressController by running this:

```bash
helm upgrade --install ingress-nginx ingress-nginx \
  --repo https://kubernetes.github.io/ingress-nginx \
  --namespace ingress-nginx --create-namespace
```

## Contribution

Source files located at main/K8sMonitorCore. This a .NET 6 app, you will need the .NET6 SDK an IDE for it.

### Code structure

Projects:

- EndpointPinger: module for pinging any endpoints.

- KubernetesSyncronizer: module for watching service configurations and registering pingers

- K8sMonitorCore: the main project with aggregation services, controllers, and WebUi

### Debugging

#### Local

To run the app, you need a developer cluster. I used DockerDesktop kubernetes. Running the app locally from your IDE works, however the pingers will not be able to access the endpoints.

#### Remote

To debug inside the cluster, deploy the app in developer mode:

```bash
helm install k8s-monitor-net k8s-monitor-net --values="k8s-monitor-net/values-developer.yaml" --namespace="monitor-ns"
```

This will deploy the latest version of the app from dockerhub.

After this, you have 60 seconds to connect to the process with a remote debugger.

With VisualStudio and DockerDesktop:

Containers > right click on pod > Attach to process > Select the dotnet process

With this method it is possible to use even breakpoints if the local code is the same as the code used to compile the docker image.

It is possible to remote debug without building a new docker image. Compiling locally and copying the files into the pod might work: [example](tools/debugger/script.ps1)

### Additional files

- Under the clusters folder there are kubernetes yaml files, useful for setting up example services.

- Under the tools folder there are additional tools and scripts useful when debugging and testing.

- There are GitHub Actions CI pipelines. One for building and publishing docker images. One for running SonarCloud analysis.
