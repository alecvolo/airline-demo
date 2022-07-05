using System.Text;

namespace FlightSchedule.Api.Infrastructure;

public class TenantDbConnectionStringManager : ITenantDbConnectionStringManager
{
    private Dictionary<string, Dictionary<string, string?>?> _connectionStringParameters;
    private Dictionary<string, string> _connectionStrings;

    public TenantDbConnectionStringManager(IConfiguration configuration)
        : this(configuration, "ConnectionStrings")
    {

    }

    public TenantDbConnectionStringManager(IConfiguration configuration, string sectionName)
    {
        var configurationSection = configuration.GetSection(sectionName);

        configurationSection.GetReloadToken().RegisterChangeCallback(o =>
        {
            _connectionStringParameters = CollectConnectionStrings(configurationSection);
            _connectionStrings = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
        }, this);
        _connectionStringParameters = CollectConnectionStrings(configurationSection);
        _connectionStrings = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
    }

    private static Dictionary<string, Dictionary<string, string?>?> CollectConnectionStrings(
        IConfigurationSection section)
    {
        var result = new Dictionary<string, Dictionary<string, string?>?>(StringComparer.CurrentCultureIgnoreCase);
        foreach (var configurationSection in section.GetChildren().Where(t => !string.IsNullOrWhiteSpace(t.Value)))
        {
            var dict = configurationSection.Value?.Split(';',
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(s => s.Split('=', StringSplitOptions.TrimEntries)).ToDictionary(s => s[0],
                    s => s.LastOrDefault(), StringComparer.CurrentCultureIgnoreCase);
            result[configurationSection.Key] = dict;
        }

        return result;
    }

    public string Get(string name)
    {
        if (_connectionStrings.TryGetValue(name, out var connectionString))
        {
            return connectionString;
        }

        _connectionStringParameters.TryGetValue("_default", out var connectionParameters);
        connectionParameters = new Dictionary<string, string?>(
            connectionParameters ?? new Dictionary<string, string?>(), StringComparer.CurrentCultureIgnoreCase);
        if (_connectionStringParameters.TryGetValue(name, out var clientConnectionStringParameters))
        {
            foreach (var clientConnectionStringParameter in clientConnectionStringParameters!)
            {
                connectionParameters[clientConnectionStringParameter.Key] = clientConnectionStringParameter.Value;
            }
        }

        if (!connectionParameters.ContainsKey("User Id"))
        {
            connectionParameters["User Id"] = name;
        }

        connectionString = connectionParameters.Aggregate(new StringBuilder(), (sb, kv) =>
        {
            sb.Append(kv.Key).Append(string.IsNullOrWhiteSpace(kv.Value) ? "" : "=" + kv.Value).Append(';');
            return sb;

        }, sb => sb.ToString());
        _connectionStrings[name] = connectionString;
        return connectionString;
    }
}
