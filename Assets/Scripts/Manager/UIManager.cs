using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class UIManager : BaseManager<UIManager>
{
    /// <summary>
    /// Page栈
    /// </summary>
    private Stack<BaseWindow> _openPages;
    
    /// <summary>
    /// Popup栈
    /// </summary>
    private List<BaseWindow> _openPopups;
    
    /// <summary>
    /// Tips栈
    /// </summary>
    private List<BaseWindow> _openTips;
    
    /// <summary>
    /// 各层级的入栈函数
    /// </summary>
    private Dictionary<UIConst.UILayer, Action<BaseWindow>> _pullFunctions;
    
    /// <summary>
    /// 各层级的根节点OrderLayer
    /// </summary>
    private Dictionary<UIConst.UILayer, int> _rootOrderLayers;
    
    public override void Init()
    {
        base.Init();
        _uiOpenRequests = new List<UIOpenRequest>();
        _openPages = new Stack<BaseWindow>();
        _openPopups = new List<BaseWindow>();
        _openTips = new List<BaseWindow>();
        _pullFunctions = new Dictionary<UIConst.UILayer, Action<BaseWindow>>()
        {
            [UIConst.UILayer.Page] = PullOpenPage,
            [UIConst.UILayer.Popup] = PullOpenPopup,
            [UIConst.UILayer.Tips] = PullOpenTips,
        };
        _rootOrderLayers = new Dictionary<UIConst.UILayer, int>()
        {
            [UIConst.UILayer.Bottom] = 1000,
            [UIConst.UILayer.Page] = 2000,
            [UIConst.UILayer.Popup] = 3000,
            [UIConst.UILayer.Tips] = 4000,
            [UIConst.UILayer.Top] = 5000,
        };
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
        
    }

#region UI初始化
    private void OnOpenWindow(UIOpenRequest request, GameObject windowObj)
    {
        request.Window.windowName = request.WindowName;
        request.Window.Bind(windowObj);
        //UI入栈
        if (_pullFunctions.TryGetValue(request.UIConfig.Layer, out var func))
        {
            func(request.Window);
        }
    }

    /// <summary>
    /// 推入Page栈，同时只能显示一个界面，隐藏当前界面会显示上一个界面
    /// </summary>
    /// <param name="window"></param>
    private void PullOpenPage(BaseWindow window)
    {
        if (_openPages.Count > 0)
        {
            var lastWindow = _openPages.Peek();
            lastWindow.CallOnDisable();

            var _cacheList = new List<BaseWindow>();
            while (_openPages.Count > 0)
            {
                var page = _openPages.Pop();
                _cacheList.Add(page);
            }
            _cacheList.Reverse();
            foreach (var page in _cacheList)
            {
                
            }
        }
        _openPages.Push(window);
        
        window.CallOnOpen();
    }

    /// <summary>
    /// 推入Popup栈，可以同时显示多个界面，最新的在最前面
    /// </summary>
    /// <param name="window"></param>
    private void PullOpenPopup(BaseWindow window)
    {
        _openPopups.Insert(0, window);
        window.CallOnOpen();
    }

    /// <summary>
    /// 推入Tips栈，同时只能显示一个界面，仅在上一个界面被关闭后才能显示这个界面
    /// </summary>
    /// <param name="window"></param>
    private void PullOpenTips(BaseWindow window)
    {
        _openTips.Add(window);
    }

#endregion
    

    private void LoadUpdate()
    {
        if (_uiOpenRequests.Count > 0)
        {
            while (_uiOpenRequests.Count > 0)
            {
                var request = _uiOpenRequests[0];
                NewLoadManager.Instance.Load<GameObject>(request.UIConfig.PrefabPath, (o =>
                {
                    var windowObj = GameObject.Instantiate(o, GetRootByLayer(request.UIConfig.Layer));
                    OnOpenWindow(request, windowObj);
                }));
            }
        }
    }
}

public class UIOpenRequest
{
    public string WindowName;
    public UIConfigStruct UIConfig;
    public BaseWindow Window;
    public object[] Args;
}
