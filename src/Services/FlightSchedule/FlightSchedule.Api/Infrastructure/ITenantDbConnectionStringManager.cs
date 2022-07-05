namespace FlightSchedule.Api.Infrastructure;

public interface ITenantDbConnectionStringManager
{
    string Get(string name);
}