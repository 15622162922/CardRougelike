using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum LoadMode
{
    Editor,
    AssetBundle,
}

public class NewLoadManager: BaseManager<NewLoadManager>
{
    private BaseLoader loader;

    public override void Init()
    {
        base.Init();
        LoadMode mode;
    #if UNITY_EDITOR
        mode = LoadMode.Editor;
        loader = new AssetDatabaseLoader();
#else
        mode = LoadMode.AssetBundle;
        loader = new AssetBundleLoader();
#endif
    }

    public LoadTask Load<T>(string path, Action<T> callback) where T : UnityEngine.Object
    {
        if (loader == null)
        {
            Error("加载器启动失败");
            return null;
        }
        
        path = path.Replace("\\", "/");
        var loadTask = loader.Load<T>(path, callback);
        return loadTask;
    }

    public void Unload(string path,  bool forceUnload = false)
    {
        loader.Unload(path, forceUnload);
    }
}
