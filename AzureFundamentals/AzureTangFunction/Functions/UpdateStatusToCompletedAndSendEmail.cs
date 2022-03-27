using System;
using System.Collections.Generic;
using System.Linq;
using AzureTangFunction.Data;
using AzureTangFunction.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace AzureTangFunction
{
    public class UpdateStatusToCompletedAndSendEmail
    {
        private readonly AzureTangDbContext _db;

        public UpdateStatusToCompletedAndSendEmail(AzureTangDbContext db)
        {
            _db = db;
        }


        [FunctionName("UpdateStatusToCompletedAndSendEmail")]
        public void Run([TimerTrigger("0 */2 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");


            IEnumerable<SalesRequest> salesRequestFromDb = _db.SalesRequests.Where(u => u.Status == "Image Processed");
            foreach (SalesRequest salesRequest in salesRequestFromDb)
            {
                salesRequest.Status = "Completed";
            }

            _db.UpdateRange(salesRequestFromDb);
            _db.SaveChanges();
        }
    }
}
