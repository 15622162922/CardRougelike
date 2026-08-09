using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using UnityEditor;
using UnityEngine;

public class ExcelToCodeGenerator : EditorWindow
{
    private const int FIELD_NAME_ROW = 0; //字段名所在行
    private const int FIELD_TYPE_ROW = 1; //字段类型所在行
    private const int FIELD_DESCRIPTION_ROW = 2; //字段描述所在行
    private const int FIELD_EXPORT_ROW = 3; //导出设置所在行
    private const int DATA_START_ROW = 4; //数据起始行

    private static readonly string ExcelFolder = "Excel";
    private static readonly string OutputFolder = "Assets/Scripts/Config/Game";
    private static readonly string TemplateFolder = "Assets/3rd/ExcelExport/Editor/Template";

    private static List<string> _sheetNamesCache;
    private static Dictionary<string, Dictionary<object, Dictionary<string, FieldData>>> _allSheetDatas;

    [MenuItem("Tools/Excel/导入配置表")]
    public static void Generate()
    {
        var excelFiles = Directory.GetFiles(ExcelFolder, "*.xlsx", SearchOption.AllDirectories);
        foreach (var file in excelFiles) ProcessExcel(file);

        AssetDatabase.Refresh();
        Debug.Log("Excel导入完成");
    }

    // 生成Cs接口脚本
    private static void ProcessExcel(string excelPath)
    {
        var excelName = Path.GetFileNameWithoutExtension(excelPath);
        var relativeFolder =
            Path.GetDirectoryName(excelPath).Replace(ExcelFolder, "").Trim(Path.DirectorySeparatorChar);

        var outputDir = Path.Combine(OutputFolder, relativeFolder);
        Directory.CreateDirectory(outputDir);

        var outputPath = Path.Combine(outputDir, $"Config_{excelName}.cs");

        //读取模板
        var classTemplate = File.ReadAllText(Path.Combine(TemplateFolder, "ConfigClassTemplate.txt"));
        var sheetTemplate = File.ReadAllText(Path.Combine(TemplateFolder, "SheetClassTemplate.txt"));

        //解析Excel
        var sheetInfos = ParseExcel(excelPath); //Sheet的格式
        var sheetDatas = ParseExcelData(excelPath); //Sheet的数据

        //拼接所有Sheet类
        var sheetClasses = new StringBuilder();
        SheetInfo mainSheet = null;
        FieldInfo keyField = null;
        for (var i = 0; i < sheetInfos.Count; i++)
            // foreach (var sheet in sheetInfos)
        {
            var sheet = sheetInfos[i];
            var isMain = i == 0;
            if (isMain)
            {
                mainSheet = sheet;
                foreach (var field in sheet.Fields)
                {
                    if (field.IsKey) keyField = field;
                    break;
                }
            }

            var cls = sheetTemplate.Replace("{{SheetName}}", $"Sheet_{sheet.SheetName}").Replace("{{Fields}}",
                string.Join("\n", sheet.Fields.Select(f => $"       // {f.Desc}\n        public {f.Type} {f.Name};")));
            sheetClasses.AppendLine(cls);
        }

        if (mainSheet != null && keyField != null)
        {
            //生成接口代码
            var cls = classTemplate.Replace("{{KeyType}}", keyField.Type)
                .Replace("{{MainSheetType}}", $"Sheet_{mainSheet.SheetName}")
                .Replace("{{ExcelName}}", excelName)
                .Replace("{{SheetClasses}}", sheetClasses.ToString());
            File.WriteAllText(outputPath, cls);

            //生成json数据

            AssetDatabase.Refresh();
            Debug.Log($"导出{outputPath}成功");
        }
        else
        {
            Debug.LogError($"{excelName}缺少Key或者主Sheet内容");
        }
    }

