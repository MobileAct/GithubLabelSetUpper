using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using McMaster.Extensions.CommandLineUtils;
using Utf8Json;
using YamlDotNet.Serialization;

namespace GitlabLabelSetUpper
{
    [Command("list", Description = "List the all labels", ResponseFileHandling = ResponseFileHandling.ParseArgsAsLineSeparated)]
    public class ListCommand : BaseCommand
    {
        [Option("--host", Description = "Host of GitLab instance, default value is https://gitlab.com")]
        public string? Host { get; }

        [Option("-t|--token", Description = "Token of GitLab")]
        public string? Token { get; }

#pragma warning disable CS8618 // auto attach non-null value by CommandLineUtils
        [Option("-r|--repository", Description = "Target repository, value format: {Owner}/{RepositoryName}")]
        [Required]
        public string Repository { get; }
#pragma warning restore CS8618

        [Option("-o|--output", Description = "Output file name, including file extension")]
        public string? OutputFileName { get; }

        protected override async Task OnExecuteAsync(CommandLineApplication application)
        {
            string projectId = HttpUtility.UrlEncode(Repository);
            string environmentHost = Environment.GetEnvironmentVariable(Constant.EnvironmentHost);
            string environmentToken = Environment.GetEnvironmentVariable(Constant.EnvironmentToken);
            var gitlabApi = new GitlabApi(Host ?? environmentHost, Token ?? environmentToken, projectId);

            IReadOnlyList<Label> labels = await gitlabApi.GetLabelsAsync();

            Console.WriteLine($"{projectId} label are:");

            foreach(var label in labels)
            {
                Console.WriteLine(label.ToString());
            }

            if(OutputFileName is { })
            {
                outputLabels(labels, OutputFileName);
            }
        }

        private void outputLabels(IReadOnlyList<Label> labels, string outputFileName)
        {
            string content;

            if (outputFileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                byte[] json = JsonSerializer.Serialize(labels);
                content = JsonSerializer.PrettyPrint(json);
            }
            else if (outputFileName.EndsWith(".yml", StringComparison.OrdinalIgnoreCase) || outputFileName.EndsWith(".yaml", StringComparison.OrdinalIgnoreCase))
            {
                content = new SerializerBuilder().Build().Serialize(labels);
            }
            else
            {
                throw new ArgumentException("Not support file extension, now support json or yaml");
            }

            if (File.Exists(outputFileName))
            {
                Console.WriteLine("Output file name is already exists, do not override.");
                return;
            }

            File.WriteAllText(outputFileName, content);
        }
    }
}
