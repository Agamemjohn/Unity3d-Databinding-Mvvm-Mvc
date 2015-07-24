using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Foundation.Databinding.View
{
    public static class BindingExtensions
    {
        /// <summary>
        ///  Looks for component at or above source in hierarchy
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T FindInParent<T>(this GameObject obj) where T : Component
        {
            var t = obj.transform;
            
            for (int i = 0;i < 25;i++)
            {
                if (t == null)
                    return null;

                var found = t.GetComponent<T>();

                if (found != null)
                {
                    return found;
                }

                if (t.parent == null)
                {
                    return null;
                }

                t = t.parent;
            }

            return null;
        }

        /// <summary>
        /// Gets a component that implements interface type T
        /// </summary>
        public static T GetInterface<T>(this GameObject gameObject) where T : class
        {
            if (!typeof(T).IsInterface)
            {
                throw new System.Exception(typeof(T).ToString() + " is not an interface");
            }

            return gameObject.GetComponents<Component>().OfType<T>().FirstOrDefault();
        }

        /// <summary>
        /// Gets all components that implement interface type T
        /// </summary>
        public static IEnumerable<T> GetInterfaces<T>(this GameObject gameObject) where T : class
        {
            if (!typeof(T).IsInterface)
            {
                Debug.LogError(typeof(T).ToString() + ": is not an actual interface!");
                return Enumerable.Empty<T>();
            }

            return gameObject.GetComponents<Component>().OfType<T>();
        }
    }
}
