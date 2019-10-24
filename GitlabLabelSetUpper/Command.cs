using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace GitlabLabelSetUpper
{
    [Command(Name = "lablabel", ResponseFileHandling = ResponseFileHandling.ParseArgsAsLineSeparated)]
    [Subcommand(typeof(ListCommand))]
    [Subcommand(typeof(SetUpCommand))]
    public class Command : BaseCommand
    {
        protected override Task OnExecuteAsync(CommandLineApplication application)
        {
            application.ShowHelp();
            return Task.CompletedTask;
        }
    }
}
