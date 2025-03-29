using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseWindow
{
    /// <summary>
    /// 界面开启时最早执行一次
    /// </summary>
    /// <param name="args"></param>
    protected abstract void OnOpen(params object[] args);
    
    /// <summary>
    /// 界面销毁前最后执行一次
    /// </summary>
    protected abstract void OnClose();

    /// <summary>
    /// 界面开启或显示后执行一次
    /// </summary>
    protected virtual void OnEnable()
    {
        
    }

    /// <summary>
    /// 界面隐藏或销毁前执行一次
    /// </summary>
    protected virtual void OnDisable()
    {
        
    }

    /// <summary>
    /// 从外部新打开时执行一次
    /// </summary>
    /// <param name="args"></param>
    protected virtual void OnRefresh(params object[] args)
    {
        
    }

    public GameObject gameObject;
    public string windowName;
    
    public void Bind(GameObject go)
    {
        gameObject = go;
    }

    public void CallOnOpen()
    {
        
    }

    public void CallOnClose()
    {
        
    }

    public void CallOnEnable()
    {
        
    }

    public void CallOnDisable()
    {
        
    }

    public void CallOrderLayerUpdate(int sortingLayer)
    {
        
    }
}
