using Microsoft.Cognitive.LUIS;
using Newtonsoft.Json;
using Activity = Microsoft.Bot.Connector.Activity;

namespace Jarvis
{
    [IntentExecutor("Get_projectInfo")]
    public class ProjectInfoExecutor : IIntentExecutor
    {
        public string Execute(Intent intent, Activity activity)
        {
            return "Something  about " + activity.Conversation.Name;
        }
    }
}