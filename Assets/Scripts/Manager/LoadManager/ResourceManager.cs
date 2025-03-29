using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 引用计数器
/// </summary>
public class ResourceManager : BaseManager<ResourceManager>
{
    private Dictionary<string, ResourceReference> resourceMap = new Dictionary<string, ResourceReference>();
    public bool RESOURCE_UNLOAD = false; //是否开启资源卸载
    
    public void AddReference(string path) 
    {
        if (resourceMap.ContainsKey(path))
        {
            var data = resourceMap[path];
            data.refCount++;
            resourceMap[path] = data;
        }
        else
        {
            var data = new ResourceReference();
            data.path = path;
            data.refCount = 1;
            resourceMap[path] = data;
        }
    }

    public void ReleaseReference(string path)
    {
        if (resourceMap.ContainsKey(path))
        {
            var data = resourceMap[path];
            data.refCount--;
            if (data.refCount <= 0 && RESOURCE_UNLOAD)
            {
                NewLoadManager.Instance.Unload(path);
            }
            else
            {
                resourceMap[path] = data;
            }
        }
    }

    public void SetUnload(bool unload)
    {
        RESOURCE_UNLOAD = unload;
    }
}

public class ResourceReference
{
    public string path;
    public int refCount;
}