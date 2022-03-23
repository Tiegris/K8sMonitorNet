k apply -f ./wait/ns.yaml
kns wait-ns
k apply -f ./bridge.yaml
k apply -f ./wait/deployments.yaml
k apply -f ./wait/services.yaml