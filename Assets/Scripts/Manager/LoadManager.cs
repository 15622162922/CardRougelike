using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using YooAsset;
using YooAsset.Editor;
using System;
using System.IO;
using UnityEditor;

public class LoadManager : BaseManager<LoadManager>
{
    public override void Init()
    {
        base.Init();
    }

    public override void Destroy()
    {
        base.Destroy();
    }

    Dictionary<string, AssetRequest> cacheRequests = new Dictionary<string, AssetRequest>();
    Dictionary<string, BundleAssetRequest> cacheBundleRequests = new Dictionary<string, BundleAssetRequest>();

    public T Load<T>(string path) where T : UnityEngine.Object
    {
        AssetRequest request = GetRequest(path);
        if (!request.isDone)
        {
            request.Load();
        }

        request.Retain();

        return request.asset as T;
    }

    public void LoadAsync<T>(string path, Action<T> compeletedCallback) where T : UnityEngine.Object
    {
        AssetRequest request = GetRequest(path);
        if (!request.isDone)
        {
            request.LoadAsync();
            request.completed += () =>
            {
                request.Retain();
                compeletedCallback?.Invoke(request.asset as T);
            };
        }
        else
        {
            request.Retain();
            compeletedCallback?.Invoke(request.asset as T);
        }
    }

    public GameObject LoadPrefab(string path, Transform root = null)
    {
        GameObject gameObject = Load<GameObject>(path);
        GameObject obj = GameObject.Instantiate(gameObject, root);

        return obj;
    }

    public void LoadPrefabAsync(string path, Transform root = null, Action<GameObject> compeletedCallback = null)
    {
        LoadAsync<GameObject>(path, (gameObject) =>
        {
            GameObject obj = GameObject.Instantiate(gameObject, root);
            compeletedCallback?.Invoke(obj);
        });
    }

    public T LoadConfig<T>() where T : UnityEngine.Object
    {
        string configName = typeof(T).ToString();
        AssetRequest request = GetRequest($"Config/Game/{configName}.asset");
        if (!request.isDone)
        {
            request.isKeep = true;
            request.Load();
            request.Retain();
        }

        return request.asset as T;
    }

    #region ·Ç±©Â¶½Ó¿Ú

    public void LoadAsset(AssetRequest request)
    {
#if UNITY_EDITOR
        string filePath = $"Assets/RawResources/{request.path}";
        Log($"Load Asset from {filePath}");
        request.asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(filePath);
        if (request.asset != null)
        {
            request.loadStatus = LoadRequest.LoadStatus.Completed;
        }
        else
        {
            Error($"Load Asset Error with path: {filePath}");
            return;
        }
#else
        AssetBundle assetBundle = GetAssetBundle(request.path);
        if (assetBundle != null)
        {
            string assetPath = GetAssetNameFromPath(request.path);
            request.asset = assetBundle.LoadAsset(assetPath);
            request.loadStatus = LoadRequest.LoadStatus.Completed;
        }
        else
        {
            BundleAssetRequest bundleRequest = GetBundleRequest(request.path);
            LoadBundle(bundleRequest);
        }
#endif

    }

    public void LoadAssetAsync(AssetRequest request)
    {
        GameManager.Instance.StartCoroutine(_LoadAssetAsync(request));
    }

    IEnumerator _LoadAssetAsync(AssetRequest request)
    {
#if UNITY_EDITOR
        string filePath = $"Assets/RawResources/{request.path}";
        Log($"Load Asset Async from {filePath}");
        request.asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(filePath);
        if (request.asset != null)
        {
            request.loadStatus = LoadRequest.LoadStatus.Completed;
        }
        else
        {
            Error($"Load Asset Async Error with path: {filePath}");
            yield return null;
        }
#else
        AssetBundle assetBundle = GetAssetBundle(request.path);
        if (assetBundle != null)
        {
            string assetPath = GetAssetNameFromPath(request.path);
            AssetBundleRequest _request = assetBundle.LoadAssetAsync(assetPath);
            request.loadStatus = LoadRequest.LoadStatus.Loading;

            yield return _request.isDone;

            request.loadStatus = LoadRequest.LoadStatus.Completed;
            request.asset = _request.asset;
            request.LoadAsyncCompleted();
        }
        else
        {
            BundleAssetRequest bundleRequest = GetBundleRequest(request.path);
            LoadBundleAsync(bundleRequest);
        }
#endif
    }

