using System.Collections.Generic;

namespace RosettaUI
{
    /// <summary>
    /// A single line Field that combines multiple Fields
    /// </summary>
    public class CompositeFieldElement : ElementGroupWithHeader
    {
        public CompositeFieldElement(LabelElement label, IEnumerable<Element> contents) : base(label, contents)
        {
            label?.SetLabelTypeToPrefixIfAuto();
        }
    }
}