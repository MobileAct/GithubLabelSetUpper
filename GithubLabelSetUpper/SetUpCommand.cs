using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Octokit;
using Utf8Json;
using YamlDotNet.Serialization;

namespace GithubLabelSetUpper
{
    [Command("setup", Description = "Setup Github labels by option value", ResponseFileHandling = ResponseFileHandling.ParseArgsAsLineSeparated)]
    public class SetUpCommand : BaseCommand
    {
        [Option("--host", Description = "Host of Github instance, default value is https://github.com")]
        public string? Host { get; }

        [Option("-t|--token", Description = "Token of Github")]
        [Required]
#pragma warning disable CS8618 // auto attach non-null value by CommandLineUtils
        public string Token { get; }
#pragma warning restore CS8618

        [Option("-r|--repository", Description = "Target repository, value format: {Owner}/{RepositoryName}")]
        [Required]
#pragma warning disable CS8618 // auto attach non-null value by CommandLineUtils
        public string Repository { get; }
#pragma warning restore CS8618

        [Option("-l|--label", Description = "The labels configuration file, support json or yml file.")]
        [FileExists]
        [Required]
#pragma warning disable CS8618 // auto attach non-null value by CommandLineUtils
        public string LabelsPath { get; }
#pragma warning restore CS8618

        [Option("-d|--dry-run", Description = "Calculate the required label changes, but do not apply Github")]
        public bool IsDryRun { get; }

        protected override async Task OnExecuteAsync(CommandLineApplication application)
        {
            (string owner, string repositoryName) = parseRepository();
            IGitHubClient github = Github.CreateClient(Token, Host);

            IReadOnlyList<Octokit.Label> repositoryLabels = await github.Issue.Labels.GetAllForRepository(owner, repositoryName);
            IList<Label> changingLabels = readLabels();
            IList<Change> changes = calculateChangeLabel(repositoryLabels, changingLabels);

            Console.WriteLine($"{owner}/{repositoryName} label will be:");

            foreach (var change in changes)
            {
                switch (change.Type)
                {
                    case Change.ChangeType.AddToRepository:
                        {
                            Console.WriteLine($"Add: {toLabelString(change.ChangingLabel)}");
                            break;
                        }
                    case Change.ChangeType.RemoveFromRepository:
                        {
                            Console.WriteLine($"Remove: {toOctokitLabelString(change.RepositoryLabel)}");
                            break;
                        }
                    case Change.ChangeType.Change:
                        {
                            Console.WriteLine($"Change: {toOctokitLabelString(change.RepositoryLabel)} ===> {toLabelString(change.ChangingLabel)}");
                            break;
                        }
                }
            }

            if (IsDryRun == false)
            {
                await pushChangeToGithubAsync(github, owner, repositoryName, changes);
            }
            else
            {
                Console.WriteLine("Dry Run: Not apply to Github.");
            }

            string toOctokitLabelString(Octokit.Label label)
            {
                return $"name: {label.Name}, color: {label.Color}, description: {label.Description}";
            }

            string toLabelString(Label label)
            {
                return $"name: {label.Name}, color: {label.Color}, description: {label.Description}";
            }
        }

        private (string owner, string repositoryName) parseRepository()
        {
            string[] ar = Repository.Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (ar.Length != 2)
            {
                throw new ArgumentException("Repository value is not containe {Owner}/{RepositoryName}");
            }

            return (ar[0], ar[1]);
        }

        private IList<Label> readLabels()
        {
            string content = File.ReadAllText(LabelsPath);

            IList<Label> labels;
            if (LabelsPath.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                labels = JsonSerializer.Deserialize<IList<Label>>(content);
            }
            else if (LabelsPath.EndsWith(".yml", StringComparison.OrdinalIgnoreCase) || LabelsPath.EndsWith(".yaml", StringComparison.OrdinalIgnoreCase))
            {
                labels = new DeserializerBuilder().Build().Deserialize<IList<Label>>(content);
            }
            else
            {
                throw new ArgumentException("Not supported file type, now support: json, yaml");
            }

            return labels;
        }

        private IList<Change> calculateChangeLabel(IReadOnlyList<Octokit.Label> repositoryLabels, IList<Label> changingLabels)
        {
            var result = new List<Change>();

            List<Octokit.Label> labelBag = new List<Octokit.Label>(repositoryLabels);

            foreach (var changingLabel in changingLabels)
            {
                Octokit.Label repositoryLabel = labelBag.FirstOrDefault(x => x.Name == changingLabel.Name);

                if (repositoryLabel != null)
                {
                    if (repositoryLabel.Color != changingLabel.Color || repositoryLabel.Description != changingLabel.Description)
                    {
                        result.Add(new Change { RepositoryLabel = repositoryLabel, ChangingLabel = changingLabel, Type = Change.ChangeType.Change });
                    }

                    labelBag.Remove(repositoryLabel);
                    continue;
                }

                repositoryLabel = labelBag.FirstOrDefault(x => changingLabel.Aliases?.Contains(x.Name) ?? false);

                if (repositoryLabel != null)
                {
                    result.Add(new Change { RepositoryLabel = repositoryLabel, ChangingLabel = changingLabel, Type = Change.ChangeType.Change });
                    labelBag.Remove(repositoryLabel);
                    continue;
                }

                result.Add(new Change { ChangingLabel = changingLabel, Type = Change.ChangeType.AddToRepository });
            }

            foreach (var repositoryLabel in labelBag)
            {
                result.Add(new Change { RepositoryLabel = repositoryLabel, Type = Change.ChangeType.RemoveFromRepository });
            }

            return result;
        }

        private async Task pushChangeToGithubAsync(IGitHubClient github, string owner, string repositoryName, IList<Change> changes)
        {
            foreach (var change in changes)
            {
                switch (change.Type)
                {
                    case Change.ChangeType.AddToRepository:
                        {
                            var newLabel = new NewLabel(change.ChangingLabel.Name, change.ChangingLabel.Color)
                            {
                                Description = change.ChangingLabel.Description
                            };
                            await github.Issue.Labels.Create(owner, repositoryName, newLabel);
                            break;
                        }
                    case Change.ChangeType.RemoveFromRepository:
                        {
                            await github.Issue.Labels.Delete(owner, repositoryName, change.RepositoryLabel.Name);
                            break;
                        }
                    case Change.ChangeType.Change:
                        {
                            var updateLabel = new LabelUpdate(change.ChangingLabel.Name, change.ChangingLabel.Color)
                            {
                                Description = change.ChangingLabel.Description
                            };
                            await github.Issue.Labels.Update(owner, repositoryName, change.RepositoryLabel.Name, updateLabel);
                            break;
                        }
                }
            }
        }

        private class Change
        {
            public enum ChangeType
            {
                AddToRepository,
                RemoveFromRepository,
                Change
            }

            /// <summary>
            /// Contains if Type = AddToRepository, Change
            /// </summary>
            /// <value>The changing label.</value>
            public Label? ChangingLabel { get; set; }

            /// <summary>
            /// Contains if Type = RemoveFromRepository, Change
            /// </summary>
            /// <value>The repository label.</value>
            public Octokit.Label? RepositoryLabel { get; set; }

            public ChangeType Type { get; set; }
        }
    }
}
