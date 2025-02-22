﻿using UnityEngine.UIElements;

namespace RosettaUI.UIToolkit.Builder
{
    public partial class UIToolkitBuilder
    {
        VisualElement Build_Label(Element element)
        {
            var labelElement = (LabelElement) element;
            var label = new Label(labelElement.Value);
            label.ListenValue(labelElement);
            
            return label;
        }
        
        void SetupFieldLabel<T, TElementValue>(BaseField<T> field, ReadOnlyFieldElement<TElementValue> fieldBaseElement)
        {
            var labelElement = fieldBaseElement.label;
            if (labelElement != null)
            {
                field.ListenLabel(labelElement);

                SetupUIObj(labelElement, field.labelElement);
            }
        }
    }
}