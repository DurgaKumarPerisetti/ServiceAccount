using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Linq;

namespace ServiceAccount
{
    public static class Function1
    {
        [FunctionName("Function1")]

        public static async Task<IActionResult> Run(
 [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {

            string result;
            var leaddata = new JObject
            {
                { "name", req.Query["AccountName"].ToString() },
                { "telephone1", req.Query["Phone"].ToString() }
            };
            string serviceUrl = "https://maharaj009.api.crm8.dynamics.com";
            string clientId = "3b714e33-9c30-4c29-8a85-e2785f2adeb8";
            string secret = "7C~gBX9y~b.Kd9rBLBCCV~pwQ5BEtgl1Ar";

            /*string serviceUrl = " https://vijayr001.crm11.dynamics.com/";
            string clientId = "ab4fabff-bb2f-4e63-93a3-19e2651605b2";
            string secret = "ShHk5--u-mcbC2Yi88Vt-3Sphd5ogT_Adg";*/

            AuthenticationContext authContext = new AuthenticationContext("https://login.microsoftonline.com/3b08c2a3-9393-4248-9c25-daec0269ceda");
            //AuthenticationContext authContext = new AuthenticationContext("https://login.microsoftonline.com/e27f4651-4dbe-43b9-b260-79e8b7247aa1");
            ClientCredential credential = new ClientCredential(clientId, secret);

            AuthenticationResult result1 = await authContext.AcquireTokenAsync(serviceUrl, credential);

            string accessToken = result1.AccessToken;

            var httpclient = new HttpClient
            {
                BaseAddress = new Uri(serviceUrl),
                Timeout = new TimeSpan(0, 2, 0)
            };
            // httpclient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
            httpclient.DefaultRequestHeaders.Add("OData-Version", "4.0");
            httpclient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "api/data/v9.0/accounts/");
            request.Content = new StringContent(JsonConvert.SerializeObject(leaddata), Encoding.UTF8, "application/json");
            //request.Content=JsonConvert.SerializeObject(leaddata)

            HttpResponseMessage response = await httpclient.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                string leadURL = response.Headers.GetValues("OData-EntityId").FirstOrDefault();
                result = leadURL;
            }
            else
            {
                result = "Something went wrong";
            }

            return new OkObjectResult(result);
        }
    }
}
