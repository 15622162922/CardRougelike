using UnityEngine;

public class MapModule : BaseModule<MapModule>
{
    private GameObject MapRoot;

    protected override void OnInit()
    {
        base.OnInit();
        LoadMapRoot();

        LoadTestMap();
    }

    protected override void OnRelease()
    {
        base.OnRelease();
    }

    private void LoadMapRoot()
    {
        MapRoot = GameManager.Instance.WorldRoot.GetProp("MapRoot");
    }

    private void LoadTestMap()
    {
        var testMap = LoadManager.Instance.LoadPrefab("Prefab/Map/Test_Map.prefab", MapRoot.transform);
        MapManager.Instance.SetMapObj(testMap);
        MapManager.Instance.GenerateTestMap();
    }
}