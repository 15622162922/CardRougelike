using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PropsProxy))]
[CanEditMultipleObjects]
public class PropsProxyInspector : Editor
{
    PropsProxy proxy;

    public int count;

    Color color_O = Color.green;
    Color color_X = Color.red;

    Dictionary<string, string> _nameKeys;

    public Dictionary<string, string> NameKeys
    {
        get
        {
            if (_nameKeys == null)
            {
                _nameKeys = new Dictionary<string, string>();
                _nameKeys["obj_"] = "GameObject";
                _nameKeys["img_"] = "Image";
                _nameKeys["btn_"] = "Button";
                _nameKeys["txt_"] = "Text";
                _nameKeys["sld_"] = "Slider";
                _nameKeys["rawImg_"] = "RawImage";
                _nameKeys["tog_"] = "Toggle";
                _nameKeys["togG_"] = "ToggleGroup";
                _nameKeys["scrRect_"] = "ScrollRect";
                _nameKeys["glg_"] = "GridLayoutGroup";
                _nameKeys["changeImg"] = "GUIChangeImage";
            }

            return _nameKeys;
        }
    }

    private void OnEnable()
    {
        proxy = target as PropsProxy;
        this.count = proxy.propCount;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.BeginHorizontal();
        GUILayout.Label("PropsList");
        this.count = EditorGUILayout.DelayedIntField(this.count);
        if(this.count != proxy.propCount)
        {
            SetPropCount(this.count);
        }

        if (GUILayout.Button("comfirm"))
        {
            SetPropCount(this.count);
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("?"))
        {
            Debug.Log(NameKeys);
        }

        if (GUILayout.Button("批量添加"))
        {
            AddChildToProxy(proxy.gameObject);
        }

        if (GUILayout.Button("生成声明"))
        {
            GenerateStatementInClipboard();
        }

        if (GUILayout.Button("生成引用"))
        {
            GenerateCiteInClipboard();
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginVertical();
        for (int i = 0; i < proxy.propCount; i++)
        {
            GUILayout.BeginHorizontal();


            GUILayout.Label(i.ToString());
            GUILayout.Space(1);
            proxy.propNames[i] = EditorGUILayout.TextField(proxy.propNames[i]);
            proxy.props[i] = (GameObject)EditorGUILayout.ObjectField(proxy.props[i], typeof(GameObject), true);
            if (proxy.propNames[i] == string.Empty && proxy.props[i] != null)
            {
                proxy.propNames[i] = proxy.props[i].name;
            }

            if (GUILayout.Button("生成声明"))
            {
                GenerateStatementInClipboard(i);
            }

            if (GUILayout.Button("生成引用"))
            {
                GenerateCiteInClipboard(i);
            }

            Color curColor = GUI.backgroundColor;
            if (DeteNameLegal(proxy.propNames[i]))
            {
                GUI.backgroundColor = color_O;
            }
            else if (proxy.props[i] == null)
            {

            }
            else
            {
                GUI.backgroundColor = color_X;
            }
            if (GUILayout.Button("X"))
            {
                proxy.propNames.RemoveAt(i);
                proxy.props.RemoveAt(i);
                count--;
                proxy.propCount--;
            }

            GUI.backgroundColor = curColor;

            GUILayout.EndHorizontal();
        }
        if (GUILayout.Button("+"))
        {
            this.count++;
            SetPropCount(this.count);
        }
        GUILayout.EndVertical();
    }

    void SetPropCount(int count)
    {
        if (count != proxy.propCount)
        {

            if (count > proxy.propCount)
            {
                //��Ҫ����
                int index = count - proxy.propCount;
                for (int i = 0; i < index; i++)
                {
                    proxy.propNames.Add(string.Empty);
                    proxy.props.Add(null);
                }
            }
            else
            {
                //��Ҫɾ��
                int index = proxy.propCount - count;
                for (int i = 0; i < index; i++)
                {
                    proxy.propNames.RemoveAt(proxy.propNames.Count - 1);
                    proxy.props.RemoveAt(proxy.props.Count - 1);
                }
            }
            proxy.propCount = count;
        }
    }

    bool DeteNameLegal(string propName)
    {
        if (propName.Contains("_"))
        {
            string firStr = propName.Split('_')[0] + "_";
            foreach (var name in NameKeys)
            {
                if(StringUtil.GetFirstLowerStr(name.Key) == StringUtil.GetFirstLowerStr(firStr))
                {
                    return true;
                }
            }
        }
        return false;
    }

    System.Type GetComponentName(string propName)
    {
        if (propName.Contains("_"))
        {
            string firStr = propName.Split('_')[0] + "_";
            if (NameKeys.ContainsKey(firStr))
            {
                return System.Type.GetType(NameKeys[firStr]);
            }
        }
        return null;
    }

    private void AddChildToProxy(GameObject obj)
    {
        if (obj.transform.childCount > 0)
        {
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                string name = obj.transform.GetChild(i).name;
                if (name.Contains("_"))
                {
                    string firStr = name.Split('_')[0] + "_";
                    foreach (var key in NameKeys.Keys)
                    {
                        if (StringUtil.GetFirstLowerStr(key) == StringUtil.GetFirstLowerStr(firStr))
                        {
                            AddProps(name, obj.transform.GetChild(i).gameObject);

                            break;
                        }
                    }
                    AddChildToProxy(obj.transform.GetChild(i).gameObject);
                }
            }
        }
    }

