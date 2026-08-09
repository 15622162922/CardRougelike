using System;
using System.Collections.Generic;

/// <summary>
///     事件管理器
///     用于多对多式推送消息
/// </summary>
public class EventManager : BaseManager<EventManager>
{
    private Dictionary<string, List<Delegate>> m_eventHandlers;

    public override void Init()
    {
        base.Init();
        m_eventHandlers = new Dictionary<string, List<Delegate>>();
    }

    public override void Destroy()
    {
        m_eventHandlers.Clear();
        base.Destroy();
    }

    public bool RegisterEventHandler<T>(string eventName, Action<T> handler)
    {
        if (!m_eventHandlers.TryGetValue(eventName, out var handlers))
        {
            handlers = new List<Delegate>();
            m_eventHandlers.Add(eventName, handlers);
        }

        if (handlers.Contains(handler))
        {
            Error($"为{eventName}注册了重复的响应");
            return false;
        }

        handlers.Add(handler);
        return true;
    }

    public bool RegisterEventHandler(string eventName, Action handler)
    {
        if (!m_eventHandlers.TryGetValue(eventName, out var handlers))
        {
            handlers = new List<Delegate>();
            m_eventHandlers.Add(eventName, handlers);
        }

        if (handlers.Contains(handler))
        {
            Error($"为{eventName}注册了重复的响应");
            return false;
        }

        handlers.Add(handler);
        return true;
    }

    public void UnRegisterEventHandler<T>(string eventName, Action<T> handler)
    {
        if (m_eventHandlers.TryGetValue(eventName, out var handlers))
        {
            if (handlers.Contains(handler)) handlers.Remove(handler);

            if (handlers.Count == 0) m_eventHandlers.Remove(eventName);
        }
    }

    public void UnRegisterEventHandler(string eventName, Action handler)
    {
        if (m_eventHandlers.TryGetValue(eventName, out var handlers))
        {
            if (handlers.Contains(handler)) handlers.Remove(handler);

            if (handlers.Count == 0) m_eventHandlers.Remove(eventName);
        }
    }

    public void DispatchEvent<T>(string eventName, T eventParam)
    {
        if (m_eventHandlers.TryGetValue(eventName, out var handlers))
            foreach (var handler in handlers)
                ((Action<T>)handler)(eventParam);
    }

    public void DispatchEvent(string eventName)
    {
        if (m_eventHandlers.TryGetValue(eventName, out var handlers))
            foreach (var handler in handlers)
                ((Action)handler)();
    }
}