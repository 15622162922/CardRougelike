using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIConst 
{
    public enum UILayer
    {
        Bottom, //最底层背景
        Page, //全屏页面
        Popup, //非全屏弹窗
        Tips, //提示窗口
        Top, //最顶层覆盖
    }

    public static Dictionary<string, UIConfigStruct> UIConfig = new Dictionary<string, UIConfigStruct>()
    {
        ["UITestLoginView"] = new UIConfigStruct(){PrefabPath = "", Layer = UILayer.Page},
    };
}

public struct UIConfigStruct
{
    public string PrefabPath; //界面预制体路径
    public UIConst.UILayer Layer; //界面创建层级
    public bool MultiWindow; //支持同时打开多个窗口
}
