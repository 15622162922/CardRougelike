using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     角色，拥有复杂逻辑和功能接口
/// </summary>
public class Role : Unit
{
    protected override void OnInit()
    {
        base.OnInit();
        m_roleComponents = new List<IRoleComponent>();
        m_commandFunction = new Dictionary<RoleConst.RoleCommandType, Action<RoleCommand>>();
        m_eventFunction = new Dictionary<string, List<Delegate>>();
    }

    protected override void OnRelease()
    {
        base.OnRelease();
        foreach (var component in m_roleComponents) component.Unbind();
        m_roleComponents.Clear();
        m_commandFunction.Clear();
        m_eventFunction.Clear();
    }

    #region RoleType

    /// <summary>
    ///     角色类型
    /// </summary>
    public RoleConst.RoleType RoleType
    {
        get => m_roleType;
        set => m_roleType = value;
    }

    protected RoleConst.RoleType m_roleType;

    public bool IsPlayer()
    {
        return m_roleType == RoleConst.RoleType.Player;
    }

    public bool IsMonster()
    {
        return m_roleType == RoleConst.RoleType.Monster;
    }

    public bool IsNpc()
    {
        return m_roleType == RoleConst.RoleType.Npc;
    }

    #endregion

    #region RoleComponent 用于将角色的逻辑拆分开

    protected List<IRoleComponent> m_roleComponents;

    /// <summary>
    ///     添加一个角色组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T AddRoleComponent<T>() where T : IRoleComponent
    {
        var component = m_roleComponents.Find(component => { return component.GetType() == typeof(T); });
        if (component != null)
        {
            Logger.Log(UnitName + GetUnitID(), Logger.LogChannel.Error, "添加了重复的角色组件");
            return (T)component;
        }

        var t = Activator.CreateInstance<T>();
        t.Bind(this);
        m_roleComponents.Add(t);

        return t;
    }

    /// <summary>
    ///     获取一个角色组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetRoleComponent<T>() where T : class, IRoleComponent
    {
        var component = m_roleComponents.Find(component => { return component.GetType() == typeof(T); });

        if (component == null) return null;

        return component as T;
    }

    /// <summary>
    ///     移除一个角色组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void RemoveRoleComponent<T>() where T : IRoleComponent
    {
        var component = m_roleComponents.Find(component => { return component.GetType() == typeof(T); });

        if (component != null)
        {
            component.Unbind();
            m_roleComponents.Remove(component);
        }
    }

    /// <summary>
    ///     是否拥有某个角色组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public bool HasRoleComponent<T>() where T : IRoleComponent
    {
        var component = m_roleComponents.Find(component => { return component.GetType() == typeof(T); });

        return component != null;
    }

    #endregion

    #region RoleCommand 用于从外部接收控制指令

    private Dictionary<RoleConst.RoleCommandType, Action<RoleCommand>> m_commandFunction;

    public void RegisterCommandType<T>(RoleConst.RoleCommandType roleCommandType, Action<T> function)
    {
        Action<RoleCommand> handler = command =>
        {
            if (command.Data is T)
                function((T)command.Data);
            else
                Debug.LogError($"{function}并不是指令{roleCommandType}的响应类型");
        };
        m_commandFunction[roleCommandType] = handler;
    }

    public void UnRegisterCommandType(RoleConst.RoleCommandType roleCommandType)
    {
        if (m_commandFunction.ContainsKey(roleCommandType))
        {
            if (m_commandFunction[roleCommandType] == null) m_commandFunction.Remove(roleCommandType);
        }
        else
        {
            Debug.LogError($"正在尝试移除未注册的指令响应{m_roleType}, {roleCommandType}");
        }
    }

    public void HandleRoleCommand(RoleCommand command)
    {
        if (m_commandFunction.ContainsKey(command.RoleCommandType))
            m_commandFunction[command.RoleCommandType]?.Invoke(command);
    }

    #endregion RoleCommand

    #region RoleEvent 用于在Component之间通信

    private Dictionary<string, List<Delegate>> m_eventFunction;

    public void RegisterRoleEvent<T>(string eventName, Action<T> handler)
    {
        if (!m_eventFunction.ContainsKey(eventName)) m_eventFunction[eventName] = new List<Delegate>();

        m_eventFunction[eventName].Add(handler);
    }

    public void RegisterRoleEvent(string eventName, Action handler)
    {
        if (!m_eventFunction.ContainsKey(eventName)) m_eventFunction[eventName] = new List<Delegate>();

        m_eventFunction[eventName].Add(handler);
    }

    public void UnRegisterRoleEvent<T>(string eventName, Action<T> handler)
    {
        if (m_eventFunction.ContainsKey(eventName))
            if (m_eventFunction[eventName].Contains(handler))
                m_eventFunction[eventName].Remove(handler);
    }

    public void UnRegisterRoleEvent(string eventName, Action handler)
    {
        if (m_eventFunction.ContainsKey(eventName))
            if (m_eventFunction[eventName].Contains(handler))
                m_eventFunction[eventName].Remove(handler);
    }

    public void DispatchRoleEvent<T>(string eventName, T param)
    {
        if (m_eventFunction.TryGetValue(eventName, out var eventFunctions))
            foreach (var handler in eventFunctions)
                ((Action)handler)();
    }

    public void DispatchRoleEvent(string eventName)
    {
        if (m_eventFunction.TryGetValue(eventName, out var eventFunctions))
            foreach (var handler in eventFunctions)
                ((Action)handler)();
    }

    #endregion RoleEvent
}