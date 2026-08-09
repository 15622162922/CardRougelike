using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
///     自动生成，请勿修改代码
/// </summary>
public static class Config_MapDataConfig
{
    private static Dictionary<int, Sheet_MapData> _config;
    
    // 获取单条内容
    public static Sheet_MapData Get(int id)
    {
        LoadConfig();
        return _config.GetValueOrDefault(id, null);
    }
    
    // 以List的形式获取所有内容
    public static List<Sheet_MapData> GetList()
    {
        LoadConfig();
        return _config.Values.ToList();
    }
    
    // 以Dictionary的形式获取所有内容
    public static Dictionary<int, Sheet_MapData> GetDict()
    {
        LoadConfig();
        return _config;
    }
    
    //预热接口，提前读取配置表
    public static void PreWarm()
    {
        LoadConfig();
    }

    //读取配置表内容
    private static void LoadConfig()
    {
        if (_config == null) _config = new Dictionary<int, Sheet_MapData>();
        //todo 从json中读取数据并存入_config内
    }
}

    [System.Serializable]
    public class Sheet_MapData
    {
       // 索引
        public int id;
       // 分组
        public int group;
       // 地图名称
        public string mapName;
       // 使用Tile分组
        public int tileGroup;
    }
