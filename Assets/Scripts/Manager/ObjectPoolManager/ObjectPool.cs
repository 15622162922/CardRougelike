using System;
using System.Collections.Generic;

/// <summary>
///     单个对象池，只提供获取和回收两种外部接口，以及一个销毁接口
/// </summary>
/// <typeparam name="T">对象池中对象的类型</typeparam>
public class ObjectPool<T> : IPool where T : class, new()
{
    private static ObjectPool<T> _instance;
    // public ObjectPool(int maxCapacity = -1)
    // {
    //     MaxCapacity = maxCapacity;
    // }

    private readonly Stack<T> m_idleObjects = new();

    private int MaxCapacity = -1;

    public static ObjectPool<T> Instance
    {
        get
        {
            if (_instance == null) ;
            {
                _instance = new ObjectPool<T>();
                ObjectPoolManager.Instance.RegisterPool(typeof(T), _instance);
            }

            return _instance;
        }
    }

    public Type ObjectType => typeof(T);
    public int Count => m_idleObjects.Count;

    /// <summary>
    ///     释放池
    /// </summary>
    public void Release()
    {
        m_idleObjects.Clear();
        _instance = null;
    }

    /// <summary>
    ///     获取
    /// </summary>
    /// <returns></returns>
    public T Get()
    {
        if (m_idleObjects.Count == 0) return new T();

        var idleObject = m_idleObjects.Pop();
        if (idleObject is IPoolable p) p.OnGet();
        return idleObject;
    }

    /// <summary>
    ///     回收
    /// </summary>
    /// <param name="obj"></param>
    public void Recycle(T obj)
    {
        if (obj == null) return;
        if (obj is IPoolable p) p.OnRecycle();
        if (MaxCapacity > 0 && Count >= MaxCapacity) return;
        m_idleObjects.Push(obj);
    }

    /// <summary>
    ///     预创建
    /// </summary>
    /// <param name="count"></param>
    public void Prewarm(int count)
    {
        for (var i = 0; i < count && Count < MaxCapacity; i++)
        {
            var obj = new T();
            if (obj is IPoolable p) p.OnRecycle();
            m_idleObjects.Push(obj);
        }
    }

    /// <summary>
    ///     设置缓存池中最大数量
    /// </summary>
    /// <param name="maxCapacity"></param>
    public void SetMaxCapacity(int maxCapacity)
    {
        MaxCapacity = maxCapacity;
    }

    /// <summary>
    ///     清空对象池
    /// </summary>
    public void Clear()
    {
        m_idleObjects.Clear();
    }
}