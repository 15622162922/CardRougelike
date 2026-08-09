using System;
using System.Collections.Generic;

/// <summary>
///     缓存池管理器
/// </summary>
public class ObjectPoolManager : BaseManager<ObjectPoolManager>
{
    private Dictionary<Type, IPool> _registeredPools;

    public override void Init()
    {
        base.Init();
        _registeredPools = new Dictionary<Type, IPool>();
    }

    public override void Destroy()
    {
        foreach (var poolPair in _registeredPools)
        {
            poolPair.Value.Release();
            _registeredPools.Remove(poolPair.Key);
        }

        base.Destroy();
    }

    /// <summary>
    ///     注册一个缓存池，方便统一管理
    /// </summary>
    /// <param name="t"></param>
    /// <param name="pool"></param>
    internal void RegisterPool(Type t, IPool pool)
    {
        _registeredPools.TryAdd(t, pool);
    }

    /// <summary>
    ///     获取一个指定类型的对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T Get<T>() where T : class, new()
    {
        return ObjectPool<T>.Instance.Get();
    }

    /// <summary>
    ///     回收一个对象
    /// </summary>
    /// <param name="obj"></param>
    /// <typeparam name="T"></typeparam>
    public void Recycle<T>(T obj) where T : class, new()
    {
        ObjectPool<T>.Instance.Recycle(obj);
    }

    /// <summary>
    ///     预热，提前创建指定数量的对象
    /// </summary>
    /// <param name="count"></param>
    /// <typeparam name="T"></typeparam>
    public void Prewarm<T>(int count) where T : class, new()
    {
        ObjectPool<T>.Instance.Prewarm(count);
    }

    /// <summary>
    ///     设置缓存池最大数量
    /// </summary>
    /// <param name="maxCapacity"></param>
    /// <typeparam name="T"></typeparam>
    public void SetMaxCapacity<T>(int maxCapacity) where T : class, new()
    {
        ObjectPool<T>.Instance.SetMaxCapacity(maxCapacity);
    }

    /// <summary>
    ///     清空池对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private void Clear<T>() where T : class, new()
    {
        ObjectPool<T>.Instance.Clear();
    }

    /// <summary>
    ///     释放池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private void Release<T>() where T : class, new()
    {
        ObjectPool<T>.Instance.Release();
    }
}