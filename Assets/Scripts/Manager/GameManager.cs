using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public ModuleManager Modules;
    public GameObject WorldRoot;
    List<BaseManager> managers;

    //List<BaseManager> managers;
    void Awake()
    {
        Instance = this;
        managers = new List<BaseManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadWorldRoot();
        LoadUIRoot();
        ModuleManager.Instance.RegisterModules();
    }

    private void OnDestroy()
    {
        DestroyAllManagers();
    }

    void LoadWorldRoot()
    {
        WorldRoot = LoadManager.Instance.LoadPrefab("Prefab/Root/WorldRoot.prefab");
        GameObject.DontDestroyOnLoad(WorldRoot);
    }

    void LoadUIRoot()
    {
        UIManager.Instance.CreateUIRoot();
    }

    public void RegisterManager(BaseManager manager)
    {
        managers.Add(manager);
    }

    void DestroyAllManagers()
    {
        foreach (var manager in managers)
        {
            manager.Destroy();
        }
    }
}
