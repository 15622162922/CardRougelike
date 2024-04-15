using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigManager : BaseManager<ConfigManager>
{
    const string configPath = "Config/Game/";

    Dictionary<string, ScriptableObject> configs = new Dictionary<string, ScriptableObject>();

    public void GetData<Config>(int key)
    {

    }

    public void GetData<Config>(int key1, int key2)
    {

    }

    

    Config LoadConfig<Config>() where Config:ScriptableObject
    {
        Config config = LoadManager.Instance.Load<Config>(configPath + typeof(Config).ToString());
        if (config)
        {

        }

        return config;
    }
}
