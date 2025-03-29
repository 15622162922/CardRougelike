using System;

/// <summary>
/// 基础加载器，提供加载方法
/// </summary>
public abstract class BaseLoader
{
    public abstract LoadTask<T> Load<T>(string path, Action<T> onComplete) where T : UnityEngine.Object;

    public abstract void Unload(string path, bool forceUnload = false);
}