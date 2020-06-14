using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
namespace ratings
{
    public static class CreateRating
    {
        static HttpClient httpClient = new HttpClient();  

        [FunctionName("CreateRating")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "icecream",
                collectionName: "ratings",
                ConnectionStringSetting = "CosmosDBConnection")]
                out dynamic document,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function CreateRating processed a request.");
            string ProductServiceUrl = System.Environment.GetEnvironmentVariable("ProductServiceUrl");
            string UserServiceUrl = System.Environment.GetEnvironmentVariable("UserServiceUrl");
            string requestBody = new StreamReader(req.Body).ReadToEndAsync().Result;
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            document = new {};
            if (data.rating < 0 || data.rating > 5)
            {
                return new BadRequestObjectResult("Ratings should be between 0 and 5");
            }
            string r = string.Empty;
            try
            {
                r = httpClient.GetStringAsync(ProductServiceUrl + "?productId=" + data.productId).Result;
                log.LogInformation(r);
            }
            catch(Exception e)
            {
                return new BadRequestObjectResult(e.InnerException.Message);
            }
            try
            {
                r = httpClient.GetStringAsync(UserServiceUrl + "?userId=" + data.userId).Result;
                log.LogInformation(r);
            }
            catch(Exception e)
            {
                return new BadRequestObjectResult(e.InnerException.Message);
            }
            document = new {userId = data.userId, productId = data.productId, locationName = data.locationName, rating = data.rating, timestamp = DateTime.UtcNow, id = Guid.NewGuid(), userNotes = data.userNotes };
            return new OkObjectResult(document);
        }
    }
}
