namespace K8sMonitorCore.Aggregation.Dto;

public class ShortStatusDto
{
    public ShortStatusDto(bool healthy, string name) {
        Healthy = healthy;
        Name = name;
    }

    public bool Healthy { get; set; }
    public string Name { get; set; }
}