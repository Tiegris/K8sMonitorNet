namespace K8sMonitorCore.Aggregation.Dto.Simple;

public class SimpleStatusDto
{
    public SimpleStatusDto(bool healthy, string name) {
        Healthy = healthy;
        Name = name;
    }

    public bool Healthy { get; set; }
    public string Name { get; set; }
}