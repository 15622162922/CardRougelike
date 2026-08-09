using System;
using System.Collections.Generic;

/// <summary>
///     消息管理器
///     多对一式推送数据
/// </summary>
public class MsgManager : BaseManager<MsgManager>
{
    private Dictionary<string, Delegate> m_msgHandlers;

    public override void Init()
    {
        base.Init();
        m_msgHandlers = new Dictionary<string, Delegate>();
    }

    public override void Destroy()
    {
        m_msgHandlers.Clear();
        base.Destroy();
    }

    public bool RegisterMsgHandler<T>(Action<T> handler)
    {
        var type = typeof(T);
        var msgName = type.Name;
        if (m_msgHandlers.ContainsKey(msgName))
        {
            Error($"重复注册消息响应：{msgName}");
            return false;
        }

        m_msgHandlers.Add(msgName, handler);
        return true;
    }

    public void UnRegisterMsgHandler<T>(Action<T> handler)
    {
        var type = typeof(T);
        var msgName = type.Name;
        if (m_msgHandlers.ContainsKey(msgName)) m_msgHandlers.Remove(msgName);
    }

    public void SendMsg<T>(T msg)
    {
        var type = typeof(T);
        var msgName = type.Name;
        if (m_msgHandlers.TryGetValue(msgName, out var handler)) ((Action<T>)handler)(msg);
    }
}