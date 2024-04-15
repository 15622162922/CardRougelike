using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseManager<T> : BaseManager where T: BaseManager<T>, new() 
{
    public string tag = typeof(T).ToString();
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new T();
                _instance.Init();
                GameManager.Instance.RegisterManager(_instance);
            }
            return _instance;
        }
    }

    public void Log(string log)
    {
        Logger.Log(tag, Logger.LogChannel.Log, log);
    }

    public void Warning(string log)
    {
        Logger.Log(tag, Logger.LogChannel.Warning, log);
    }

    public void Error(string log)
    {
        Logger.Log(tag, Logger.LogChannel.Error, log);
    }
}

public class BaseManager
{
    public virtual void Init()
    {

    }

    public virtual void Destroy()
    {

    }
}