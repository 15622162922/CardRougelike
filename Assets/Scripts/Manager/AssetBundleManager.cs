using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEditor;

public class AssetBundleManager : BaseManager<AssetBundleManager>
{
    public override void Init()
    {
        base.Init();
        cacheBundle = new Dictionary<string, AssetBundle>();
    }

    private Dictionary<string, AssetBundle> cacheBundle;

    public T LoadAsset<T>(string path) where T: UnityEngine.Object
    {
#if UNITY_EDITOR
        string filePath = $"Assets/RawResources/{path}";
        T asset = AssetDatabase.LoadAssetAtPath<T>(filePath);
        if (asset != null)
        {
            return asset;
        }
#else
        AssetBundle ab = GetAssetBundle(path);
        string fileName = GetAssetNameFromPath(path);
        if (ab != null && !string.IsNullOrEmpty(fileName))
        {
            T asset = ab.LoadAsset<T>(fileName);
            return asset;
        }
#endif
        return default(T);
    }

    public void LoadAssetAsync<T>(string path, Action<T> callback)
    {
        
    }

    public GameObject LoadPrefab(string path, Transform parent = null)
    {
        GameObject prefab = LoadAsset<GameObject>(path);
        GameObject obj = GameObject.Instantiate(prefab, parent);
        return obj;
    }

    public void LoadPrefabAsync(string path, Transform parent, Action<GameObject> callback)
    {

    }

    #region ·Ç±©Â¶½Ó¿Ú
    AssetBundle GetAssetBundle(string path)
    {
        string directoryPath = GetAssetBundleNameFromPath(path);
        if (cacheBundle.ContainsKey(directoryPath))
        {
            return cacheBundle[directoryPath];
        }
        else
        {
            string fullPath = Application.dataPath + $"/AssetBundle/{directoryPath}.ab";
            AssetBundle ab = AssetBundle.LoadFromFile(fullPath);
            if (ab != null)
            {
                cacheBundle.Add(directoryPath, ab);
                return ab;
            }
        }
        return null;
    }

    string GetAssetBundleNameFromPath(string path)
    {
        int secondSlashIndex = path.IndexOf('/', path.IndexOf('/') + 1);
        string directoryPath = secondSlashIndex != -1 ? path.Substring(0, secondSlashIndex) : path;

        return directoryPath;
    }

    string GetAssetNameFromPath(string path)
    {
        int secondSlashIndex = path.IndexOf('/', path.IndexOf('/') + 1);
        string assetName = secondSlashIndex != -1 ? path.Substring(secondSlashIndex + 1) : string.Empty;

        return assetName;
    }

    #endregion
}
