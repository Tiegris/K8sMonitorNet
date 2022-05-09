F:
$SOLUTION_FOLDER = "\UNI\MSc_Onlab_1\K8sMonitorNet\main\K8sMonitorCore\"
dotnet build $SOLUTION_FOLDER
$POD = (k get pod -o name | Select-String debugger).Line.Split('/')[1]
kubectl exec -it $POD -- rm -rf /sources
kubectl cp ${SOLUTION_FOLDER} ${POD}:/sources/
kubectl exec -it $POD -- bash -c "cd /sources/K8sMonitorCore/bin/Debug/net6.0/ && dotnet K8sMonitorCore.dll"
#kubectl exec -it $POD -- dotnet run --project /sources/K8sMonitorCore/K8sMonitorCore.csproj
