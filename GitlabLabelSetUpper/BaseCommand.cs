using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace GitlabLabelSetUpper
{
    [HelpOption("--help|-h")]
    public abstract class BaseCommand
    {
        protected abstract Task OnExecuteAsync(CommandLineApplication application);
    }
}
