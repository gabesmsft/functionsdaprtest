using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Dapr.Client;

namespace MyFunctionProj
{
    public static class HttpTrigger1
    {
        [FunctionName("HttpTrigger1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "mybindingforinput")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = String.Empty;
            string unescaped = "";
            using (var streamReader = new StreamReader(req.Body))
            {
                var body = await streamReader.ReadToEndAsync();
                string jsonString = JsonSerializer.Serialize(body);

                unescaped = System.Text.RegularExpressions.Regex.Unescape(jsonString);

                log.LogInformation("Message or request data: {0}", unescaped);

                //The following lines of code use DaprClient SDK to make a request to the Dapr binding route for mybindingforoutput, which will invoke the specified output binding.
                var daprClient = new DaprClientBuilder().Build();
                await daprClient.InvokeBindingAsync("mybindingforoutput", "create", unescaped);

            }

            return new OkObjectResult(unescaped);
        }
    }
}
