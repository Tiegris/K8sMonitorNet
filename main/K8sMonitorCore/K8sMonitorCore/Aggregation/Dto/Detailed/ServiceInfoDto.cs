using KubernetesSyncronizer.Data;

namespace K8sMonitorCore.Aggregation.Dto.Detailed;

public class ServiceInfoDto
{
    public ServiceInfoDto(string name, ServiceConfigurationError errors, ServiceHealthStatusDto? status, ServiceSettingsDto? pingerSettings) {
        Name = name;
        Errors = errors;
        Status = status;
        PingerSettings = pingerSettings;
    }

    public string Name { get; init; }
    public ServiceConfigurationError Errors { get; init; }
    public ServiceHealthStatusDto? Status { get; set; }
    public ServiceSettingsDto? PingerSettings { get; init; }
}


