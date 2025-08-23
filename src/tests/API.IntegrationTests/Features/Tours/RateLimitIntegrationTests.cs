using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

// This attribute is crucial for creating a test fixture that initializes
// the WebApplicationFactory once for all tests in this class.
[TestFixture]
public class RateLimitIntegrationTests
{
    // The WebApplicationFactory is the core of integration testing.
    // It bootstraps your API in-memory for testing.
    private WebApplicationFactory<Program> _factory;
    private HttpClient _client;

    // [OneTimeSetUp] runs once before any tests in this fixture.
    // Use this for expensive setup, like creating the factory.
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _factory = new WebApplicationFactory<Program>();
    }

    // [SetUp] runs before each individual test method.
    // Use this to create a fresh HttpClient for each test (good practice).
    [SetUp]
    public void Setup()
    {
        _client = _factory.CreateClient();
    }

    // [TearDown] runs after each test to clean up resources.
    [TearDown]
    public void TearDown()
    {
        _client?.Dispose();
    }

    // [OneTimeTearDown] runs after all tests are done to clean up the factory.
    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _factory?.Dispose();
    }

    // The [Test] attribute marks a method as a test case.
    [Test]
    public async Task GetTours_ExceedsLimit_Returns429()
    {
        // Arrange
        // We'll use the same client for all requests to simulate one IP.
        var requestLimit = 30; // From your appsettings.json rule
        var successfulRequests = 0;

        // Act & Assert
        for (int i = 0; i < requestLimit + 5; i++)
        {
            var response = await _client.GetAsync("/api/tours");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                successfulRequests++;
                // If we miraculously get more OKs than the limit, fail early.
                Assert.That(successfulRequests, Is.AtMost(requestLimit), 
                    "The number of successful requests should not exceed the limit before being blocked.");
            }
            else if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                // Assert - We got rate-limited! This is the expected behavior.
                Assert.That(successfulRequests, Is.EqualTo(requestLimit), 
                    $"Should have had exactly {requestLimit} successful requests before being limited.");

                // Optional: Check for the 'Retry-After' header
                // Assert.That(response.Headers.Contains("Retry-After"), Is.True);

                return; // Test passed, exit early.
            }
        }

        // If we finish the loop without hitting a 429, the test fails.
        Assert.Fail($"The request was never rate limited. Made {requestLimit + 5} requests and {successfulRequests} succeeded. Check your rate limit configuration.");
    }
}