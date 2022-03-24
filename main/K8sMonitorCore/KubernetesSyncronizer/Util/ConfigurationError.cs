using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KubernetesSyncronizer.Util
{
    public enum ConfigurationErrorType
    {
        ParseError,
        OutOfRangeError,
        RequiredValueNotFound,
    }

    public class ServiceConfigurationError
    {
        List<ConfigurationErrorEntry> errors = new();

        public bool HasErrors => errors.Count > 0;

        public bool NoErrors => errors.Count == 0;

        public void AddIfNotNull(ConfigurationErrorEntry? error) {
            if (error is not null) errors.Add(error);
        }
        public void Add(ConfigurationErrorEntry error) => errors.Add(error);
    }

    public class ConfigurationErrorEntry
    {
        public ConfigurationErrorEntry(string key, string found, ConfigurationErrorType type, string? message = null) {
            Key = key;
            Found = found;
            Message = message;
            Type = type;
        }

        public string Key { get; init; }
        public string Found { get; init; }
        public ConfigurationErrorType Type { get; init; }
        public string? Message { get; init; }
    }
}