    // 解析Excel格式
    private static List<SheetInfo> ParseExcel(string path)
    {
        var sheetInfos = new List<SheetInfo>();
        using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            IWorkbook workbook = null;
            if (path.EndsWith(".xlsx")) workbook = new XSSFWorkbook(fs);
            else if (path.EndsWith(".xls")) workbook = new HSSFWorkbook(fs);

            if (workbook != null)
            {
                for (var i = 0; i < workbook.NumberOfSheets; i++)
                {
                    var sheet = workbook.GetSheetAt(i);
                    if (sheet == null) continue;
                    var sheetInfo = new SheetInfo(sheet.SheetName);

                    // 字段名行
                    var fieldNameRow = sheet.GetRow(FIELD_NAME_ROW);
                    var fieldTypeRow = sheet.GetRow(FIELD_TYPE_ROW);
                    var fieldExportRow = sheet.GetRow(FIELD_EXPORT_ROW);
                    var fieldDescRow = sheet.GetRow(FIELD_DESCRIPTION_ROW);

                    for (var c = 0; c < fieldNameRow.LastCellNum; c++)
                    {
                        var name = fieldNameRow.GetCell(c)?.ToString();
                        var type = fieldTypeRow.GetCell(c)?.ToString();
                        var export = fieldExportRow.GetCell(c)?.ToString();
                        var desc = fieldDescRow.GetCell(c)?.ToString();

                        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(type) && export != "N")
                            sheetInfo.Fields.Add(new FieldInfo(name, type, desc, export));
                    }

                    sheetInfos.Add(sheetInfo);
                }

                fs.Close();
            }
        }

        return sheetInfos;
    }

    // 生成数据Json表格
    // private static void ProcessJson(List<SheetInfo> sheetInfos)
    // {
    //     var _allSheetDatas =
    //         new Dictionary<string, Dictionary<object, Dictionary<string, object>>>();
    //     for (var i = 0; i < sheetInfos.Count; i++)
    //     {
    //         var sheetInfo = sheetInfos[i];
    //         var sheetName = sheetInfo.SheetName;
    //         var fieldDict = new Dictionary<string, object>();
    //
    //         foreach (var field in sheetInfo.Fields)
    //         {
    //             var type = field.Type;
    //             var name = field.Name;
    //             var isKey = field.IsKey;
    //         }
    //     }
    // }

    // 解析Excel中的数据
    private static Dictionary<object, Dictionary<string, object>> ParseExcelData(string path)
    {
        _allSheetDatas =
            new Dictionary<string, Dictionary<object, Dictionary<string, FieldData>>>();

        using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            IWorkbook workbook = null;
            if (path.EndsWith(".xlsx")) workbook = new XSSFWorkbook(fs);
            else if (path.EndsWith(".xls")) workbook = new HSSFWorkbook(fs);

            if (workbook != null)
            {
                _sheetNamesCache = new List<string>();
                //遍历所有Sheet名，得到allSheetDatas缓存
                for (var i = 0; i < workbook.NumberOfSheets; i++)
                {
                    var sheet = workbook.GetSheetAt(i);
                    if (sheet == null) continue;

                    _sheetNamesCache.Add(sheet.SheetName);
                    // 字段名行
                    var fieldNameRow = sheet.GetRow(FIELD_NAME_ROW);
                    var fieldTypeRow = sheet.GetRow(FIELD_TYPE_ROW);
                    var fieldExportRow = sheet.GetRow(FIELD_EXPORT_ROW);
                    var fieldDescRow = sheet.GetRow(FIELD_DESCRIPTION_ROW);
                    var fieldKeyColum = -1;

                    for (var c = 0; c < fieldNameRow.LastCellNum; c++)
                    {
                        var export = fieldExportRow.GetCell(c)?.ToString();
                        if (export == "K")
                        {
                            fieldKeyColum = c;
                            break;
                        }
                    }

                    if (fieldKeyColum != -1)
                    {
                        var keyDict =
                            new Dictionary<object, Dictionary<string, FieldData>>();
                        var keyType = fieldTypeRow.GetCell(fieldKeyColum).ToString();
                        var keyName = fieldNameRow.GetCell(fieldKeyColum).ToString();
                        for (var r = DATA_START_ROW; r < sheet.GetColumnWidth(fieldKeyColum); r++) //逐行
                        {
                            var rowData = sheet.GetRow(r);
                            var keyField = new FieldData(keyName, keyType, rowData.GetCell(fieldKeyColum).ToString());

                            // var key = ConvertValue(rowData.GetCell(fieldKeyColum).ToString(), keyType);
                            var fieldDict = new Dictionary<string, FieldData>();
                            for (var c = 0; c < rowData.LastCellNum; c++) //行内的每一列
                            {
                                var name = fieldNameRow.GetCell(c)?.ToString();
                                var type = fieldTypeRow.GetCell(c)?.ToString();
                                var valueField = new FieldData(name, type, rowData.GetCell(fieldKeyColum).ToString());
                                fieldDict.Add(name, valueField);
                            }

                            keyDict.Add(keyField.GetValue(), fieldDict);
                        }

                        _allSheetDatas.Add(sheet.SheetName, keyDict);
                    }
                }

                //todo 处理嵌套表的方法：封装一个解析指定Sheet、指定key下数据的方法，不再解析所有Sheets，只解析主Sheet，遇到嵌套的情况就去对应Sheet、key中查找。
                //遍历主Sheet，将嵌套表中的数据递归成对应sheet表
                var mainSheet = workbook.GetSheetAt(0);
                if (mainSheet == null) return null;
                var mainSheetName = mainSheet.SheetName;
                var mainSheetData = _allSheetDatas[mainSheetName];
                if (mainSheetData != null)
                {
                    var mainSheetInfo = new Dictionary<object, Dictionary<string, object>>();
                    foreach (var data in mainSheetData)
                    {
                        var key = data.Key;
                        var values = data.Value;
                        if (mainSheetInfo.ContainsKey(key))
                        {
                            Debug.LogError($"检测到重复的Key: {key}");
                            return null;
                        }

                        var fieldDict = new Dictionary<string, object>();
                        foreach (var field in values)
                        {
                            var fieldType = field.Value.Type;
                            if (IsNesting(fieldType))
                            {
                                var value = field.Value.GetValue();
                                var sheetData = _allSheetDatas[fieldType];
                                // 用字典结构替代id
                                //todo 这里应该用个递归
                                fieldDict.Add(field.Key, sheetData[value]);
                            }
                            else
                            {
                                fieldDict.Add(field.Key, field.Value.GetValue());
                            }
                        }

                        mainSheetInfo.Add(key, fieldDict);
                    }

                    return mainSheetInfo;
                }
            }

            fs.Close();
        }

        return null;
    }

    //主要依赖Sheet名的生成
    private static object ConvertValue(string value, string typeName)
    {
        if (string.IsNullOrEmpty(value)) return null;

        if (typeName == "int") return int.Parse(value);
        if (typeName == "float") return float.Parse(value);
        if (typeName == "bool") return bool.Parse(value);
        if (typeName.StartsWith("List"))
        {
            var match = Regex.Match(typeName, @"List<([^>]+)>");
            if (match.Success)
            {
                var content = match.Groups[1].Value;
                var listValues = value.Split("#");
                var list = new List<object>();
                for (var i = 0; i < listValues.Length; i++)
                {
                    var listValue = ConvertValue(listValues[i], content);
                    list.Add(listValue);
                }

                return list;
            }

            Debug.LogError($"无法解析的格式: {typeName}");
            return value;
        }

        return value;
    }

    // 是否为嵌套类型
    private static bool IsNesting(string typeName)
    {
        return _sheetNamesCache.Contains(typeName);
    }

    // 递归用字典结构替换value
    private static object GetNestingValue(string typeName, object key)
    {
        var sheetData = _allSheetDatas[typeName];
        if (sheetData == null) return null;
        if (sheetData.TryGetValue(key, out var fields))
            foreach (var field in fields)
                if (IsNesting(field.Value.Type))
                {
                }

        return null;
    }

    private class SheetInfo
    {
        public readonly List<FieldInfo> Fields = new();
        public readonly string SheetName;

        public SheetInfo(string name)
        {
            SheetName = name;
        }
    }

    private class FieldInfo
    {
        public readonly string Desc;
        public readonly string Export;
        public readonly bool IsKey;
        public readonly string Name;
        public readonly string Type;

        public FieldInfo(string name, string type, string desc, string export)
        {
            Name = name;
            Type = type;
            Desc = desc;
            Export = export;
            IsKey = Export == "K";
        }
    }

    private class FieldData
    {
        public readonly string Name;
        public readonly string Type;
        public readonly string Value;

        public FieldData(string name, string type, string value)
        {
            Name = name;
            Type = type;
            Value = value;
        }

        public object GetValue()
        {
            return ConvertValue(Value, Type);
        }
    }
}