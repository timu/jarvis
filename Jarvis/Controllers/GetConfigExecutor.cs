using System.Collections.Generic;
using System.Linq;
using Microsoft.Bot.Connector;
using Microsoft.Cognitive.LUIS;

namespace Jarvis
{
    [IntentExecutor("GetConfig")]
    public class GetConfigExecutor : IIntentExecutor
    {
        public Dictionary<string, Dictionary<string, string>>
            data = new Dictionary<string, Dictionary<string, string>>()
                   {
                       {
                           "Dev", new Dictionary<string, string>()
                                  {
                                      {"Database", "sqlserver://serv.comee"},
                                      {"homeUrl", "http://server.com"},
                                      {"adminEmail", "jarvis@hackandcraft.com"},
                                  }
                       },{
                           "Demo", new Dictionary<string, string>()
                                  {
                                      {"Database", "sqlserver://servDemo.comee"},
                                      {"homeUrl", "http://serverDemo.com"},
                                      {"adminEmail", "jarviDemos@hackandcraft.com"},
                                  }
                       },{
                           "Production", new Dictionary<string, string>()
                                  {
                                      {"Database", "sqlserver://Production.comee"},
                                      {"homeUrl", "http://Production.com"},
                                      {"adminEmail", "jarvisProduction@hackandcraft.com"},
                                  }
                       },
                   };

        public string Execute(Intent intent, Activity activity)
        {
            var parameters = intent.Actions.First().Parameters;
            var keyParam = parameters.FirstOrDefault(x => x.Name == "configParameter");
            var configParam = keyParam?.ParameterValues?.FirstOrDefault(x => x.Type == "ConfigParameter")?.Entity;

            var envParam = parameters.FirstOrDefault(x => x.Name == "environment");
            var env = envParam?.ParameterValues?.FirstOrDefault(x => x.Type == "Environment")?.Entity;

            var envKey = data.Keys.FirstOrDefault(x => x == env) ??
                         data.Keys.FirstOrDefault(x => x.ToLower().Contains(env.ToLower()));

            if (envKey == null)
            {
                return "Environment '" + env + "' not found";
            }

            var configs = data[envKey];

            var configValues = configs.Where(x => x.Key.ToLower().Contains((configParam??"").ToLower())).Select(x => x.Key + ": " + x.Value).ToArray();

            return string.Join("<br/>", configValues);
        }
    }
}