using System.Linq;
using Core;

namespace GiteaLabelSetUpper
{
    public class LabelDifferenceProcessor : BaseLabelDifferenceProcessor<Label, GiteaApi>
    {
        public LabelDifferenceProcessor(GiteaApi api) : base(api) { }

        protected override bool IsSameLabel(Label label1, Label label2)
        {
            return label1.Name == label2.Name && label1.Color == label2.Color && label1.Description == label2.Description;
        }

        protected override bool IsEqualLabel(Label source, Label configuredLabel)
        {
            if (source.Id is long sourceId && configuredLabel.Id is long configuredId)
            {
                return sourceId == configuredId;
            }
            return source.Name == configuredLabel.Name || (configuredLabel.Aliases?.Contains(source.Name) ?? false);
        }
    }
}
