using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BLAUnitTests
{
    public class APIHealthTest
    {
        public string IdentityBaseUrl { get; private set; }
        public string ApiBaseUrl { get; private set; }

        public APIHealthTest()
        {
            string localidentityBaseUrl = Environment.GetEnvironmentVariable("IdentityBaseUrl");
            if (!String.IsNullOrEmpty(IdentityBaseUrl))
            {
                IdentityBaseUrl = localidentityBaseUrl;
            }
            string localBaseUrl = Environment.GetEnvironmentVariable("BLA_BASE_URL");
            if (String.IsNullOrEmpty(ApiBaseUrl))
            {
                ApiBaseUrl = localBaseUrl + "/api";
            }
        }

        [Fact]
        public async Task BasicGetTest_GetOKStatus()
        {
            var apiClient = new HttpClient();
            var apiResponse = await apiClient.GetAsync($"{ApiBaseUrl}/users");

            Assert.Equal(System.Net.HttpStatusCode.OK, apiResponse.StatusCode);
        }
    }
}