    public void LoadBundle(BundleAssetRequest bundleRequest)
    {
        bundleRequest.bundle = AssetBundle.LoadFromFile(bundleRequest.bundlePath);
        bundleRequest.loadStatus = LoadRequest.LoadStatus.Completed;
    }

    public void LoadBundleAsync(BundleAssetRequest bundleRequest)
    {
        GameManager.Instance.StartCoroutine(_LoadBundleAsync(bundleRequest));
    }

    IEnumerator _LoadBundleAsync(BundleAssetRequest bundleRequest)
    {
        AssetBundleCreateRequest _request = AssetBundle.LoadFromFileAsync(bundleRequest.bundlePath);
        bundleRequest.loadStatus = LoadRequest.LoadStatus.Loading;
        yield return _request.isDone;
        bundleRequest.bundle = _request.assetBundle;
        bundleRequest.loadStatus = LoadRequest.LoadStatus.Completed;
        bundleRequest.LoadAsyncCompleted();
    }

    public AssetRequest GetRequest(string path)
    {
        AssetRequest request = null;
        if (cacheRequests.TryGetValue(path, out request))
        {
            return request;
        }

        request = new AssetRequest();
        request.path = path;
        request.loadStatus = LoadRequest.LoadStatus.Idle;

        cacheRequests.Add(path, request);

        return request;
    }

    public BundleAssetRequest GetBundleRequest(string path)
    {
        BundleAssetRequest request = null;
        if (cacheBundleRequests.TryGetValue(path, out request))
        {
            return request;
        }

        request = new BundleAssetRequest();
        request.path = path;
        request.bundlePath = GetAssetBundleNameFromPath(path);
        request.loadStatus = LoadRequest.LoadStatus.Idle;
        cacheBundleRequests.Add(request.bundlePath, request);

        return request;
    }

    AssetBundle GetAssetBundle(string path)
    {
        string assetBundleName = GetAssetBundleNameFromPath(path);
        BundleAssetRequest request = null;
        if (cacheBundleRequests.TryGetValue(assetBundleName, out request))
        {
            if (request.isDone)
            {
                return request.bundle;
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

public class LoadRequest
{
    public string path;
    public UnityEngine.Object asset;
    public virtual bool isDone
    {
        get
        {
            if (isAsync && loadStatus == LoadStatus.Completed)
            {
                return true;
            }
            else if (!isAsync && loadStatus == LoadStatus.Completed)
            {
                return true;
            }

            return false;
        }
    }
    public enum LoadStatus
    {
        Idle,
        Loading,
        Completed,
    }

    public bool isKeep = false;
    public bool isAsync = false;
    public LoadStatus loadStatus = LoadStatus.Idle;
    public delegate void CompletedDelegate();
    public CompletedDelegate completed;

    int refNum = 0;

    public virtual void Load()
    {
        
    }

    public virtual void LoadAsync()
    {

    }

    public virtual void LoadAsyncCompleted()
    {

    }

    public void Retain()
    {
        refNum++;
    }

    public void Release()
    {
        refNum--;
    }

    public bool HasRef()
    {
        return refNum > 0;
    }
}

public class AssetRequest : LoadRequest
{
    public override void Load()
    {
        base.Load();
        LoadManager.Instance.LoadAsset(this);
    }

    public override void LoadAsync()
    {
        base.LoadAsync();
        LoadManager.Instance.LoadAssetAsync(this);
    }

    public override void LoadAsyncCompleted()
    {
        base.LoadAsyncCompleted();
        completed?.Invoke();
    }
}

public class BundleAssetRequest : LoadRequest
{
    public string bundlePath;
    public AssetBundle bundle;

    public override void Load()
    {
        base.Load();
        LoadManager.Instance.LoadBundle(this);

        AssetRequest request = LoadManager.Instance.GetRequest(path);
        LoadManager.Instance.LoadAsset(request);
    }

    public override void LoadAsync()
    {
        base.LoadAsync();
        LoadManager.Instance.LoadBundleAsync(this);
    }

    public override void LoadAsyncCompleted()
    {
        base.LoadAsyncCompleted();
        AssetRequest request = LoadManager.Instance.GetRequest(path);
        if (request.isDone)
        {
            Retain();

            completed?.Invoke();
        }
        else
        {
            request.LoadAsync();
        }
    }
}