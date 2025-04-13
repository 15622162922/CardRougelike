using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class UIManager : BaseManager<UIManager>
{
    /// <summary>
    /// 各个层级的管理器
    /// </summary>
    private Dictionary<UIConst.UILayer, BaseUILayer> _layers = new Dictionary<UIConst.UILayer, BaseUILayer>();
    
    public override void Init()
    {
        base.Init();
        _uiOpenRequests = new List<UIOpenRequest>();
        
        _layers.Add(UIConst.UILayer.Page, new UIPageLayer());
        _layers.Add(UIConst.UILayer.Popup, new UIPopupLayer());
        _layers.Add(UIConst.UILayer.Tips, new UITipsLayer());
        
        UpdateManager.Instance.RegisterUpdate(LoadUpdate);
    }

    public override void Destroy()
    {
        base.Destroy();
    }

#region UIRoot

    public GameObject UIRoot;
    GameObject bottomRoot;
    GameObject pageRoot;
    GameObject popupRoot;
    GameObject tipsRoot;
    GameObject topRoot;

    public Dictionary<UIConst.UILayer, Transform> layerRoots;
    
    /// <summary>
    /// 创建UI根节点
    /// </summary>
    public void CreateUIRoot()
    {
        NewLoadManager.Instance.Load<GameObject>("Prefab/Root/UIRoot.prefab", OnUIRootOpened);
    }

    /// <summary>
    /// 创建UI根节点完成
    /// </summary>
    /// <param name="obj">根节点GameObject</param>
    private void OnUIRootOpened(GameObject obj)
    {
        UIRoot = GameObject.Instantiate(obj);
        GameObject.DontDestroyOnLoad(UIRoot);
        PropsProxy proxy = UIRoot.GetComponent<PropsProxy>();
        bottomRoot = proxy.GetGameObject("bottomRoot");
        pageRoot = proxy.GetGameObject("pageRoot");
        popupRoot = proxy.GetGameObject("popupRoot");
        tipsRoot = proxy.GetGameObject("tipsRoot");
        topRoot = proxy.GetGameObject("topRoot");
        layerRoots = new Dictionary<UIConst.UILayer, Transform>()
        {
            [UIConst.UILayer.Bottom] = bottomRoot.transform,
            [UIConst.UILayer.Page] = pageRoot.transform,
            [UIConst.UILayer.Popup] = popupRoot.transform,
            [UIConst.UILayer.Tips] = tipsRoot.transform,
            [UIConst.UILayer.Top] = topRoot.transform,
        };
    }

    /// <summary>
    /// 根据界面的Layer来获取根节点
    /// </summary>
    /// <param name="layer"></param>
    /// <returns></returns>
    public Transform GetRootByLayer(UIConst.UILayer layer)
    {
        if (layerRoots.TryGetValue(layer, out var root))
        {
            return root;
        }

        return null;
    }
#endregion

    /// <summary>
    /// UI加载请求队列
    /// </summary>
    private List<UIOpenRequest> _uiOpenRequests;

    /// <summary>
    /// 打开一个窗口
    /// </summary>
    /// <param name="windowName">窗口名称</param>
    /// <param name="args">参数</param>
    public void OpenWindow<T>(string windowName, params object[] args) where T:BaseWindow
    {
        if (UIConst.UIConfig.TryGetValue(windowName, out var uiStruct))
        {
            //如果是可以支持同时开启多个窗口的界面，则直接建立加载请求
            if (uiStruct.MultiWindow)
            {
                T window = System.Activator.CreateInstance<T>();
                _uiOpenRequests.Add(new UIOpenRequest(){WindowName = windowName, UIConfig = uiStruct, Window = window, Args = args});
            }
            else
            {
                foreach (var request in _uiOpenRequests)
                {
                    //已经有加载请求了，更新参数
                    if (request.WindowName == windowName)
                    {
                        request.Args = args;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 关闭一个窗口
    /// </summary>
    /// <param name="windowName">窗口名称</param>
    /// <param name="window">对于能打开多个窗口的界面，需要传入指定的窗口关闭</param>
    public void CloseWindow(string windowName, BaseWindow window = null)
    {
        var uiLayer = GetUILayer(windowName);
        uiLayer.OnCloseWindow(windowName);
    }

#region UI初始化
    private void OnOpenWindow(UIOpenRequest request, GameObject windowObj)
    {
        request.Window.windowName = request.WindowName;
        request.Window.Bind(windowObj);
        windowObj.AddComponent<Canvas>();
        //UI入栈
        var uiLayer = GetUILayer(request.WindowName);
        uiLayer.OnOpenWindow(request.Window, request.Args);
    }
#endregion
    

    private void LoadUpdate()
    {
        if (_uiOpenRequests.Count > 0)
        {
            while (_uiOpenRequests.Count > 0)
            {
                var request = _uiOpenRequests[0];
                _uiOpenRequests.RemoveAt(0);
                NewLoadManager.Instance.Load<GameObject>(request.UIConfig.PrefabPath, (o =>
                {
                    var windowObj = GameObject.Instantiate(o, GetRootByLayer(request.UIConfig.Layer));
                    OnOpenWindow(request, windowObj);
                }));
            }
        }
    }
    
    public UIConfigStruct GetUIStruct(string windowName)
    {
        if (UIConst.UIConfig.TryGetValue(windowName, out var uiStruct))
        {
            return uiStruct;
        }

        return uiStruct;
    }

    public BaseUILayer GetUILayer(string windowName)
    {
        var uiStruct = GetUIStruct(windowName);
        if (_layers.TryGetValue(uiStruct.Layer, out var layer))
        {
            return layer;
        }
        
        return null;
    }

    public bool IsOpen(string windowName)
    {
        var uiLayer = GetUILayer(windowName);
        return uiLayer.IsOpen(windowName);
    }

    public bool IsEnabled(string windowName)
    {
        var uiLayer = GetUILayer(windowName);
        return uiLayer.IsEnabled(windowName);
    }
}

public class UIOpenRequest
{
    public string WindowName;
    public UIConfigStruct UIConfig;
    public BaseWindow Window;
    public object[] Args;
}
