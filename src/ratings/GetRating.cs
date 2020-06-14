using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ratings
{    
    public static class GetRating
    {
        [FunctionName("GetRating")]        
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetRating/{ratingId}")] HttpRequest req,
            [CosmosDB(
                databaseName: "icecream",
                collectionName: "ratings",
                ConnectionStringSetting = "CosmosDBConnection",
                SqlQuery = "SELECT * FROM c WHERE c.id={ratingId}")]
                IEnumerable<RatingEntity> document,            
            ILogger log,
            string ratingId)
        {
            log.LogInformation("C# HTTP trigger function GetRating processed a request.");
            if (document == null)
            {
                return new NotFoundResult();
            }
            await Task.Delay(0);
            return new OkObjectResult(document);
        }
    }
}
