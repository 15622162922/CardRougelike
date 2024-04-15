using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

public static class Logger
{
    //关闭日志打印
    public static bool disableLog = false;

    public enum LogChannel
    {
        Log,
        Warning,
        Error,
    }

    #region 基础日志
    public static void Log(string tag, LogChannel channel, string logContent)
    {
        if (disableLog) return;
        Output(CombineContent(tag, logContent), channel);
    }
    #endregion

    #region 拼接日志
    public static void LogFormat(string tag, LogChannel channel, string logContent, params string[] args)
    {
        if (disableLog) return;
        string content = string.Format(logContent, args);
        Output(CombineContent(tag, content), channel);
    }
    #endregion

    #region 结构日志
    public static void LogList(string tag, LogChannel channel, IList list)
    {
        if (disableLog) return;
        StringBuilder builder = new StringBuilder();
        _getListContent(builder, list, 3, 1);
        string listContent = builder.ToString();
        Output(CombineContent(tag, listContent), channel);
    }

    private static void _getListContent(StringBuilder sb, IList list, int maxDepth, int currentDepth)
    {
        string indent = new string(' ', currentDepth * 2); //设置缩进
        sb.AppendFormat("%s{\n", indent);

        if (currentDepth >= maxDepth)
        {
            sb.Append($"List [{list.GetType()}]");
        }
        else
        {
            foreach (var item in list)
            {
                Type itemType = item.GetType();
                var properties = itemType.GetProperties();

                foreach (var property in properties)
                {
                    if (property.PropertyType == typeof(IList))
                    {
                        sb.Append($"{property.Name} (List<{itemType.Name}>){{\n");
                        var subList = (IList)property.GetValue(item);
                        _getListContent(sb, subList, maxDepth, currentDepth + 1);
                        sb.Append("}\n");
                    }
                    else
                    {
                        sb.Append($"{property.Name}: {property.PropertyType.Name} = {property.GetValue(item)}\n");
                    }
                }
            }
        }

        sb.AppendFormat("%s}\n", indent);
    }

    public static void LogDictionary(string tag, LogChannel channel, IDictionary dictionary)
    {
        if (disableLog) return;
        StringBuilder builder = new StringBuilder();
        _getDictionaryContent(builder, dictionary, 3, 1);
        string dictionaryContent = builder.ToString();
        Output(CombineContent(tag, dictionaryContent), channel);
    }

    private static void _getDictionaryContent(StringBuilder sb, IDictionary dictionary, int maxDepth, int currentDepth)
    {
        string indent = new string(' ', currentDepth * 2); //设置缩进
        sb.AppendFormat("%s{\n", indent);

        if (currentDepth >= maxDepth)
        {
            sb.Append("....");
        }
        else
        {
            foreach (DictionaryEntry entry in dictionary)
            {
                sb.Append($"{indent}[{entry.Key}] ({entry.Key.GetType().Name}) = {entry.Value} ({entry.Value.GetType().Name})\n");
                if (entry.Value is IDictionary subDictionary)
                {
                    _getDictionaryContent(sb, subDictionary, maxDepth, currentDepth + 1);
                }
            }
        }

        sb.AppendFormat("%s}\n", indent);
    }
    #endregion

    #region 底层函数
    private static string CombineContent(string tag, string content)
    {
        string output = $"[{tag}] {content}";
        return output;
    }

    private static void Output(string content, LogChannel channel)
    {
        switch (channel)
        {
            case LogChannel.Log:
                Debug.Log(content);
                break;

            case LogChannel.Warning:
                Debug.LogWarning(content);
                break;

            case LogChannel.Error:
                Debug.LogError(content);
                break;
        }
    }

    #endregion
}
