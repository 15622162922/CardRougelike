using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


public class BaseModule<T> : BaseModule where T : BaseModule<T>, new()
{
    private static BaseModule _module;

    public static BaseModule Module
    {
        get
        {
            if (_module == null)
            {
                _module = new T();
            }
            return _module;
        }
    }
}

public class BaseModule 
{
    public virtual void Register()
    {
    
    }

    public virtual void UnRegister()
    {
    
    }
}
