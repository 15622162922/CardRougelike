using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 加载任务
/// </summary>
/// <typeparam name="T"></typeparam>
public class LoadTask<T>:LoadTask
{
    public T Result { get; private set; }

    private List<Action<T>> _onCompleteCallbacks = new List<Action<T>>();

    public void AddCallback(Action<T> callback)
    {
        if (callback != null)
            _onCompleteCallbacks.Add(callback);
    }

    internal void Complete(T result)
    {
        IsCompleted = true;
        Result = result;
        foreach(var cb in _onCompleteCallbacks)
        {
            // ResourceManager.Instance.AddReference(this.Path);
            cb(result);
        }
    }
}

public class LoadTask
{
    public string Path { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsPaused { get; set; }
    public bool IsCanceled { get; set; }
    public float Progress { get; set; }

    public void Pause() { IsPaused = true; }

    public void Resume() { IsPaused = false; }

    public void Cancel() { IsCanceled = true; }

    internal void SetProgress(float progress)
    {
        Progress = progress;
    }
}
