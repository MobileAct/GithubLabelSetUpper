using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using Core;
using McMaster.Extensions.CommandLineUtils;
using Utf8Json;
using YamlDotNet.Serialization;

namespace GitlabLabelSetUpper
{
    [Command("setup", Description = "Setup GitLab labels by option value", ResponseFileHandling = ResponseFileHandling.ParseArgsAsLineSeparated)]
    public class SetUpCommand : BaseCommand
    {
        [Option("--host", Description = "Host of GitLab instance, default value is https://gitlab.com")]
        public string? Host { get; }

        [Option("-t|--token", Description = "Token of GitLab")]
        public string? Token { get; }

#pragma warning disable CS8618 // auto attach non-null value by CommandLineUtils
        [Option("-r|--repository", Description = "Target repository, value format: {Owner}/{RepositoryName}")]
        [Required]
        public string Repository { get; }

        [Option("-l|--label", Description = "The labels configuration file, support json or yml file.")]
        [FileExists]
        [Required]
        public string LabelsPath { get; }
#pragma warning restore CS8618

        [Option("-d|--dry-run", Description = "Calculate the required label changes, but do not apply GitLab")]
        public bool IsDryRun { get; }

        protected override async Task OnExecuteAsync(CommandLineApplication application)
        {
            string projectId = HttpUtility.UrlEncode(Repository);
            string environmentHost = Environment.GetEnvironmentVariable(Constant.EnvironmentHost);
            string environmentToken = Environment.GetEnvironmentVariable(Constant.EnvironmentToken);
            var gitlabApi = new GitlabApi(Host ?? environmentHost, Token ?? environmentToken, projectId);
            var labelDifferenceProcessor = new LabelDifferenceProcessor(gitlabApi);

            IReadOnlyList<Label> repositoryLabels = await gitlabApi.GetLabelsAsync();
            IReadOnlyList<Label> configuredLabels = readLabels();
            IReadOnlyList<LabelChangeStrategy> labelChangeStrategies = labelDifferenceProcessor.Process(repositoryLabels, configuredLabels);

            Console.WriteLine($"{projectId} label will be:");

            foreach(var labelChangeStrategy in labelChangeStrategies)
            {
                Console.WriteLine(labelChangeStrategy.ToString());
            }

            if(IsDryRun is false)
            {
                foreach(var labelChangeStrategy in labelChangeStrategies)
                {
                    await labelChangeStrategy.ChangeLabelAsync();
                }
            }
            else
            {
                Console.WriteLine("Dry Run: Not apply to Gitlab.");
            }
        }

        private IReadOnlyList<Label> readLabels()
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

            foreach (var label in labels)
            {
                label.ValidateOrThrow();
            }

            return (IReadOnlyList<Label>)labels;
        }
    }
}
