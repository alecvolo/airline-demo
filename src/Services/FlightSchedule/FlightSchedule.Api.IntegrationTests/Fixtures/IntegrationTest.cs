using System.Net.Http;
using Xunit;

namespace FlightSchedule.Api.IntegrationTests.Fixtures
{
    [Trait("Category", "Integration")]
    public abstract class IntegrationTest : IClassFixture<ApiWebApplicationFactory>
    {
        //private readonly Checkpoint _checkpoint = new Checkpoint
        //{
        //    SchemasToInclude = new[] {
        //    "Playground"
        //},
        //    WithReseed = true
        //};

        protected readonly ApiWebApplicationFactory Factory;
        protected readonly HttpClient Client;

        public IntegrationTest(ApiWebApplicationFactory fixture)
        {
            Factory = fixture;
            Client = Factory.CreateClient();

            // if needed, reset the DB
            //_checkpoint.Reset(_factory.Configuration.GetConnectionString("SQL")).Wait();
        }
    }
}
