using System;
using System.Linq;
using System.Text;
using Microsoft.Cognitive.LUIS;
using Octokit;
using Activity = Microsoft.Bot.Connector.Activity;

namespace Jarvis
{
    [IntentExecutor("GetIssuesInfo")]
    public class GithubIssuesExecutor : IIntentExecutor
    {
        public string Execute(Intent intent, Activity activity)
        {
            var client = new GitHubClient(new ProductHeaderValue("Jarvis"));
            var tokenAuth = new Credentials("98b3dfccfd7a5475dc042f41e9c8c30bd599db7c");
            client.Credentials = tokenAuth;
            var request = new RepositoryIssueRequest();// { State = ItemStateFilter.Closed, Assignee = "timu" };
            var parameters = intent.Actions.First().Parameters;
            var userParam = parameters.FirstOrDefault(x => x.Name == "user");
            var userName = userParam?.ParameterValues?.FirstOrDefault(x => x.Type == "User")?.Entity;
            if (!string.IsNullOrWhiteSpace(userName))
            {
                request.Assignee = userName;
            }

            var statusParam = parameters.FirstOrDefault(x => x.Name == "status");
            var statusName = statusParam?.ParameterValues?.FirstOrDefault(x => x.Type == "Status")?.Entity;
            if (!string.IsNullOrWhiteSpace(statusName))
            {
                request.State = (ItemStateFilter) Enum.Parse(typeof(ItemStateFilter), statusName, true);
            }

            var issues = client.Issue.GetAllForRepository("HackandCraft", "LeviathanApi", 
                request,
                new ApiOptions() {PageSize = 10,PageCount = 1}).Result;
            if (issues.Count == 0)
            {
                return $"No issues found for {userName}";
            }

            var sb = new StringBuilder();
            foreach (var issue in issues)
            {
                sb.AppendLine($"{issue.HtmlUrl} - {issue.Title}<br/>");
            }

            return sb.ToString();
        }
    }
}