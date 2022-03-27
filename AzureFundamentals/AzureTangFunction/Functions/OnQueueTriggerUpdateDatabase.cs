using System;
using AzureTangFunction.Data;
using AzureTangFunction.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace AzureTangFunction
{
    public class OnQueueTriggerUpdateDatabase
    {
        private readonly AzureTangDbContext _db;

        public OnQueueTriggerUpdateDatabase(AzureTangDbContext db)
        {
            _db = db;
        }

        [FunctionName("OnQueueTriggerUpdateDatabase")]
        public void Run([QueueTrigger("SalesRequestInBound", Connection = "AzureWebJobsStorage")]SalesRequest myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
            //quando um item é adicionado na fila com o nome dessa queue
            myQueueItem.Status = "Submiteed";
            _db.SalesRequests.Add(myQueueItem);
            _db.SaveChanges();
        }
    }
}
