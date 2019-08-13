using System;
using Octokit;

namespace GithubLabelSetUpper
{
    public static class Github
    {
        private const string defaultGithubHost = "https://github.com";

        private const string userAgent = nameof(GithubLabelSetUpper);

        public static IGitHubClient CreateClient(string githubToken, string githubHost = defaultGithubHost)
        {
            return new GitHubClient(new ProductHeaderValue(userAgent), new Uri(githubHost))
            {
                Credentials = new Credentials(githubToken)
            };
        }
    }
}
