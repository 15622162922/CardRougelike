using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PropsProxy : MonoBehaviour
{
    [HideInInspector]
    public List<string> propNames = new List<string>();

    [HideInInspector]
    public List<GameObject> props = new List<GameObject>();

    [HideInInspector]
    public int propCount = 0;

    public GameObject GetGameObject(string name)
    {
        foreach(var _name in propNames)
        {
            if(StringUtil.GetFirstLowerStr(_name) == StringUtil.GetFirstLowerStr(name))
            {
                int index = propNames.IndexOf(_name);
                return props[index];
            }
        }

        Debug.LogError($"Can't Find the gameObject what name is {name}");
        return null;
    }

    public T Get<T>(string name) where T : Component
    {
        foreach (var _name in propNames)
        {
            if (StringUtil.GetFirstLowerStr(_name) == StringUtil.GetFirstLowerStr(name))
            {
                int index = propNames.IndexOf(_name);
                T t = props[index].GetComponent<T>();
                if (t != null)
                {
                    return t;
                }
                Debug.LogError($"Can't Find the gameObject which name is {name} and have component {typeof(T).ToString()}");
                return default(T);
            }
        }

        Debug.LogError($"Can't Find the gameObject what name is {name}");
        return default(T);
    }
}
