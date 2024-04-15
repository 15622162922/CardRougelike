using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectExtension
{
    public static T GetProp<T>(this GameObject root, string propName) where T: UnityEngine.Component
    {
        PropsProxy proxy = root.GetComponent<PropsProxy>();
        if (proxy != null)
        {
            T prop = proxy.Get<T>(propName);

            if (prop != null)
            {
                return prop;
            }
        }

        return default(T);
    }

    public static GameObject GetProp(this GameObject root, string propName)
    {
        PropsProxy proxy = root.GetComponent<PropsProxy>();
        if (proxy != null)
        {
            GameObject prop = proxy.GetGameObject(propName);

            if (prop != null)
            {
                return prop;
            }
        }

        return null;
    }
}
