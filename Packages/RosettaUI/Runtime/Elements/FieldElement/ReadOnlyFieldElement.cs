﻿namespace RosettaUI
{
    /// <summary>
    /// 値を持ち外部と同期するFieldElement
    /// ラベル付きのReadOnlyValueElement
    /// </summary>
    public abstract class ReadOnlyFieldElement<T> : ReadOnlyValueElement<T>
    {
        public readonly LabelElement label;

        protected ReadOnlyFieldElement(LabelElement label, IGetter<T> getter) : base(getter)
        {
            if (label != null)
            {
                label.SetLabelTypeToPrefixIfAuto();
                
                this.label = label;
                
                AddChild(this.label);
            }
        }
    }
}