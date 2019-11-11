using ServiceStack.Configuration;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BLAUnitTests
{
    public class UnitTest1
    {
        //public string IdentityBaseUrl { get => identityBaseUrl; set => identityBaseUrl = value; }
        //public string ApiBaseUrl { get => apiBaseUrl; set => apiBaseUrl = value; }
        public string IdentityBaseUrl { get; private set; } //= Config.BASE_URL;
        public string ApiBaseUrl { get; private set; }

        // constructor
        public UnitTest1()
        {
            //string[] args = { "whatever" };
            //BankLedgerAPI.Program.Main(args);
            string localidentityBaseUrl = Environment.GetEnvironmentVariable("IdentityBaseUrl");
            if (!String.IsNullOrEmpty(IdentityBaseUrl))
            {
                IdentityBaseUrl = localidentityBaseUrl;
            }
            string localapiBaseUrl = Environment.GetEnvironmentVariable("ApiBaseUrl");
            if (!String.IsNullOrEmpty(ApiBaseUrl))
            {
                ApiBaseUrl = localapiBaseUrl;
            }

            ApiBaseUrl = "https://localhost:5001/api";
        }

        [Fact]
        public async Task BasicGetTest_GetOKStatus()
        {
            var apiClient = new HttpClient();
            var apiResponse = await apiClient.GetAsync($"{ApiBaseUrl}/users");

            Assert.Equal(System.Net.HttpStatusCode.OK, apiResponse.StatusCode);
        }

        [Theory]
        [InlineData("", "", "", "")]
        [InlineData("sdfsdf", "asdf", "sdfsdfsd", "")]
        [InlineData("sdfsdf", "sdfsdf", "", "sdfsdf")]
        [InlineData("dfsf", "", "sdfsdf", "sdfsdfsd")]
        [InlineData("", "asdf", "sdfasd", "dsafsdf")]
        public async Task Register_EmptyParams_ReturnFailStatus(string username, string password, string firstName, string lastName)
        {
            var apiClient = new HttpClient();
            string request = "fname=" + firstName + "&lname=" + lastName;
            var content = new StringContent(request, Encoding.UTF8);
            content.Headers.Add("username", username);
            content.Headers.Add("password", password);

            var apiResponse = await apiClient.PostAsync($"{ApiBaseUrl}/users/register", content);
            var msg = await apiResponse.Content.ReadAsStringAsync();

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, apiResponse.StatusCode);
            Assert.Contains("invalid", msg.ToLower());
        }
    }
}
