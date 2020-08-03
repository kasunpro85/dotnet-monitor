using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace FunctionApp1
{
    public static class Function1
    {
        public static List<RegisterVal> listRegisterVal = new List<RegisterVal>();
        public static List<PollingVal> listpollingVal = new List<PollingVal>();

        [FunctionName("Function1")]
        public static void Run([TimerTrigger("%CroneTime%")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            try { new LogApplicationService().SendLog(); }
            catch (Exception ex)
            {
                log.LogInformation(ex.ToString() + " " + DateTime.Now);
                Function1.listpollingVal.Add(new PollingVal { isActive = false });
                Function1.listRegisterVal.Add(new RegisterVal { isActive = false });
                //new LogApplicationService().PostList();
            }
        }
    }
}
