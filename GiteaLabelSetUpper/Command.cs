using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace GiteaLabelSetUpper
{
    [Command(Name = "tealabel", ResponseFileHandling = ResponseFileHandling.ParseArgsAsLineSeparated)]
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
