using BlackjackBot.Bot;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace BlackjackBot.WorkerRoleHost
{
    public class WorkerRole : RoleEntryPoint
    {
        CustomBot bot;
        bool firstTime = true;

        string url = "http://blackjackbotserver.azurewebsites.net/";

        public override void Run()
        {
            RunAsync().Wait();
            // Our Run method should always block.            
        }

                async private Task RunAsync()
        {

            while (true)
            {
                if (firstTime)
                {
                    firstTime = false;
  
                    bot = new CustomBot();
                
                    //start another game
                    await bot.InitializeAsync(url);
                    //await bot.StartSoloGameSeriesAsync(10);
                    await bot.JoinMultiPlayerGameQueueAsync(); 
                    
                    string result = await bot.CallEchoTest("Hello World from Worker Role!");
                    Trace.TraceInformation(result); 
                }
                
                await Task.Delay(5000);
                Trace.TraceInformation("Working...");
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }
    }
}
