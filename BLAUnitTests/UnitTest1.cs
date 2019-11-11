using ServiceStack.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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
            var content = GetStringContentWithNamesAndPass(username, password, firstName, lastName);

            // TODO look into why using params from StringContent aren't being received in controller, then remove this workaround
            var apiResponse = await apiClient.PostAsync($"{ApiBaseUrl}/users/register?fname={firstName}&lname={lastName}", content);
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
            var apiClient = new HttpClient();
            //string request = "fname=" + firstName + "&lname=" + lastName;
            //var content = new StringContent(request, Encoding.UTF8);
            //content.Headers.Add("username", HttpUtility.UrlEncode(username));
            //content.Headers.Add("password", HttpUtility.UrlEncode(password));
            var content = GetStringContentWithNamesAndPass(username, password, firstName, lastName);
            var apiResponse = await apiClient.PostAsync($"{ApiBaseUrl}/users/register?fname={firstName}&lname={lastName}", content);
            var msg = await apiResponse.Content.ReadAsStringAsync();
            var msg2 = await content.ReadAsStringAsync();

            Assert.Equal(System.Net.HttpStatusCode.OK, apiResponse.StatusCode);
            Assert.Contains("created", msg.ToLower());
        }

        private StringContent GetStringContentWithNamesAndPass(string username, string password, string fname, string lname)
        {
            username = HttpUtility.UrlEncode(username);
            password = HttpUtility.UrlEncode(password);
            fname = HttpUtility.UrlEncode(fname);
            lname = HttpUtility.UrlEncode(lname);

            var values = new List<KeyValuePair<string, string>>();
            values.Add(new KeyValuePair<string, string>("fname", fname));
            values.Add(new KeyValuePair<string, string>("lname", lname));

            string request = "?fname=" + fname + "&lname=" + lname;
            var content = new StringContent(request, Encoding.UTF8);
            //var content = new FormUrlEncodedContent(values);
            content.Headers.Add("username", username);
            content.Headers.Add("password", password);
            //content.Headers.ContentType. application/x-www-form-urlencoded

            return content;
        }
    }
}
