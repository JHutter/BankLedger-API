using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BLAUnitTests
{
    public class UnitTestRegistration
    {
        public string IdentityBaseUrl { get; private set; }
        public string ApiBaseUrl { get; private set; }
        public string EndpointRegister { get; private set; }


        public UnitTestRegistration()
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

            EndpointRegister = $"{ApiBaseUrl}/users/register";

        }

        [Theory]
        [InlineData("", "", "", "")]
        [InlineData("sdfsdf", "asdf", "sdfsdfsd", "")]
        [InlineData("sdfsdf", "sdfsdf", "", "sdfsdf")]
        [InlineData("dfsf", "", "sdfsdf", "sdfsdfsd")]
        [InlineData("", "asdf", "sdfasd", "dsafsdf")]
        public async Task Register_EmptyParams_ReturnFailStatus(string username, string password, string firstName, string lastName)
        {
            var apiResponse = await TestUtils.SetUpRegistrationRequest(EndpointRegister, username, password, firstName, lastName);
            var msg = await apiResponse.Content.ReadAsStringAsync();

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, apiResponse.StatusCode);
            Assert.Contains("invalid", msg.ToLower());
        }

        [Theory]
        [InlineData("usernamesaresupermainstream", "whateverpassword", "Britta", "Perry")]
        [InlineData("wingerisnumber1", "whateverpassword", "Jeff", "Winger")]
        [InlineData("secondaccountWinger", "whateverpassword", "Jeff", "Winger")]
        public async Task Register_UniqueUsername_ReturnsOK(string username, string password, string firstName, string lastName)
        {
            var apiResponse = await TestUtils.SetUpRegistrationRequest($"{ApiBaseUrl}/users/register", username, password, firstName, lastName);
            var msg = await apiResponse.Content.ReadAsStringAsync();

            Assert.Equal(System.Net.HttpStatusCode.OK, apiResponse.StatusCode);
            Assert.Contains("created", msg.ToLower());
        }

        [Theory]
        [InlineData("usernamesaresupermainstream2", "whateverpassword", "Britta", "Perry")]
        [InlineData("wingerisnumber2", "whateverpassword", "Jeff", "Winger")]
        [InlineData("secondaccountWinger2", "whateverpassword", "Jeff", "Winger")]
        public async Task Register_NotUniqueUsername_ReturnsBadRequest(string username, string password, string firstName, string lastName)
        {
            var apiResponseFirst = await TestUtils.SetUpRegistrationRequest($"{ApiBaseUrl}/users/register", username, password, firstName, lastName);
            var apiResponse = await TestUtils.SetUpRegistrationRequest($"{ApiBaseUrl}/users/register", username, password, firstName, lastName);
            var msg = await apiResponse.Content.ReadAsStringAsync();

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, apiResponse.StatusCode);
            Assert.Contains("already exists", msg.ToLower());
        }

        [Theory]
        [InlineData("@B3D", "inspectorspacetimeiscool", "Abed", "Nadir")]
        [InlineData("troy's username", "secretpassword", "Troy", "Barnes")]
        [InlineData("troys username again", "secretpassword", "Troy", "Barnes")]
        public async Task Register_IllegalCharUsername_ReturnsBadRequest(string username, string password, string firstName, string lastName)
        {
            var apiResponse = await TestUtils.SetUpRegistrationRequest($"{ApiBaseUrl}/users/register", username, password, firstName, lastName);
            var msg = await apiResponse.Content.ReadAsStringAsync();

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, apiResponse.StatusCode);
            Assert.Contains("illegal", msg.ToLower());
        }

        [Theory]
        [InlineData("coolcoolcoolcoolcoolcoolcoolcoolcoolcoolcoolcoolcoolcoolcoolcoolcoolcoolcoolcoolcoolcoolcoolcoolcoolcoolcoolcoolcoolcoolcoolcool", "inspectorspacetimeiscool", "Abed", "Nadir")]
        [InlineData("ab", "inspectorspacetime", "Abed", "Nadir")]
        [InlineData("a", "inspectorspacetime", "Abed", "Nadir")]
        public async Task Register_ShortOrLongUsername_ReturnsBadRequest(string username, string password, string firstName, string lastName)
        {
            var apiResponse = await TestUtils.SetUpRegistrationRequest($"{ApiBaseUrl}/users/register", username, password, firstName, lastName);
            var msg = await apiResponse.Content.ReadAsStringAsync();

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, apiResponse.StatusCode);
            Assert.Contains("illegal", msg.ToLower());
        }
    }
}
