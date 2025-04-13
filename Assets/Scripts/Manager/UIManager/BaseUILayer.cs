using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BaseUILayer 
{
    protected List<BaseWindow> _openWindows = new List<BaseWindow>();
    
    /// <summary>
    /// 通过界面名获取界面
    /// </summary>
    /// <param name="windowName">界面名</param>
    /// <returns>界面BaseWindow</returns>
    public BaseWindow GetOpenWindow(string windowName)
    {
        foreach (var window in _openWindows)
        {
            if (window.windowName == windowName)
            {
                return window;
            }
        }
        
        return null;
    }

    public virtual void OnOpenWindow(BaseWindow window, object[] param)
    {
        if (_openWindows.Count > 0)
        {
            var lastWindow = _openWindows.Last();
            lastWindow.CallOnDisable();
        }

        var maxSortingOrder = GetMaxSortingOrder();
        window.SetSortingOrder(maxSortingOrder + GetSortingOrderInterval());
        
        _openWindows.Add(window);
        window.CallOnOpen(param);
    }

    public virtual void OnCloseWindow(string windowName)
    {
        BaseWindow window = GetOpenWindow(windowName);
        if (window != null)
        {
            _openWindows.Remove(window);
            window.CallOnClose();

            if (_openWindows.Count > 0)
            {
                var lastWindow = GetLastWindow();
                lastWindow.CallOnEnable();
            }
        }
    }

    public List<BaseWindow> GetOpenWindows()
    {
        return _openWindows;
    }

    public virtual bool IsOpen(string windowName)
    {
        return _openWindows.Exists(w => w.windowName == windowName);
    }

    public virtual bool IsEnabled(string windowName)
    {
        var window = GetOpenWindow(windowName);
        if (window != null)
        {
            return window.isEnabled;
        }

        return false;
    }

    public int GetMaxSortingOrder()
    {
        if (_openWindows.Count > 0)
        {
            var lastWindow = GetLastWindow();
            return lastWindow.sortingOrder;
        }

        return GetBaseSortingOrder();
    }
    
    public virtual int GetBaseSortingOrder()
    {
        return UIConst.BaseSortingOrder[GetUILayer()];
    }

    public virtual int GetSortingOrderInterval()
    {
        return UIConst.SortingOrderInterval;
    }

    public virtual UIConst.UILayer GetUILayer()
    {
        return UIConst.UILayer.Bottom;
    }

    /// <summary>
    /// 获取最上层的界面
    /// </summary>
    /// <returns></returns>
    public virtual BaseWindow GetLastWindow()
    {
        if (_openWindows.Count > 0)
        {
            return _openWindows.Last();
        }
        return null;
    }
}
