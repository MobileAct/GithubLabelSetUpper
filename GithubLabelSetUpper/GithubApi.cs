using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Octokit;

namespace GithubLabelSetUpper
{
    public static class GithubExtensions
    {
        public static Label ToLabel(this Octokit.Label label)
        {
            return new Label
            {
                Name = label.Name,
                Description = label.Description,
                Color = label.Color
            };
        }
    }
    public class GithubApi : IApi<Label>
    {
        private const string defaultGithubHost = "https://github.com";
        private const string userAgent = nameof(GithubLabelSetUpper);

        private readonly IGitHubClient client;
        private readonly string owner;
        private readonly string repository;

        public GithubApi(string? githubHost, string? githubToken, string owner, string repository)
        {
            var client = new GitHubClient(new ProductHeaderValue(userAgent), new Uri(githubHost ?? defaultGithubHost));
            if (githubToken is { })
            {
                client.Credentials = new Credentials(githubToken);
            }
            this.client = client;
            this.owner = owner;
            this.repository = repository;
        }

        public async Task<IReadOnlyList<Label>> GetLabelsAsync()
        {
            IReadOnlyList<Octokit.Label> labels = await client.Issue.Labels.GetAllForRepository(owner, repository);
            return labels.Select(x => x.ToLabel()).ToList();
        }

        public async Task CreateLabelAsync(Label newLabel)
        {
            var request = new NewLabel(newLabel.Name, newLabel.Color)
            {
                Description = newLabel.Description
            };
            await client.Issue.Labels.Create(owner, repository, request);
        }

        public async Task DeleteLabelAsync(Label oldLabel)
        {
            await client.Issue.Labels.Delete(owner, repository, oldLabel.Name);
        }

        public async Task UpdateLabelAsync(Label oldLabel, Label newLabel)
        {
            var request = new LabelUpdate(newLabel.Name, newLabel.Color)
            {
                Description = newLabel.Description
            };
            await client.Issue.Labels.Update(owner, repository, oldLabel.Name, request);
        }
    }
}
