using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BLAUnitTests
{
    public class SessionControllerUnitTests
    {
        public string IdentityBaseUrl { get; private set; }
        public string ApiBaseUrl { get; private set; }

        public SessionControllerUnitTests()
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
        public async Task Login_Simple()
        {
            string firstName = "Sleve";
            string lastName = "McDichael";
            string username = "mcdichael" + TestUtils.GenerateRandomString(16, 49, 57);
            string password = "1234";

            var apiResponse = await TestUtils.SetUpRegistrationRequest($"{ApiBaseUrl}/users/register", username, password, firstName, lastName);
            Assert.Equal(System.Net.HttpStatusCode.OK, apiResponse.StatusCode);

            var loginResponse = await TestUtils.SetUpPostRequestWithUsernameAndPassword($"{ApiBaseUrl}/session/login", username, password);
            var msg = await loginResponse.Content.ReadAsStringAsync();
            Assert.Equal(System.Net.HttpStatusCode.OK, loginResponse.StatusCode);
            //Assert.DoesNotContain("Bad Request", msg);
        }

        [Theory]
        [InlineData("onsonsweemey", "secretpassword", "chimichanga")]
        [InlineData("darrylarchideld", "1234", "")]
        [InlineData("anatolismorin", "1234", "12345")]
        [InlineData("glenallenmixon", "1234", " 1234")]
        [InlineData("glenallenmixon2", "1234", "1234 ")]
        public async Task Login_BadPassword(string username, string firstPassword, string secondPassword)
        {
            string firstName = "First";
            string lastName = "Last";
            username += TestUtils.GenerateRandomString(16, 49, 57);

            var apiResponse = await TestUtils.SetUpRegistrationRequest($"{ApiBaseUrl}/users/register", username, firstPassword, firstName, lastName);
            Assert.Equal(System.Net.HttpStatusCode.OK, apiResponse.StatusCode);

            var loginResponse = await TestUtils.SetUpPostRequestWithUsernameAndPassword($"{ApiBaseUrl}/session/login", username, secondPassword);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, loginResponse.StatusCode);
        }

        [Fact]
        public async Task Logout_ValidTokenOkRequest()
        {
            string firstName = "Raul";
            string lastName = "Chamgerlain";
            string username = "chamgerlain" + TestUtils.GenerateRandomString(16, 49, 57);
            string password = "1234567";
            
            var apiResponse = await TestUtils.SetUpRegistrationRequest($"{ApiBaseUrl}/users/register", username, password, firstName, lastName);
            Assert.Equal(System.Net.HttpStatusCode.OK, apiResponse.StatusCode);

            var loginResponse = await TestUtils.SetUpPostRequestWithUsernameAndPassword($"{ApiBaseUrl}/session/login", username, password);
            string authHeader = loginResponse.Headers.GetValues("Authorization").FirstOrDefault();
            Assert.Equal(System.Net.HttpStatusCode.OK, loginResponse.StatusCode);

            var logoutResponse = await TestUtils.SetUpRequestWithAuth($"{ApiBaseUrl}/session/logout", authHeader, "");
            var logoutAuthToken = logoutResponse.Headers.GetValues("Authorization").FirstOrDefault();
            Assert.Equal(System.Net.HttpStatusCode.OK, logoutResponse.StatusCode);
            Assert.Empty(logoutAuthToken);
        }

        [Fact]
        public async Task Logout_InvalidTokenOkRequest()
        {
            string firstName = "Raul";
            string lastName = "Chamgerlain";
            string username = "chamgerlain" + TestUtils.GenerateRandomString(16, 49, 57);
            string password = "1234567";

            var apiResponse = await TestUtils.SetUpRegistrationRequest($"{ApiBaseUrl}/users/register", username, password, firstName, lastName);
            Assert.Equal(System.Net.HttpStatusCode.OK, apiResponse.StatusCode);

            var loginResponse = await TestUtils.SetUpPostRequestWithUsernameAndPassword($"{ApiBaseUrl}/session/login", username, password);
            string authHeader = loginResponse.Headers.GetValues("Authorization").FirstOrDefault();
            Assert.Equal(System.Net.HttpStatusCode.OK, loginResponse.StatusCode);

            var logoutResponse = await TestUtils.SetUpRequestWithAuth($"{ApiBaseUrl}/session/logout", authHeader+"modifiedtokenstringhere", "");
            var logoutAuthToken = logoutResponse.Headers.GetValues("Authorization").FirstOrDefault();
            Assert.Equal(System.Net.HttpStatusCode.OK, logoutResponse.StatusCode);
            Assert.Empty(logoutAuthToken);
        }

        [Fact]
        public async Task Logout_MissingTokenBadRequest()
        {
            string firstName = "Raul";
            string lastName = "Chamgerlain";
            string username = "chamgerlain" + TestUtils.GenerateRandomString(16, 49, 57);
            string password = "1234567";

            var apiResponse = await TestUtils.SetUpRegistrationRequest($"{ApiBaseUrl}/users/register", username, password, firstName, lastName);
            Assert.Equal(System.Net.HttpStatusCode.OK, apiResponse.StatusCode);

            var loginResponse = await TestUtils.SetUpPostRequestWithUsernameAndPassword($"{ApiBaseUrl}/session/login", username, password);
            string authHeader = loginResponse.Headers.GetValues("Authorization").FirstOrDefault();
            Assert.Equal(System.Net.HttpStatusCode.OK, loginResponse.StatusCode);

            var logoutResponse = await TestUtils.SetUpPostRequestWithUsernameAndPassword($"{ApiBaseUrl}/session/logout", username, password);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, logoutResponse.StatusCode);
        }

    }
}
