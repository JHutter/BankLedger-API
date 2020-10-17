using BankLedgerAPI.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BLAUnitTests
{
    public static class TestUtils
    {

        /// <summary>
        /// Returns HttpContent to be posted
        /// </summary>
        /// <param name="username">username added to header</param>
        /// <param name="password">string password, will be hashed before being added to header</param>
        /// <param name="fName">first name, sent in body in json obj</param>
        /// <param name="lName">last name, sent in body in json obj</param>
        /// <returns>StringContent with the params added to header and body</returns>
        public static StringContent GetHttpContentWithNamesAndPass(string username, string password, string fName, string lName)
        {
            var hashedPassword = BankLedgerAPI.Utilities.HashUtils.HashPassword(password);
            var values = new Dictionary<string, string>
            {
              {"lname", lName}, {"fname", fName}
            };
            var json = JsonConvert.SerializeObject(values);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            content.Headers.Add("username", username);
            content.Headers.Add("password", Encoding.ASCII.GetString(hashedPassword)); // Convert.ToBase64String(System.Net.WebUtility.UrlEncodeToBytes(hashedPassword, 0, hashedPassword.Length)));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return content;
        }

        /// <summary>
        /// Returns HttpContent to be posted
        /// </summary>
        /// <param name="username">username added to header</param>
        /// <param name="password">string password, will be hashed before being added to header</param>
        /// <returns>StringContent with the params added to header and body</returns>
        public static StringContent GetHttpContentWithUserAndPass(string username, string password, string httpContent)
        {
            byte[] hashedPassword = BankLedgerAPI.Utilities.HashUtils.HashPassword(password);
            var content = new StringContent(httpContent);

            content.Headers.Add("username", username);
            content.Headers.Add("password", Encoding.ASCII.GetString(hashedPassword)); // Convert.ToBase64String(System.Net.WebUtility.UrlEncodeToBytes(hashedPassword, 0, hashedPassword.Length)));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return content;
        }

        /// <summary>
        /// Sets up and sends request
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="fName"></param>
        /// <param name="lName"></param>
        /// <returns>HttpResponseMessage from post to api endpoint</returns>
        public static async Task<HttpResponseMessage> SetUpRegistrationRequest(string endpoint, string username, string password, string fName, string lName)
        {
            var content = GetHttpContentWithNamesAndPass(username, password, fName, lName);
            var apiClient = new HttpClient();
            var response = await apiClient.PostAsync(endpoint, content);
            return response;
        }

        /// <summary>
        /// Sets up and sends request
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="fName"></param>
        /// <param name="lName"></param>
        /// <returns>HttpResponseMessage from post to api endpoint</returns>
        public static async Task<HttpResponseMessage> SetUpRequestWithAuth(string endpoint, string token, string httpContent)
        {
            var apiClient = new HttpClient();
          
            apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await apiClient.PostAsync(endpoint, new StringContent(httpContent));
            return response;
        }

        // <summary>
        /// Sets up and sends request
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="fName"></param>
        /// <param name="lName"></param>
        /// <returns>HttpResponseMessage from post to api endpoint</returns>
        public static async Task<HttpResponseMessage> SetUpPostRequestWithUsernameAndPassword(string endpoint, string username, string password)
        {
            var content = GetHttpContentWithUserAndPass(username, password, "");
            var apiClient = new HttpClient();
            var response = await apiClient.PostAsync(endpoint, content);
            return response;
        }

        // <summary>
        /// Sets up and sends request
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="fName"></param>
        /// <param name="lName"></param>
        /// <returns>HttpResponseMessage from post to api endpoint</returns>
        public static async Task<HttpResponseMessage> SetUpSimplePostRequest(string endpoint, string httpContent)
        {
            var apiClient = new HttpClient();
            var response = await apiClient.PostAsync(endpoint, new StringContent(httpContent));
            return response;
        }

        public static async Task<HttpResponseMessage> SetUpPostRequestWithHeaderAndAuth(string endpoint, string httpContent, string token, Dictionary<string, string> header)
        {
            var apiClient = new HttpClient();
            apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            foreach (var item in header)
            {
                apiClient.DefaultRequestHeaders.Add(item.Key, item.Value);
            }
                
            var response = await apiClient.PostAsync(endpoint, new StringContent(httpContent));
            return response;
        }

        public static string GenerateRandomString(int length, int min, int max)
        {
            string generated = string.Empty;
            var random = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < length; i++)
            {
                char newChar = (char)random.Next(min, max);
                generated += newChar;
            }
            return generated;
        }

        public static string GenerateRandomNumericPostfix()
        {
            return GenerateRandomString(Settings.UsernameMinLength, Settings.ZeroCharVal, Settings.NineCharVal);
        }
    }
}
