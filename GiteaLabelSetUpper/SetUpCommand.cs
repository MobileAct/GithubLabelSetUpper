﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using Core;
using McMaster.Extensions.CommandLineUtils;
using Utf8Json;
using YamlDotNet.Serialization;

namespace GiteaLabelSetUpper
{
    [Command("setup", Description = "Setup Gitea labels by option value", ResponseFileHandling = ResponseFileHandling.ParseArgsAsLineSeparated)]
    public class SetUpCommand : BaseCommand
    {
        [Option("--host", Description = "Host of Gitea instance, default value is https://try.gitea.io")]
        public string? Host { get; }

        [Option("-t|--token", Description = "Token of Gitea")]
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

        [Option("-d|--dry-run", Description = "Calculate the required label changes, but do not apply Gitea")]
        public bool IsDryRun { get; }

        protected override async Task OnExecuteAsync(CommandLineApplication application)
        {
            (string owner, string repositoryName) = parseRepository();
            string environmentHost = Environment.GetEnvironmentVariable(Constant.EnvironmentHost);
            string environmentToken = Environment.GetEnvironmentVariable(Constant.EnvironmentToken);
            var giteaApi = new GiteaApi(Host ?? environmentHost, Token ?? environmentToken, owner, repositoryName);
            var labelDifferenceProcessor = new LabelDifferenceProcessor(giteaApi);

            IReadOnlyList<Label> repositoryLabels = await giteaApi.GetLabelsAsync();
            IReadOnlyList<Label> configuredLabels = readLabels();
            IReadOnlyList<LabelChangeStrategy> labelChangeStrategies = labelDifferenceProcessor.Process(repositoryLabels, configuredLabels);

            Console.WriteLine($"{owner}/{repositoryName} label will be:");

            foreach (var labelChangeStrategy in labelChangeStrategies)
            {
                Console.WriteLine(labelChangeStrategy.ToString());
            }

            if (IsDryRun is false)
            {
                foreach (var labelChangeStrategy in labelChangeStrategies)
                {
                    await labelChangeStrategy.ChangeLabelAsync();
                }
            }
            else
            {
                Console.WriteLine("Dry Run: Not apply to Gitea.");
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
