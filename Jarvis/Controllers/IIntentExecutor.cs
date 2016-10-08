using Microsoft.Bot.Connector;
using Microsoft.Cognitive.LUIS;

namespace Jarvis
{
    public interface IIntentExecutor
    {
        string Execute(Intent intent, Activity activity);
    }
}