﻿using System;
using System.Collections.Generic;

namespace RosettaUI
{
    public static partial class UICustom
    {
        public class CreationFunc
        {
            public Func<LabelElement, IBinder, Element> func;
        }
        
        private static readonly Dictionary<Type, CreationFunc> CreationFuncTable = new();

        // struct は creationFunc 渡してもコピーになってしまうので class のみ対応
        public static void RegisterElementCreationFunc<T>(Func<LabelElement, T, Element> creationFunc)
            where T : class
        {
            var cf = new CreationFunc
            {
                // binder.GetObject() が変わったら UI を作り直す
                // 下手すると毎フレーム変わるので重いかもしれない
                // 本当は binder から Element を生成するべきだが複雑な UI を作ろうとするとかなり大変
                // とりあえず「重いけど安全に動く」動作を選択
                func = (label,binder) => UI.DynamicElementOnStatusChanged(
                    binder.GetObject,
                    obj => creationFunc(label, (T)obj).RegisterValueChangeCallback(() => binder.SetObject(obj))
                ),
            };

            RegisterElementCreationFunc(typeof(T), cf);
        }

        public static void RegisterElementCreationFunc(Type type, CreationFunc creationFunc)
        {
            CreationFuncTable[type] = creationFunc;
        }

        public static bool UnregisterElementCreationFunc(Type type)
        {
            return CreationFuncTable.Remove(type);
        }

        public static CreationFunc GetElementCreationMethod(Type type)
        {
            if (type == null) return null;
            
            if (!CreationFuncTable.TryGetValue(type, out var func))
            {
                func = GetElementCreationMethod(type.BaseType);
                CreationFuncTable[type] = func;
            }

            return func;
        }
        
        
        
        #region Scope
        
        public readonly ref struct ElementCreationFuncScope<T>
            where T : class
        {
            private readonly CreationFunc _creationFuncCache;

            public ElementCreationFuncScope(Func<LabelElement, T, Element> creationFunc)
            {
                _creationFuncCache = GetElementCreationMethod(typeof(T));
                RegisterElementCreationFunc(creationFunc);
            }

            public void Dispose()
            {
                RegisterElementCreationFunc(typeof(T), _creationFuncCache);
            }
        }
        
        #endregion
    }
}