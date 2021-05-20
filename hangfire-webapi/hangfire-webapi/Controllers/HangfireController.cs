using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hangfire_webapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HangfireController : ControllerBase
    {
        //Fire and Forget jobs: Jobs executados uma única vez, usando o método Enqueue. Por exemplo, serve para mandar um e-mail de boas vindas
        [HttpPost]
        [Route("[action]")]
        public IActionResult Welcome()
        {
            var jobId = BackgroundJob.Enqueue(() => SendWelcomeEmail("Welcome to our app"));

            return Ok($"Job ID: {jobId}. Welcome email sent to the user");
        }

        //Delayed Jobs: Jobs executados uma vez também, porém um tempo depois de acionados, tempo esse determinado. Por exemplo, mandar um desconto para o 
        //usuário algum tempo depois dele ter se inscrito
        [HttpPost]
        [Route("[action]")]
        public IActionResult Discount()
        {
            int timeInSeconds = 30;
            var jobId = BackgroundJob.Schedule(() => SendWelcomeEmail("Welcome to our app"), TimeSpan.FromSeconds(timeInSeconds));

            return Ok($"Job ID: {jobId}. Discount email will be sent in {timeInSeconds} seconds!");
        }

        //Recurring jobs: Jobs que rodam a cada x tempo, seja para enviar e-mails de news, relatórios mensais, monitoramento de database e etc.
        [HttpPost]
        [Route("[action]")]
        public IActionResult DatabaseUpdate()
        {
            RecurringJob.AddOrUpdate(() => Console.WriteLine("Database updated"), Cron.Minutely);
            return Ok("Database check job initiated!");
        }

        //Countinous jobs: Jobs que são executados depois de algum outro, por exemplo, quando um usuário que fazer unsubscribe, e você manda um e-mail para ele depois
        //confirmando que ele não receberá mais e-mails. Esse tipo de job depende que outro execute com sucesso para ser acionado
        [HttpPost]
        [Route("[action]")]
        public IActionResult Confirm()
        {
            int timeInSeconds = 30;
            var parentJobId = BackgroundJob.Schedule(() => Console.WriteLine("You asked to be unsubscribed!"), TimeSpan.FromSeconds(timeInSeconds));
            BackgroundJob.ContinueJobWith(parentJobId, () => Console.WriteLine("You were unsubscribed!"));

            return Ok("Confirmation job created!");
        }

        public void SendWelcomeEmail(string text)
        {
            Console.WriteLine(text);
        }
    }
}
