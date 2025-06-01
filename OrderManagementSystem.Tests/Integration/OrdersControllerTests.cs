using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderManagementSystem.Application.Models;
using OrderManagementSystem.Domain.Entities;
using OrderManagementSystem.Domain.Enums;
using OrderManagementSystem.Infrastructure.Data;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace OrderManagementSystem.Tests.Integration
{
    public class OrdersControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        // Known GUIDs from DbSeeder
        private static readonly Guid GoldOrderId = DbSeeder.GoldOrderId;
        private static readonly Guid LoyalNewOrderId = DbSeeder.LoyalNewOrderId;
        private static readonly Guid HighValueOrderId = DbSeeder.HighValueOrderId;

        public OrdersControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove existing DbContext registration
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<OrderManagementSystemContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    // In-memory DbContext for testing
                    services.AddDbContext<OrderManagementSystemContext>(opts =>
                        opts.UseInMemoryDatabase("TestDb"));

                    // Seed test data
                    var sp = services.BuildServiceProvider();
                    using var scope = sp.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<OrderManagementSystemContext>();
                    DbSeeder.SeedTestData(db);
                });
            });
        }

        [Fact]
        public async Task GetDiscount_ForGoldOrder_ReturnsGoldDiscount()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/api/v1.0/Orders/{GoldOrderId}/discount");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var discountResult = await response.Content.ReadFromJsonAsync<DiscountResult>();
            Assert.NotNull(discountResult);

            // Gold order total is 200; 10% off =20
            Assert.Equal(200m, discountResult!.OriginalTotal);
            Assert.Equal(20m, discountResult.DiscountAmount);
            Assert.Contains("Gold Customer 10% Off", discountResult.AppliedRules);
        }

        [Fact]
        public async Task GetDiscount_ForLoyalNewOrder_ReturnsLoyalDiscount()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/api/v1.0/Orders/{DbSeeder.LoyalNewOrderId}/discount");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var discountResult = await response.Content.ReadFromJsonAsync<DiscountResult>();
            Assert.NotNull(discountResult);

            // Loyal new order total is 400; 5% off =20
            Assert.Equal(400m, discountResult!.OriginalTotal);
            Assert.Equal(20m, discountResult.DiscountAmount);
            Assert.Contains("Silver Customer 5% Off", discountResult.AppliedRules);
        }

        [Fact]
        public async Task GetDiscount_ForHighValueOrder_ReturnsCombinedDiscounts()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/api/v1.0/Orders/{HighValueOrderId}/discount");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var discountResult = await response.Content.ReadFromJsonAsync<DiscountResult>();
            Assert.NotNull(discountResult);

            // Original total 600
            Assert.Equal(600m, discountResult!.OriginalTotal);

            // Bronze customer 2% of 600 = 12, plus High-Value flat 25, total = 37
            Assert.Equal(37m, discountResult.DiscountAmount);

            Assert.Contains("Bronze Customer 2% Off", discountResult.AppliedRules);
            Assert.Contains("High-Value Order $25 Off", discountResult.AppliedRules);
        }


        [Fact]
        public async Task GetDiscount_ForNonexistentOrder_ReturnsNotFound()
        {
            var client = _factory.CreateClient();
            var randomId = Guid.NewGuid();

            var response = await client.GetAsync($"/api/v1.0/Orders/{randomId}/discount");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
