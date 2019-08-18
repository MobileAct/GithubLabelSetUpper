using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core
{
    public interface IApi<TLabel> where TLabel : ILabel
    {
        public Task<IReadOnlyList<TLabel>> GetLabelsAsync();

        public Task CreateLabelAsync(TLabel newLabel);

        public Task UpdateLabelAsync(TLabel oldLabel, TLabel newLabel);

        public Task DeleteLabelAsync(TLabel oldLabel);
    }
}
