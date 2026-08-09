using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using UnityEngine;

public class ExcelReader
{
    /// <summary>
    ///     读取目录下的所有Excel文件
    /// </summary>
    /// <param name="directoryPath"></param>
    public static void ReadAllExcelFiles(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            Debug.LogError($"{directoryPath} does not exist");
            return;
        }

        var files = Directory.GetFiles(directoryPath, "*.xlsx");
        for (var i = 0; i < files.Length; i++) ReadExcelFile(files[i]);
    }

    /// <summary>
    ///     读取指定的Excel文件
    /// </summary>
    /// <param name="excelFilePath"></param>
    public static void ReadExcelFile(string excelFilePath)
    {
        using (var fs = new FileStream(excelFilePath, FileMode.Open, FileAccess.Read))
        {
            IWorkbook workbook = null;
            if (excelFilePath.EndsWith(".xlsx"))
                workbook = new XSSFWorkbook(fs);
            else if (excelFilePath.EndsWith(".xls")) workbook = new HSSFWorkbook(fs);

            if (workbook != null)
                for (var i = 0; i < workbook.NumberOfSheets; i++)
                {
                    var sheet = workbook.GetSheetAt(i);
                    //sheet.SheetName
                    for (var row = 0; row < sheet.LastRowNum; row++)
                    {
                        var currentRow = sheet.GetRow(row);
                        if (currentRow == null) continue;

                        for (var col = 0; col < currentRow.LastCellNum; col++)
                        {
                            var cell = currentRow.GetCell(col);
                            if (cell != null)
                            {
                                //cell.ToString()
                            }
                        }
                    }
                }
        }
    }
}