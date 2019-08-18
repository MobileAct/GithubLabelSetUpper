using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public abstract class BaseLabelDifferenceProcessor<TLabel, TApi> where TLabel : class, ILabel where TApi : IApi<TLabel>
    {
        private readonly TApi api;

        public BaseLabelDifferenceProcessor(TApi api)
        {
            this.api = api;
        }

        public IReadOnlyList<LabelChangeStrategy> Process(IReadOnlyList<TLabel> sourceLabels, IReadOnlyList<TLabel> configuredLabels)
        {
            var result = new List<LabelChangeStrategy>();
            var labelBag = new List<TLabel>(sourceLabels);

            foreach (var configuredLabel in configuredLabels)
            {
                TLabel foundLabel = labelBag.Find(x => x.Name == configuredLabel.Name || (configuredLabel.Aliases?.Contains(x.Name) ?? false));
                if (foundLabel is { })
                {
                    if (IsSameLabel(foundLabel, configuredLabel) is false)
                    {
                        result.Add(new LabelChangeStrategy.Update<TLabel, TApi>(api, foundLabel, configuredLabel));
                    }
                    labelBag.Remove(foundLabel);
                    continue;
                }

                result.Add(new LabelChangeStrategy.Add<TLabel, TApi>(api, configuredLabel));
            }

            foreach (var removeLabel in labelBag)
            {
                result.Add(new LabelChangeStrategy.Remove<TLabel, TApi>(api, removeLabel));
            }

            return result;
        }

        protected abstract bool IsSameLabel(TLabel label1, TLabel label2);
    }
}
