using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UITipsLayer : BaseUILayer
{
    private Dictionary<BaseWindow, object[]> _paramCache = new Dictionary<BaseWindow, object[]>();
    public override UIConst.UILayer GetUILayer()
    {
        return UIConst.UILayer.Tips;
    }

    public override void OnOpenWindow(BaseWindow window, object[] param)
    {
        _openWindows.Add(window);
        if (_paramCache.ContainsKey(window))
        {
            _paramCache[window] = param;
        }
        else
        {
            _paramCache.Add(window, param);
        }
    }

    public override void OnCloseWindow(string windowName)
    {
        BaseWindow window = GetOpenWindow(windowName);
        if (window != null)
        {
            _openWindows.Remove(window);
            window.CallOnClose();
        }
        
        var lastWindow = GetLastWindow();
        var maxSortingOrder = GetMaxSortingOrder();
        lastWindow.SetSortingOrder(maxSortingOrder + GetSortingOrderInterval());
        var param = _paramCache[lastWindow];
        lastWindow.CallOnOpen(param);
    }

    public override BaseWindow GetLastWindow()
    {
        if (_openWindows.Count > 0)
        {
            return _openWindows.First();
        }
        
        return null;
    }

    public override bool IsOpen(string windowName)
    {
        return IsEnabled(windowName);
    }
}
