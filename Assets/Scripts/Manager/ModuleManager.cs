using Sirenix.Serialization.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModuleManager : BaseManager<ModuleManager>
{
    List<BaseModule> moduleList;

    public override void Init()
    {
        base.Init();
        moduleList = new List<BaseModule>();
    }

    public override void Destroy()
    {
        base.Destroy();
    }

    public void RegisterModules()
    {
        RegisterModule(CharacterModule.Module);
        RegisterModule(MapModule.Module);
    }

    public void UnRegisterModules()
    {
        foreach (var module in moduleList)
        {
            module.UnRegister();
        }
    }

    void RegisterModule(BaseModule module)
    {
        if (!moduleList.Contains(module))
        {
            module.Register();
            moduleList.Add(module);
        }
    }
}
