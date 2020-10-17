using BankLedgerAPI.Models;
using BankLedgerAPI.Utilities;
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
    public class AccountControllerUnitTests
    {
        public string IdentityBaseUrl { get; private set; }
        public string ApiBaseUrl { get; private set; }
        public string EndpointOpenAccount { get; private set; }
        public string EndpointRegister { get; private set; }
        public string EndpointLogin { get; private set; }
        public string EndpointLogout { get; private set; }

        public AccountControllerUnitTests()
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

            EndpointOpenAccount = $"{ApiBaseUrl}/account/open";
            EndpointRegister = $"{ApiBaseUrl}/users/register";
            EndpointLogin = $"{ApiBaseUrl}/session/login";
            EndpointLogout = $"{ApiBaseUrl}/session/logout";
        }

        [Theory]
        [InlineData("mychecking", "checking")]
        [InlineData("mychecking123", "checking")]
        [InlineData("mysav123", "checking")]
        [InlineData("1stacct", "checking")]
        [InlineData("1", "checking")]
        [InlineData("mychecking", "Checking")]
        [InlineData("mychecking123", "cHECKing")]
        [InlineData("1stacct", "0")]
        [InlineData("1", "checkinG")]
        [InlineData("mysavings", "savings")]
        [InlineData("sav", "savings")]
        [InlineData("sav1", "savings")]
        [InlineData("2", "savings")]
        [InlineData("chk2", "savings")]
        [InlineData("mysavings", "1")]
        [InlineData("sav", "sAVings")]
        [InlineData("sav1", "Savings")]
        [InlineData("2", "sAVINGS")]
        [InlineData("chk2", "1")]
        public async Task Account_OpenAccount_ReturnOK(string accountNickname, string type)
        {
            string firstName = "Sleve";
            string lastName = "McDichael";
            string username = "smcdichael";
            string password = "1234";

            username += TestUtils.GenerateRandomString(Settings.UsernameMinLength, Settings.ZeroCharVal, Settings.NineCharVal);

            var registrationResponse = await TestUtils.SetUpRegistrationRequest(EndpointRegister, username, password, firstName, lastName);
            var loginResponse = await TestUtils.SetUpPostRequestWithUsernameAndPassword(EndpointLogin, username, password);
            System.Threading.Thread.Sleep(500);
            Assert.Equal(System.Net.HttpStatusCode.OK, loginResponse.StatusCode);
            Assert.True(loginResponse.Headers.Contains("Authorization"));

            var token = loginResponse.Headers.GetValues("Authorization").FirstOrDefault();
            Dictionary<string, string> header = new Dictionary<string, string>();
            header.Add("accountNickname", accountNickname);
            header.Add("type", type);

            var accountResponse = await TestUtils.SetUpPostRequestWithHeaderAndAuth(EndpointOpenAccount, String.Empty, token, header);
            Assert.Equal(System.Net.HttpStatusCode.OK, accountResponse.StatusCode);

            var logoutResponse = await TestUtils.SetUpRequestWithAuth(EndpointLogout, token + "modifiedtokenstringhere", "");
        }
    }
}
