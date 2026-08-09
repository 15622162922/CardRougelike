using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
///     自动生成，请勿修改代码
/// </summary>
public class Config_TestConfig
{
    private Dictionary<int, Config_TestConfigData> _config;

    // 获取单条内容
    public Config_TestConfigData Get(int id)
    {
        LoadConfig();
        return _config.GetValueOrDefault(id, null);
    }

    // 以List的形式获取所有内容
    public List<Config_TestConfigData> GetList()
    {
        LoadConfig();
        return _config.Values.ToList();
    }

    // 以Dictionary的形式获取所有内容
    public Dictionary<int, Config_TestConfigData> GetDict()
    {
        LoadConfig();
        return _config;
    }

    //预热接口，提前读取配置表
    public void PreWarm()
    {
        LoadConfig();
    }

    //读取配置表内容
    private void LoadConfig()
    {
        if (_config == null) _config = new Dictionary<int, Config_TestConfigData>();
        //todo 从json中读取数据并存入_config内
    }
}

[Serializable]
public class Config_TestConfigData
{
    public int id;
    public Sheet_TestSheet data;
}

[Serializable]
public class Sheet_TestSheet
{
    public string name;
}