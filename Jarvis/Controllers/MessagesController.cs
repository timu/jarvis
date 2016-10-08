using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Microsoft.Cognitive.LUIS;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Extensions;

namespace Jarvis
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private static Lazy<Dictionary<string, Type>> intentExecutors =
            new Lazy<Dictionary<string, Type>>(() => AppDomain.CurrentDomain.GetAssemblies()
                                                 .SelectMany(x => x.GetTypes())
                                                 .Where(
                                                     x =>
                                                             x.GetAttribute<IntentExecutorAttribute>() != null)
                                                 .ToDictionary(x => x.GetAttribute<IntentExecutorAttribute>().Name, x => x));

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message && activity.Text.Contains("@jarvis2"))
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

                activity.Text = activity.Text.Replace("@jarvis2", "");

                var client = new RestClient("https://api.projectoxford.ai/luis/v1/application?id=02139510-4d03-4abe-bf4d-fac6c4b00090&subscription-key=6d53353c60f047f1b7761f51d721df64&q=" + HttpUtility.UrlEncode(activity.Text) + "&timezoneOffset=0.0");
                var request = new RestRequest(Method.GET);
                request.AddHeader("cache-control", "no-cache");
                IRestResponse webresponse = client.Execute(request);

                var data = JsonConvert.DeserializeObject<JToken>(webresponse.Content);
                var luisResult = new LuisResult(data);
                foreach (var intent in luisResult.Intents)
                {
                    //Activity nureply = activity.CreateReply(intent.Name + intent.Score);
                    //await connector.Conversations.ReplyToActivityAsync(nureply);
                }


                var firstIntent = luisResult.Intents.FirstOrDefault();
                if (firstIntent == null)
                {
                    Activity nureply = activity.CreateReply($"You sent {activity.Text} which was not understood");
                    await connector.Conversations.ReplyToActivityAsync(nureply);
                    return Request.CreateResponse(HttpStatusCode.OK);
                }

                Type intentType;
                if (!intentExecutors.Value.TryGetValue(firstIntent.Name, out intentType))
                {
                    Activity nureply = activity.CreateReply($"You sent {activity.Text} which was not understood");
                    await connector.Conversations.ReplyToActivityAsync(nureply);
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                string replyText;
                try
                {
                    var intentExecutor = (IIntentExecutor) Activator.CreateInstance(intentType);
                    replyText = intentExecutor.Execute(firstIntent, activity);
                }
                catch (Exception e)
                {
                    replyText = e.Message + e.StackTrace;
                }
                // return our reply to the user
                Activity reply = activity.CreateReply(replyText);
                await connector.Conversations.ReplyToActivityAsync(reply);
                var response = Request.CreateResponse(HttpStatusCode.OK);
                return response;
            }

            return null;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}