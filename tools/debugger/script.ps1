F:
$SOLUTION_FOLDER = "\UNI\MSc_Onlab_1\K8sMonitorNet\main\K8sMonitorCore\"
dotnet build $SOLUTION_FOLDER
$POD = (k get pod -o name | Select-String debugger).Line.Split('/')[1]
kubectl exec -it $POD -- rm -rf /sources
kubectl cp ${SOLUTION_FOLDER} ${POD}:/sources/
kubectl exec -it $POD -- dotnet /sources/K8sMonitorCore/bin/Debug/net6.0/K8sMonitorCore.dll
