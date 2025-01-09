using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameEngine
{
    public abstract class ViewBase : MonoBehaviour
    {
        protected Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();

        public abstract void Init();
        
        protected void Bind<T>(Type type = null) where T : UnityEngine.Object
        {
            string[] names;
            if (type == null)
                names = new string[] { typeof(T).Name };
            else
                names = Enum.GetNames(type);
            UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
            _objects.Add(typeof(T), objects);

            for (int i = 0; i < names.Length; i++)
            {
                if (typeof(T) == typeof(GameObject))
                    objects[i] = Util.FindChild(gameObject, names[i], true);
                else
                    objects[i] = Util.FindChild<T>(gameObject, names[i], true);

                if (objects[i] == null)
                    Debug.LogError($"Failed to bind({names[i]})");
            }
        }

        protected T Get<T>(int idx) where T : UnityEngine.Object
        {
            UnityEngine.Object[] objects = null;
            if (_objects.TryGetValue(typeof(T), out objects) == false)
                return null;

            return objects[idx] as T;
        }

        protected GameObject GetObject(int idx) { return Get<GameObject>(idx); }
        protected Transform GetTransform(int idx) { return Get<Transform>(idx); }
        protected Text GetText(int idx) { return Get<Text>(idx); }
        protected Button GetButton(int idx) { return Get<Button>(idx); }
        protected Image GetImage(int idx) { return Get<Image>(idx); }
    }
}