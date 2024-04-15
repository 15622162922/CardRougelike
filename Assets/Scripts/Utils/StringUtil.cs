using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringUtil
{
    public static string GetFirstLowerStr(string str)
    {
        return str.Substring(0, 1).ToLower() + str.Substring(1);
    }
}