    void AddProps(string propName, GameObject obj)
    {
        if (!proxy.props.Contains(obj))
        {
            int emptyIndex = -1;
            for(int i = 0; i < proxy.propCount; i++)
            {
                if(string.IsNullOrEmpty(proxy.propNames[i]) && proxy.props[i] == null)
                {
                    emptyIndex = i;
                    proxy.propNames[i] = propName;
                    proxy.props[i] = obj;
                }
            }

            if(emptyIndex == -1)
            {
                proxy.propNames.Add(propName);
                proxy.props.Add(obj);
                count++;
                proxy.propCount++;
            }
        }
    }

    void GenerateStatementInClipboard()
    {
        string str = "";
        for(int i = 0; i < proxy.propCount; i++)
        {
            if(!string.IsNullOrEmpty(proxy.propNames[i]) && proxy.props[i] != null)
            {
                string propType = GetPropTypeName(proxy.propNames[i]);
                if(propType != null)
                {
                    str += string.Format("{0} {1};\n", propType, StringUtil.GetFirstLowerStr(proxy.propNames[i]));
                }
            }
        }
        GUIUtility.systemCopyBuffer = str;
    }

    void GenerateStatementInClipboard(int index)
    {
        if (!string.IsNullOrEmpty(proxy.propNames[index]) && proxy.props[index] != null)
        {
            string propType = GetPropTypeName(proxy.propNames[index]);
            if (propType != null)
            {
                GUIUtility.systemCopyBuffer = string.Format("{0} {1};\n", propType, StringUtil.GetFirstLowerStr(proxy.propNames[index]));
            }
        }
    }

    void GenerateCiteInClipboard()
    {
        string str = "PropsProxy proxy = this.gameObject.GetComponent<PropsProxy>();\n";
        for (int i = 0; i < proxy.propCount; i++)
        {
            if (!string.IsNullOrEmpty(proxy.propNames[i]) && proxy.props[i] != null)
            {
                string propType = GetPropTypeName(proxy.propNames[i]);
                if (propType != null)
                {
                    if(propType == "GameObject")
                    {
                        str += string.Format("{0} = proxy.GetGameObject(\"{1}\");\n", StringUtil.GetFirstLowerStr(proxy.propNames[i]), StringUtil.GetFirstLowerStr(proxy.propNames[i]));
                    }
                    else
                    {
                        str += string.Format("{0} = proxy.Get<{1}>(\"{2}\");\n", StringUtil.GetFirstLowerStr(proxy.propNames[i]), propType, StringUtil.GetFirstLowerStr(proxy.propNames[i]));
                    }
                }
            }
        }
        GUIUtility.systemCopyBuffer = str;
    }

    void GenerateCiteInClipboard(int index)
    {
        if (!string.IsNullOrEmpty(proxy.propNames[index]) && proxy.props[index] != null)
        {
            string propType = GetPropTypeName(proxy.propNames[index]);
            if (propType != null)
            {
                if (propType == "GameObject")
                {
                    GUIUtility.systemCopyBuffer = string.Format("{0} = proxy.GetGameObject(\"{1}\");\n", StringUtil.GetFirstLowerStr(proxy.propNames[index]), StringUtil.GetFirstLowerStr(proxy.propNames[index]));
                }
                else
                {
                    GUIUtility.systemCopyBuffer = string.Format("{0} = proxy.Get<{1}>(\"{2}\");\n", StringUtil.GetFirstLowerStr(proxy.propNames[index]), propType, StringUtil.GetFirstLowerStr(proxy.propNames[index]));
                }
            }
        }
    }

    string GetPropTypeName(string propName)
    {
        if (propName.Contains("_"))
        {
            string firStr = propName.Split('_')[0] + "_";
            foreach (var key in NameKeys.Keys)
            {
                if (StringUtil.GetFirstLowerStr(key) == StringUtil.GetFirstLowerStr(firStr))
                {
                    return NameKeys[key];
                }
            }
        }
        return null;
    }
}
