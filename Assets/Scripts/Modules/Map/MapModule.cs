using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapModule : BaseModule<MapModule>
{
    GameObject MapRoot;
    public override void Register()
    {
        base.Register();
        LoadMapRoot();

        LoadTestMap();
    }

    public override void UnRegister()
    {
        base.UnRegister();
    }

    void LoadMapRoot()
    {
        MapRoot = GameManager.Instance.WorldRoot.GetProp("MapRoot");
    }

    void LoadTestMap()
    {
        GameObject testMap = LoadManager.Instance.LoadPrefab("Prefab/Map/Test_Map.prefab", MapRoot.transform);
        MapManager.Instance.SetMapObj(testMap);
        MapManager.Instance.GenerateTestMap();
    }
}
