using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace GitlabLabelSetUpper
{
    public class Program
    {
        public static Task<int> Main(string[] args) => CommandLineApplication.ExecuteAsync<Command>(args);
    }
}
