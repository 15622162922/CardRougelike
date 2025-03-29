using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TestCommand 
{
    [MenuItem("Test/TestCommand")]
    public static void Test()
    {
        //自由填写
        NewLoadManager.Instance.Load<GameObject>("Prefab/Character/Test_Player.prefab", o =>
        {
            GameObject.Instantiate(o);
            Logger.Log("TestCommand", Logger.LogChannel.Log, o.name);
        });
    }
}
