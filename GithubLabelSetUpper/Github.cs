using System;
using Octokit;

namespace GithubLabelSetUpper
{
    public static class Github
    {
        private const string defaultGithubHost = "https://github.com";

        private const string userAgent = nameof(GithubLabelSetUpper);

        public static IGitHubClient CreateClient(string? githubToken, string? githubHost)
        {
            var client = new GitHubClient(new ProductHeaderValue(userAgent), new Uri(githubHost ?? defaultGithubHost));
            if (githubToken! is null)
            {
                client.Credentials = new Credentials(githubToken);
            }
            return client;
        }
    }
}
