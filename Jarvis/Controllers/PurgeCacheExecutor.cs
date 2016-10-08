using Microsoft.Bot.Connector;
using Microsoft.Cognitive.LUIS;

namespace Jarvis
{
    [IntentExecutor("PurgeCache")]
    public class PurgeCacheExecutor : IIntentExecutor
    {
        public string Execute(Intent intent, Activity activity)
        {
            return "Purged by executing DELETE /zones/:identifier/purge_cache";
        }
    }
}