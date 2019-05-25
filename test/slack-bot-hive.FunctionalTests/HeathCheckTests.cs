using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using Xunit;

namespace slack_bot_hive.FunctionalTests
{
    public class HeathCheckTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private const string JsonContentType = "application/json; charset=utf-8";

        private readonly WebApplicationFactory<Startup> _factory;

        public HeathCheckTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task HealthCheck_Should_Return200Ok()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/healthcheck");

            response.EnsureSuccessStatusCode(); // Status Code 200-299

            var responseContent = await response.Content.ReadAsStringAsync();

            responseContent.Should().Be("OK");
        }
    }
}
