using System.Net.Http;
using System.Threading.Tasks;
using BobTheBot;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Connector;
using RJ.Core;

namespace WebApplication.Controllers
{
    /// <summary>
    /// This controller will receive the skype messages and handle them to the EchoBot service. 
    /// </summary>
    [Route("api/[controller]")]
    public class MessagesController : Controller
    {

        private readonly MessageService messageService;

        public MessagesController(
            MessageService messageService)
        {
            this.messageService = messageService;
        }

        /// <summary>
        /// This method will be called every time the bot receives an activity. This is the messaging endpoint
        /// </summary>
        /// <param name="activity">The activity sent to the bot. I'm using dynamic here to simplify the code for the post</param>
        /// <returns>201 Created</returns>
        [HttpPost]
        public virtual async Task<Result<System.Dynamic.ExpandoObject>> Post([FromBody] Activity activity)
        {
            var result = await messageService.CheckSentences(activity);
            return result;
        }
    }
}