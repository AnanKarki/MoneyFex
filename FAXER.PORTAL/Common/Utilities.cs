using Microsoft.Office.Interop.Excel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Common
{
    public class Utilities
    {

        public FileStreamResult CreateExcelWorkSheet<T>(List<T> data, string fileName)
        {
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
            workSheet.Row(1).Style.Font.Bold = true;
            workSheet.Row(1).Style.Fill.PatternType = ExcelFillStyle.Solid;
            workSheet.Row(1).Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
            workSheet.Row(1).Style.Font.Color.SetColor(Color.White);
            workSheet.Cells[1, 1].LoadFromCollection(data, true);
            workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();
            workSheet.Cells[workSheet.Dimension.Address].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[workSheet.Dimension.Address].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[workSheet.Dimension.Address].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            workSheet.Cells[workSheet.Dimension.Address].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            var fileStream = new MemoryStream();
            excel.SaveAs(fileStream);
            fileStream.Position = 0;
            var fsr = new FileStreamResult(fileStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            fsr.FileDownloadName = fileName + ".xlsx";
            return fsr;
        }
    }
}