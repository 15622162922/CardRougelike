using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class AssetDatabaseLoader : BaseLoader
{
    //模拟的加载时间
    public float SIMULATED_TIME = 0.1f;
    public override LoadTask<T> Load<T>(string path, Action<T> onComplete)
    {
        //自动添加资源文件夹路径
        path = $"Assets/RawResources/{path}";
        var task = new LoadTask<T>();
        task.Path = path;
        if (onComplete != null)
            task.AddCallback(onComplete);
        // 启动异步加载流程
        LoadInternal(path, task);
        return task;
    }

    private async void LoadInternal<T>(string path, LoadTask<T> task) where T : UnityEngine.Object
    {
        float simulatedTime = 0f;
        while (simulatedTime < SIMULATED_TIME)
        {
            if (task.IsCanceled)
                return;
            if (!task.IsPaused)
            {
                simulatedTime += 0.1f;
                float progress = Mathf.Clamp01(simulatedTime / SIMULATED_TIME);
                task.SetProgress(progress);
            }

            await Task.Delay(100); //100毫秒更新一次进度
        }
            
        task.SetProgress(1f);
        task.Complete(AssetDatabase.LoadAssetAtPath<T>(path));
        try
        {
            
        }
        catch (Exception e)
        {
            Logger.Log("AssetDatabaseLoader", Logger.LogChannel.Error, $"无法加载{path}：{e.Message}");
        }
    }

    public override void Unload(string path, bool forceUnload = false)
    {
        //AssetDatabase不需要手动卸载
    }
}
