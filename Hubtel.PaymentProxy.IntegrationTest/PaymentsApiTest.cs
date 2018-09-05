using Hubtel.PaymentProxy.IntegrationTest.Data;
using Hubtel.PaymentProxyData.EntityModels;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Hubtel.PaymentProxy.IntegrationTest
{
    public class PaymentsApiTest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public PaymentsApiTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task AddOrderWithFullPayment_ReturnsAddedOrder()
        {
            // Arrange
            var client = _factory.CreateClient();
            var orderRequestDto = PredefinedData.OrderPaymentRequestWithPayment;

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"hubtel-bearer ODBlNTJkN2ExY2RmNDZkOGFlNzJhNGE3NTdjNGFhOGU6eyJBY2NvdW50SWQiOiJnZW9yZ2VoYWdhbiJ9");
            var contents = new StringContent(JsonConvert.SerializeObject(orderRequestDto), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/v1/payments", contents);

            // Assert
            var responseString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var ordersResponse = JsonConvert.DeserializeObject<PaymentsApiResponse<PaymentRequest>>(responseString);
            Assert.Equal("Paid", ordersResponse.Data.Status);
        }
    }
}
