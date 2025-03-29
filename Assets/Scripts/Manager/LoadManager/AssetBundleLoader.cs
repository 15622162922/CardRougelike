using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// AssetBundle模式的加载器
/// </summary>
public class AssetBundleLoader : BaseLoader
{
    public override LoadTask<T> Load<T>(string path, Action<T> onComplete)
    {
        var task = new LoadTask<T>();
        task.Path = path;
        if (onComplete != null)
            task.AddCallback(onComplete);
        //异步加载任务
        LoadInternal<T>(path, task);
        return task;
    }

    public override void Unload(string path, bool forceUnload = false)
    {
        //暂时不卸载AssetBundle资源
    }

    private async void LoadInternal<T>(string path, LoadTask<T> task) where T : UnityEngine.Object
    {
        try
        {
            //加载依赖包
            string[] dependencies = await GetDependenciesAsync(path);
            foreach (var dep in dependencies)
            {
                await LoadAssetBundleAsync(dep, task);
            }
            
            //加载主包
            AssetBundle mainBundle = await LoadAssetBundleAsync(GetAssetBundleNameFromPath(path), task);
            if (mainBundle == null)
            {
                Logger.Log("AssetBundleLoader", Logger.LogChannel.Error, $"无法加载AssetBundle：{path}");
                return;
            }
            
            //从主包中加载目标资源
            string assetName = GetAssetNameFromPath(path);
            AssetBundleRequest assetRequest = mainBundle.LoadAssetAsync(assetName);
            while (!assetRequest.isDone)
            {
                if (task.IsCanceled)
                {
                    return;
                }

                task.SetProgress(assetRequest.progress);
                await Task.Yield();
            }
            
            task.Complete(assetRequest.asset as T);
        }
        catch (Exception e)
        {
            Logger.Log("AssetBundleLoader", Logger.LogChannel.Error, $"加载{path}时发生错误：{e.Message}");
        }
    }

    /// <summary>
    /// 异步加载AssetBundle
    /// </summary>
    /// <param name="url"></param>
    /// <param name="loadTask"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private async Task<AssetBundle> LoadAssetBundleAsync<T>(string url, LoadTask<T> loadTask)
    {
        using (var request = UnityWebRequestAssetBundle.GetAssetBundle(url))
        {
            var asyncOp = request.SendWebRequest();
            while (!asyncOp.isDone)
            {
                if (loadTask.IsCanceled)
                {
                    return null;
                }
                loadTask.SetProgress(asyncOp.progress);
                await Task.Yield();
            }

            if (request.result != UnityWebRequest.Result.Success)
            {
                Logger.Log("AssetBundleLoader", Logger.LogChannel.Error, $"加载Bundle失败：{url}");
                return null;
            }

            return DownloadHandlerAssetBundle.GetContent(request);
        }
    }

    private async Task<string[]> GetDependenciesAsync(string bundlePath)
    {
        // 示例中暂时不返回依赖，实际可根据需求实现
        await Task.Yield();
        return new string[0];
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
}
