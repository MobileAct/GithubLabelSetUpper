using Core;

namespace GithubLabelSetUpper
{
    public class LabelDifferenceProcessor : BaseLabelDifferenceProcessor<Label, GithubApi>
    {
        public LabelDifferenceProcessor(GithubApi api) : base(api) { }

        protected override bool IsSameLabel(Label label1, Label label2)
        {
            return label1.Name == label2.Name && label1.Color == label2.Color && label1.Description == label2.Description;
        }
    }
}
