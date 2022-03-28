kubectl apply -f ./wait/ns.yaml
kubectl config set-context --current --namespace="wait-ns"
kubectl apply -f ./debugger.yaml
kubectl apply -f ./bridge.yaml
kubectl apply -f ./wait/deployments.yaml
kubectl apply -f ./wait/services.yaml
