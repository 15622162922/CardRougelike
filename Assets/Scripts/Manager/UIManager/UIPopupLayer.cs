using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPopupLayer : BaseUILayer
{
    public override UIConst.UILayer GetUILayer()
    {
        return UIConst.UILayer.Popup;
    }

    public override void OnOpenWindow(BaseWindow window, object[] param)
    {
        var maxSortingOrder = GetMaxSortingOrder();
        window.SetSortingOrder(maxSortingOrder + GetSortingOrderInterval());
        
        _openWindows.Add(window);
        window.CallOnOpen(param);
    }

    public override void OnCloseWindow(string windowName)
    {
        BaseWindow window = GetOpenWindow(windowName);
        if (window != null)
        {
            _openWindows.Remove(window);
            window.CallOnClose();
            
            
        }
    }
}
