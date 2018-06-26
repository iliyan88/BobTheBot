using System.Threading.Tasks;
using BobTheBot.Entities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using SendGrid;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Dynamic;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using SendGrid.Helpers.Mail;
using BobTheBot.Kernel;
using RJ.Core;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace BobTheBot
{
    public class MessageService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMemoryCache memoryCache;
        private readonly IWordCache wordCache;
        private readonly ISendGridClient sendGridClient;
        BotCredentials botCredentials;
        private readonly IConfiguration configurationRoot;

        public MessageService(
            IUnitOfWork unitOfWork,
            IMemoryCache memoryCache,
            IOptions<BotCredentials> botCredentials,
            IWordCache wordCache,
            IConfiguration configurationRoot)
        {
            this.unitOfWork = unitOfWork;
            this.memoryCache = memoryCache;
            this.wordCache = wordCache;
            this.configurationRoot = configurationRoot;
            this.botCredentials = botCredentials.Value;
            this.sendGridClient = new SendGridClient(Environment.GetEnvironmentVariable("BOBTHEBOT_SENDGRID_EMAIL_APIKEY"));
        }




        public async Task<Result<System.Dynamic.ExpandoObject>> CheckSentences(Activity activity)
        {
            var wordsEntity = await wordCache.GetAsync();
            var words = wordsEntity.Select(x => x.Word);
            // Get the conversation id so the bot answers.
            var conversationId = activity.From.Id.ToString();

            // Get a valid token 
            string token = await this.GetBotApiToken();

            dynamic message = new ExpandoObject();
            // send the message back
            using (var client = new System.Net.Http.HttpClient())
            {
                // I'm using dynamic here to make the code simpler
                message.type = "message/text";
                message.text = "TEST TEST TEST";
                var senderEmail = Environment.GetEnvironmentVariable("BOBTHEBOT_SENDER_EMAIL");
                var senderName = Environment.GetEnvironmentVariable("BOBTHEBOT_SENDER_NAME");
                var recipientEmail = Environment.GetEnvironmentVariable("BOBTHEBOT_RECIPIENT_EMAIL");
                var recipientName = Environment.GetEnvironmentVariable("BOBTHEBOT_RECIPIENT_NAME");
                string messages = activity.Text.ToString();
                string from = activity.From.Name.ToString();
                string conv = activity.Conversation.Id;
                var group = conv.Split("|").Last().Split("\"").First();
                if (words.Any(c => messages.ToLower().Contains(c.ToLower())))
                {
                    //var msg = MailHelper.CreateSingleEmail(
                    //    new EmailAddress(senderEmail, senderName),
                    //    new EmailAddress("k652384@nwytg.com", recipientName),
                    //    "Subject",
                    //    "",
                    //    messages + " from " + from + " at " + group);

                    //var response2 = await sendGridClient.SendEmailAsync(msg);

                    // Set the toekn in the authorization header.
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var appCredentials = new MicrosoftAppCredentials(configurationRoot);
                    var connector = new ConnectorClient(new Uri(activity.ServiceUrl), appCredentials);
                   // var asd = await connector.Conversations.GetConversationMembersAsync(qwe);

                    //var newActovity = new Activity(id: "2cb6eb60-761b-11e8-b7ed-f79c3f4b062e",
                    //    serviceUrl:activity.ServiceUrl,
                    //    replyToId: "9aec9e40-7611-11e8-b7ed-f79c3f4b062e",
                    //    type:"text",
                    //    textFormat:"plain",
                    //    channelId:"emulator",
                    //    channelData: "9aec9e40-7611-11e8-b7ed-f79c3f4b062e");

                    //var qwfsv = activity.Recipient;
                    //activity.Recipient.Name = "Bot";
                    //activity.Recipient.Id = "bea98650-748a-11e8-a3ac-fb4c46d160e7";

                    var length = (activity.Text ?? string.Empty).Length;
                    var reply = activity.CreateReply($"You send {activity.Text} which was{length} characters!");
                    //var asd = new Activity(ChannelAccount(r reply.Recipient.Id == "bea98650-748a-11e8-a3ac-fb4c46d160e7";
                   // var asd = new Activity()
                    await connector.Conversations.ReplyToActivityAsync(reply);

                    //var asd = await connector.Conversations.CreateConversationAsync(new ConversationParameters());
                    //await client.PostAsJsonAsync(
                    //"http://localhost:33812/api/messages/c444ceb0-754f-11e8-a3ac-fb4c46d160e7|livechat",
                    //message as ExpandoObject);
                    ///conversations/c444ceb0-754f-11e8-a3ac-fb4c46d160e7|livechat/activities
                    //await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());

                    string trustServiceUri = "https://api.skypeforbusiness.com/platformservice/botframework";
                    MicrosoftAppCredentials.TrustServiceUrl(trustServiceUri);
                    //ConnectorClient connector = new ConnectorClient(new Uri(trustServiceUri));
                    List<ChannelAccount> participants = new List<ChannelAccount>();
                    participants.Add(new ChannelAccount("sip:yourAgentaccount@contoso.com", "Agent"));
                    ConversationParameters parameters = new ConversationParameters(true, new ChannelAccount("sip:yourBotaccount@contoso.com", "Bot"), participants, "TestTopic");
                    ConversationResourceResponse response = connector.Conversations.CreateConversationAsync(parameters).Result;

                    // await client.PostAsJsonAsync<ExpandoObject>(
                    //$"https://api.skype.net/v3/conversations/{conversationId}/activities",
                    //message as ExpandoObject);
                }
            }

            return Result.Ok(message);
        }

        /// <summary>
        /// Gets and caches a valid token so the bot can send messages.
        /// </summary>
        /// <returns>The token</returns>
        private async Task<string> GetBotApiToken()
        {
            // Check to see if we already have a valid token
            string token = memoryCache.Get("token")?.ToString();
            if (string.IsNullOrEmpty(token))
            {
                // we need to get a token.
                using (
                    var client = new System.Net.Http.HttpClient())
                {
                    // Create the encoded content needed to get a token
                    var parameters = new Dictionary<string, string>
                    {
                        {"client_id", this.botCredentials.ClientId },
                        {"client_secret", this.botCredentials.ClientSecret },
                        {"scope", "https://graph.microsoft.com/.default" },
                        {"grant_type", "client_credentials" }
                    };
                    var content = new System.Net.Http.FormUrlEncodedContent(parameters);

                    // Post
                    var response = await client.PostAsync("https://login.microsoftonline.com/common/oauth2/v2.0/token", content);

                    // Get the token response
                    var tokenResponse = await response.Content.ReadAsJsonAsync<Entities.TokenResponse>();

                    token = tokenResponse.access_token;

                    // Cache the token fo 15 minutes.
                    memoryCache.Set(
                        "token",
                        token,
                        new DateTimeOffset(DateTime.Now.AddMinutes(15)));
                }
            }

            return token;
        }
    }
}