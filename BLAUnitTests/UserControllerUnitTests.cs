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
    public class UnitTest1
    {
        public string IdentityBaseUrl { get; private set; }
        public string ApiBaseUrl { get; private set; }

        public UnitTest1()
        {
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

            // TODO do this using env vars instead
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
            var apiResponse = await SetUpRequest(username, password, firstName, lastName);
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
            var apiResponse = await SetUpRequest(username, password, firstName, lastName);
            var msg = await apiResponse.Content.ReadAsStringAsync();

            Assert.Equal(System.Net.HttpStatusCode.OK, apiResponse.StatusCode);
            Assert.Contains("created", msg.ToLower());
        }

        [Theory]
        [InlineData("usernamesaresupermainstream2", "whateverpassword", "Britta", "Perry")]
        [InlineData("wingerisnumber2", "whateverpassword", "Jeff", "Winger")]
        [InlineData("secondaccountWinger2", "whateverpassword", "Jeff", "Winger")]
        public async Task Register_NotUniqueUsername_ReturnsOK(string username, string password, string firstName, string lastName)
        {
            var apiResponseFirst = await SetUpRequest(username, password, firstName, lastName);
            var apiResponse = await SetUpRequest(username, password, firstName, lastName);
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
            var apiResponse = await SetUpRequest(username, password, firstName, lastName);
            var msg = await apiResponse.Content.ReadAsStringAsync();

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, apiResponse.StatusCode);
            Assert.Contains("illegal", msg.ToLower());
        }

        [Theory]
        [InlineData("coolcoolcoolcoolcoolcoolcoolcoolcoolcoolcoolcoolcoolcoolcoolcoolcoolcoolcoolcoolcoolcoolcoolcoolcoolcoolcoolcoolcoolcoolcoolcool", "inspectorspacetimeiscool", "Abed", "Nadir")]
        [InlineData("ab", "inspectorspacetimeiscool", "Abed", "Nadir")]
        [InlineData("a", "inspectorspacetimeiscool", "Abed", "Nadir")]
        public async Task Register_ShortOrLongUsername_ReturnsBadRequest(string username, string password, string firstName, string lastName)
        {
            var apiResponse = await SetUpRequest(username, password, firstName, lastName);
            var msg = await apiResponse.Content.ReadAsStringAsync();

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, apiResponse.StatusCode);
            Assert.Contains("illegal", msg.ToLower());
        }

        #region helpers
        /// <summary>
        /// Returns HttpContent to be posted
        /// </summary>
        /// <param name="username">username added to header</param>
        /// <param name="password">string password, will be hashed before being added to header</param>
        /// <param name="fName">first name, sent in body in json obj</param>
        /// <param name="lName">last name, sent in body in json obj</param>
        /// <returns>StringContent with the params added to header and body</returns>
        private StringContent GetHttpContentWithNamesAndPass(string username, string password, string fName, string lName)
        {
            var hashedPassword = BankLedgerAPI.Utilities.HashUtil.HashPassword(password);
            var values = new Dictionary<string, string>
            {
              {"lname", lName}, {"fname", fName}
            };
            var json = JsonConvert.SerializeObject(values);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            content.Headers.Add("username", username);
            content.Headers.Add("password", Encoding.ASCII.GetString(hashedPassword));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return content;
        }

        /// <summary>
        /// Sets up and sends request
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="fName"></param>
        /// <param name="lName"></param>
        /// <returns>HttpResponseMessage from post to api endpoint</returns>
        private async Task<HttpResponseMessage> SetUpRequest(string username, string password, string fName, string lName)
        {
            var content = GetHttpContentWithNamesAndPass(username, password, fName, lName);
            var apiClient = new HttpClient();
            var response = await apiClient.PostAsync($"{ApiBaseUrl}/users/register", content);
            return response;
        }
        #endregion
    }
}
