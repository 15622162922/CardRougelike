using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseConfigManager<ConfigManager, DataConfig> : ScriptableObject where ConfigManager: BaseConfigManager<ConfigManager, DataConfig>, new()
{
    private static ConfigManager _instance;
    public static ConfigManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ConfigManager();
            }
            return _instance;
        }
    }

    public virtual string tag
    {
        get
        {
            return "BaseConfig";
        }
    }

    protected SortedList<int, DataConfig> sourceDatas = new SortedList<int, DataConfig>();

    public int GetCount()
    {
        return sourceDatas.Count;
    }

    public DataConfig GetData(int key)
    {
        DataConfig value = default(DataConfig);
        if (sourceDatas.TryGetValue(key, out value))
        {
            return value;
        }
        else
        {
            Logger.Log(tag, Logger.LogChannel.Error, $"Can Find Data with error id: {key}, please checkout.");
            return default(DataConfig);
        }
    }
}
