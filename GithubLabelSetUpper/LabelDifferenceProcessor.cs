﻿using System.Linq;
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

        protected override bool IsEqualLabel(Label source, Label configuredLabel)
        {
            return source.Name == configuredLabel.Name || (configuredLabel.Aliases?.Contains(source.Name) ?? false);
        }
    }
}
