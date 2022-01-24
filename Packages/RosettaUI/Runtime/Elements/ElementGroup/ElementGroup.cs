﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace RosettaUI
{
    /// <summary>
    /// 子供として任意の Element を持てる Element
    /// </summary>
    public abstract class ElementGroup : Element
    {
        #region For Builder

        public event Action<ElementGroup> onRebuildChildren;

        public void RebuildChildren() => onRebuildChildren?.Invoke(this);

        #endregion


        protected List<Element> elements;
        
        // Children　Update()で更新される子要素すべて
        // Contents　特殊な意味の子要素は含まれない
        // WindowのタイトルバーなどはChildrenに含むがContentsには含まない
        public ReadOnlyCollection<Element> Children => elements.AsReadOnly();
        public virtual IEnumerable<Element> Contents => Children;

        public virtual string DisplayName => GetType().Name;

        protected ElementGroup() { }

        public ElementGroup(IEnumerable<Element> elements)
        {
            SetElements(elements);
        }

        protected void SetElements(IEnumerable<Element> elements)
        {
            this.elements = elements?.Where(e => e != null).ToList() ?? new List<Element>();
            foreach (var e in this.elements)
            {
                e.SetParent(this);
            }
        }

        protected override void UpdateInternal()
        {
            base.UpdateInternal();

            foreach (var e in elements)
            {
                e.Update();
            }
        }

        public override void Destroy()
        {
            foreach (var e in elements)
            {
                e.Destroy();
            }
            
            base.Destroy();
        }
    }
}