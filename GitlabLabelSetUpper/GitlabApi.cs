using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core;
using GitLabApiClient;
using GitLabApiClient.Internal.Paths;
using GitLabApiClient.Models.Projects.Requests;

namespace GitlabLabelSetUpper
{
    public static class GitlabExtensions
    {
        public static Label ToLabel(this GitLabApiClient.Models.Projects.Responses.Label label)
        {
            return new Label
            {
                Id = label.Id,
                Name = label.Name,
                Description = label.Description,
                Color = label.Color,
                Priority = label.Priority
            };
        }
    }

    public class GitlabApi : IApi<Label>
    {
        private const string defaultGitlabHost = "https://gitlab.com";

        private readonly GitLabClient client;
        private readonly ProjectId projectId;

        public GitlabApi(string? gitlabHost, string? gitlabToken, string projectId)
        {
            client = new GitLabClient(gitlabHost ?? defaultGitlabHost, gitlabToken ?? string.Empty);
            this.projectId = projectId;
        }

        public async Task<IReadOnlyList<Label>> GetLabelsAsync()
        {
            var response = await client.Projects.GetLabelsAsync(projectId);
            return response.Select(x => x.ToLabel()).ToList();
        }

        public async Task CreateLabelAsync(Label newLabel)
        {
            var request = new CreateProjectLabelRequest(newLabel.Name)
            {
                Color = newLabel.Color,
                Description = newLabel.Color,
                Priority = newLabel.Priority
            };
            await client.Projects.CreateLabelAsync(projectId, request);
        }

        public async Task DeleteLabelAsync(Label oldLabel)
        {
            await client.Projects.DeleteLabelAsync(projectId, oldLabel.Name);
        }

        public async Task UpdateLabelAsync(Label oldLabel, Label newLabel)
        {
            var request = UpdateProjectLabelRequest.FromNewName(oldLabel.Name, newLabel.Name);
            request.Color = newLabel.Color;
            request.Description = newLabel.Description;
            request.Priority = newLabel.Priority;
            await client.Projects.UpdateLabelAsync(projectId, request);
        }
    }
}
