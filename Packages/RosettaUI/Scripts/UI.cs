﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace RosettaUI
{
    /// <summary>
    /// Top level interface
    /// </summary>
    public static class UI
    {
        #region Button

        public static ButtonElement Button(LabelElement label, Action onClick)
        {
            return new ButtonElement(label?.getter, onClick);
        }

        #endregion


        private static void SetInteractableWithBinder(Element element, IBinder binder)
        {
            element.interactable = !binder.IsReadOnly;
        }

        #region Label

        public static LabelElement Label(LabelElement label)
        {
            return label;
            // use implicit operator of Label
        }

        public static LabelElement Label(Func<string> readLabel)
        {
            return readLabel;
        }

        #endregion


        #region Field

        public static Element Field<T>(Expression<Func<T>> targetExpression, Action<T> onValueChanged = null)
        {
            return Field(ExpressionUtility.CreateLabelString(targetExpression), targetExpression, onValueChanged);
        }

        public static Element Field<T>(LabelElement label, Expression<Func<T>> targetExpression,
            Action<T> onValueChanged = null)
        {
            var binder = ExpressionUtility.CreateBinder(targetExpression);
            if (binder == null) return null;
            binder.onValueChanged += onValueChanged;

            return Field(label, binder);
        }

        public static Element Field(LabelElement label, IBinder binder)
        {
            var element = BinderToElement.CreateElement(label, binder);
            if (element != null) SetInteractableWithBinder(element, binder);

            return element;
        }

        #endregion


        #region Slider

        public static Element Slider(Expression<Func<int>> targetExpression, int max, Action<int> onValueChanged = null)
        {
            return Slider(targetExpression, 0, max, onValueChanged);
        }

        public static Element Slider(Expression<Func<float>> targetExpression, float max,
            Action<float> onValueChanged = null)
        {
            return Slider(targetExpression, 0f, max, onValueChanged);
        }

        public static Element Slider<T>(Expression<Func<T>> targetExpression, T min, T max,
            Action<T> onValueChanged = null)
        {
            return Slider(ExpressionUtility.CreateLabelString(targetExpression), targetExpression,
                ConstMinMaxGetter.Create(min, max), onValueChanged);
        }

        public static Element Slider<T>(Expression<Func<T>> targetExpression, Action<T> onValueChanged = null)
        {
            return Slider(ExpressionUtility.CreateLabelString(targetExpression), targetExpression, null,
                onValueChanged);
        }


        public static Element Slider(LabelElement label, Expression<Func<int>> targetExpression, int max,
            Action<int> onValueChanged = null)
        {
            return Slider(label, targetExpression, 0, max, onValueChanged);
        }

        public static Element Slider(LabelElement label, Expression<Func<float>> targetExpression, float max,
            Action<float> onValueChanged = null)
        {
            return Slider(label, targetExpression, 0f, max, onValueChanged);
        }

        public static Element Slider<T>(LabelElement label, Expression<Func<T>> targetExpression, T min, T max,
            Action<T> onValueChanged = null)
        {
            return Slider(label, targetExpression, ConstMinMaxGetter.Create(min, max), onValueChanged);
        }

        public static Element Slider<T>(LabelElement label, Expression<Func<T>> targetExpression,
            Action<T> onValueChanged = null)
        {
            return Slider(label, targetExpression, null, onValueChanged);
        }

        public static Element Slider<T>(LabelElement label, Expression<Func<T>> targetExpression,
            IMinMaxGetter minMaxGetter, Action<T> onValueChanged = null)
        {
            var binder = ExpressionUtility.CreateBinder(targetExpression);
            if (binder == null) return null;
            binder.onValueChanged += onValueChanged;
            return Slider(label, binder, minMaxGetter);
        }

        public static Element Slider(LabelElement label, IBinder binder, IMinMaxGetter minMaxGetter)
        {
            var contents = BinderToElement.CreateSliderElement(label, binder, minMaxGetter);
            if (contents == null) return null;

            SetInteractableWithBinder(contents, binder);

            return contents;
        }

        #endregion


        #region List

        public static Element List<TItem, TValue>(LabelElement label, List<TItem> list,
            Func<TItem, TValue> readItemValue, Action<TItem, TValue> onItemValueChanged,
            Func<TItem, string> createItemLabel = null)
            where TItem : class
        {
            return List(label,
                list,
                (binder, defaultLabelString) =>
                {
                    var childBinder = new ChildBinder<TItem, TValue>(binder,
                        readItemValue,
                        (item, value) =>
                        {
                            onItemValueChanged?.Invoke(item, value);
                            return item;
                        }
                    );

                    var itemLabel = createItemLabel != null && !binder.IsNull
                        ? Label(() => createItemLabel(binder.Get()))
                        : (LabelElement) defaultLabelString;

                    return Field(itemLabel, childBinder);
                }
            );
        }

        public static Element List<T>(LabelElement label, List<T> list,
            Func<BinderBase<T>, string, Element> createItemElement = null)
        {
            Func<IBinder, string, Element> createItemElementIBinder = null;
            if (createItemElement != null)
                createItemElementIBinder =
                    (ibinder, itemLabel) => createItemElement(ibinder as BinderBase<T>, itemLabel);

            var element = BinderToElement.CreateListElement(label, ConstGetter.Create(list), createItemElementIBinder);
            return Fold(label, element);
        }

        #endregion


        #region Dropdown

        public static Element Dropdown(Expression<Func<int>> targetExpression, IEnumerable<string> options,
            Action<int> onValueChanged = null)
        {
            return Dropdown(ExpressionUtility.CreateLabelString(targetExpression), targetExpression, options,
                onValueChanged);
        }

        public static Element Dropdown(LabelElement label, Expression<Func<int>> targetExpression,
            IEnumerable<string> options, Action<int> onValueChanged = null)
        {
            var binder = ExpressionUtility.CreateBinder(targetExpression);
            if (binder == null) return null;
            binder.onValueChanged += onValueChanged;

            Element element = new DropdownElement(label, binder, options);

            SetInteractableWithBinder(element, binder);

            return element;
        }

        #endregion


        #region Row/Column/Box

        public static Row Row(params Element[] elements)
        {
            return new Row(elements);
        }

        public static Row Row(IEnumerable<Element> elements)
        {
            return new Row(elements);
        }

        public static Column Column(params Element[] elements)
        {
            return new Column(elements);
        }

        public static Column Column(IEnumerable<Element> elements)
        {
            return new Column(elements);
        }

        public static BoxElement Box(params Element[] elements)
        {
            return new BoxElement(elements);
        }

        public static BoxElement Box(IEnumerable<Element> elements)
        {
            return new BoxElement(elements);
        }

        #endregion


        #region Fold

        public static FoldElement Fold(LabelElement label, params Element[] elements)
        {
            return Fold(label, elements as IEnumerable<Element>);
        }

        public static FoldElement Fold(LabelElement label, IEnumerable<Element> elements)
        {
            return new FoldElement(label, elements);
        }

        #endregion


        #region Window

        public static WindowElement Window(LabelElement title = null, params Element[] elements)
        {
            return new WindowElement(title, elements);
        }

        public static WindowElement Window(LabelElement title, IEnumerable<Element> elements)
        {
            return new WindowElement(title, elements);
        }

        #endregion


        #region Window Launcher

        public static WindowLauncherElement WindowLauncher(WindowElement window)
        {
            return WindowLauncher(null, window);
        }

        public static WindowLauncherElement WindowLauncher(LabelElement title, WindowElement window)
        {
            var label = title ?? window.title;
            return new WindowLauncherElement(label, window);
        }


        #endregion


        #region ElementCreator

        public static DynamicElement ElementCreatorWindowLauncher<T>(LabelElement title = null)
            where T : Behaviour, IElementCreator
        {
            return FindObjectObserverElement<T>(
                t =>
                {
                    title ??= typeof(T).ToString().Split('.').LastOrDefault();
                    var window = Window(title, t.CreateElement());
                    return WindowLauncher(window);
                });
        }

        
        public static DynamicElement ElementCreatorInline<T>(bool rebuildIfDisabled = true)
            where T : Behaviour, IElementCreator
        {
            return FindObjectObserverElement<T>(t => t.CreateElement(), typeof(T).Name, rebuildIfDisabled);
        }

        public static DynamicElement FindObjectObserverElement<T>(Func<T, Element> build, string displayName = null, bool rebuildIfDisabled = true)
            where T : Behaviour
        {
            T target = null;
            var lastCheckTime = Time.realtimeSinceStartup;
            var interval =
                Random.Range(1f, 1.5f); // 起動時に多くのFindObjectObserverElementが呼ばれるとFindObject()を呼ぶタイミングがかぶって重いのでランダムで散らす

            return new DynamicElement(
                () => target != null ? build?.Invoke(target) : null,
                e =>
                {
                    if (!CheckTargetEnable())
                    {
                        var t = Time.realtimeSinceStartup;
                        if (t - lastCheckTime > interval)
                        {
                            lastCheckTime = t;
                            target = Object.FindObjectOfType<T>();
                            return true;
                        }
                    }

                    return false;
                },
                displayName
            );

            bool CheckTargetEnable()
            {
                return target != null && !(rebuildIfDisabled && !target.isActiveAndEnabled);
            }
        }

        #endregion
    }
}