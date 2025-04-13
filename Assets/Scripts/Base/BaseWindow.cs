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

    protected virtual void OnSortingOrderUpdate(int sortingOrder)
    {
        
    }

    public GameObject gameObject;
    public string windowName;
    public int sortingOrder;

    public bool isEnabled;
    
    public void Bind(GameObject go)
    {
        gameObject = go;
    }

    public void SetSortingOrder(int sortingOrder)
    {
        this.sortingOrder = sortingOrder;
        var canvas = gameObject.GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.sortingOrder = sortingOrder;
            CallSortingOrderUpdate(sortingOrder);
        }
    }

    public void CallOnOpen(params object[] args)
    {
        OnOpen(args);
        
        OnEnable();
    }

    public void CallOnClose()
    {
        OnDisable();
        
        OnClose();
    }

    public void CallOnEnable()
    {
        isEnabled = true;
        
        OnEnable();
    }

    public void CallOnDisable()
    {
        isEnabled = false;
        
        OnDisable();
    }

    public void CallSortingOrderUpdate(int sortingOrder)
    {
        OnSortingOrderUpdate(sortingOrder);
    }
}
