using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OfficeOpenXml;

public static class EPPlusExtension
{
    public static ExcelRange GetExcelRange(this ExcelWorksheet sheet, int r1, int c1, int r2, int c2)
    {
        return sheet.Cells[r1 + 1, c1 + 1, r2 + 1, c2 + 1];
    }
    public static string GetValueInCell(this ExcelWorksheet sheet, int r, int c)
    {
        return sheet.Cells[r + 1, c + 1].Text;
    }
    public static ExcelWorksheet SetValueInCell(this ExcelWorksheet sheet, int r, int c, string text)
    {
        sheet.Cells[r + 1, c + 1].Value = text;
        return sheet;
    }
}
