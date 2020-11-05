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
        [FunctionName("ServiceAccount")]

        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {

            string result;
            var leaddata = new JObject
            {
                { "name", req.Query["AccountName"].ToString()},
                { "telephone1", req.Query["Phone"].ToString()}
             };
            string serviceUrl = "https://perisett.crm8.dynamics.com/";
            string clientId = "70c01f55-b442-4e54-a9ec-45d4a31eb902";
            string secret = "y~F26M0JHpvu_r~b1AhXRmwJE33.Sla0A0";

            AuthenticationContext authContext = new AuthenticationContext("https://login.microsoftonline.com/decf26c1-386d-4581-857a-b0fdc57bfa07");
            ClientCredential credential = new ClientCredential(clientId, secret);

            AuthenticationResult result1 = await authContext.AcquireTokenAsync(serviceUrl, credential);

            string accessToken = result1.AccessToken;

            var httpclient = new HttpClient
            {
                BaseAddress = new Uri(serviceUrl),
                Timeout = new TimeSpan(0, 2, 0)
            };
            
            httpclient.DefaultRequestHeaders.Add("OData-Version", "4.0");
            httpclient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "api/data/v9.0/accounts/");
            request.Content = new StringContent(JsonConvert.SerializeObject(leaddata), Encoding.UTF8, "application/json");
           
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
