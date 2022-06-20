using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Bootcamp
{
    public static class FunctionChaining
    {
        [FunctionName("FunctionChaining")]
        public static async Task<object> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            int invoiceId = context.GetInput<int>();

            try
            {
                var paidInvoice = await context.CallActivityAsync<object>("ProcessPayment", invoiceId);
                var taxReceipt = await context.CallActivityAsync<object>("GenerateTaxReceipt", paidInvoice);
                return await context.CallActivityAsync<object>("SendTaxReceipt", taxReceipt);
            }
            catch (System.Exception)
            {
                // Tratamento de exceções
                return null;
            }
        }

        [FunctionName("ProcessPayment")]
        public static string ProcessPayment([ActivityTrigger] int invoiceId, ILogger log)
        {
            log.LogInformation($"Processing payment for invoice #{invoiceId}.");
            // Add payment logic
            return $"Payment processed for invoice #{invoiceId}!";
        }

        [FunctionName("GenerateTaxReceipt")]
        public static string GenerateTaxReceipt([ActivityTrigger] string paidInvoice, ILogger log)
        {
            log.LogInformation($"{paidInvoice} -> Generating Tax Receipt");
            // Add NF logic here
            return $"{paidInvoice} -> Tax Receipt generated!";
        }

        [FunctionName("SendTaxReceipt")]
        public static string SendTaxReceipt([ActivityTrigger] string taxReceipt, ILogger log)
        {
            log.LogInformation($"{taxReceipt} -> Sending Tax Receipt");
            // Add logic to send NF here
            return $"{taxReceipt} -> Tax Receipt sent!";
        }

        [FunctionName("FunctionChaining_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "chaining/{invoiceId}")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            int invoiceId,
            ILogger log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("FunctionChaining", null, invoiceId);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}