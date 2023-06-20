
using OfficeOpenXml;
using System.Text.RegularExpressions;

namespace BotAit
{
    public class ExcelParse
    {
        public static async Task<string> readExcel(string gruppName)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            string pathToExcel = Path.Combine(Environment.CurrentDirectory, @"расписание.xlsx");

            string searchValue = gruppName;
            Console.WriteLine("________________Номер группы: " + gruppName + " ________________");

            using (var package = new ExcelPackage(new FileInfo(pathToExcel)))
            {

                var worksheet = package.Workbook.Worksheets[0];

                var cell = worksheet.Cells.FirstOrDefault(c => c.Value != null && c.Value.ToString() == searchValue);
                

                if (cell != null)
                {

                    var lastRow = worksheet.Dimension.End.Row;
                    var lastColumn = worksheet.Dimension.End.Column;
                    string resultString="";

                    string[] daysOfWeek = { "Пон", "Вто", "Сре", "Чет", "Пят", "Суб" };
                    List<string> arrPara = new List<string>();

                    var rowsWithValues = worksheet.Cells
                        .Where(c => c.Value != null && c.Value.ToString() == searchValue)
                        .Select(c => c.Start.Row)
                        .Distinct();

                    var outputRange = worksheet.Cells[rowsWithValues.Min(), cell.Start.Column + 1, rowsWithValues.Max() + 10, lastColumn];

                    var columnWidths = Enumerable.Range(outputRange.Start.Column, outputRange.Columns)
                        .Select(col => outputRange.Worksheet.Cells[outputRange.Start.Row, col, outputRange.End.Row, col]
                            .Max(cell => cell.Value?.ToString().Length ?? 0))
                        .ToArray();
                   
                    for (int row = outputRange.Start.Row; row <= outputRange.End.Row; row += 2) 
                    {

                        string strFirst = "";
                        string strSec = "";
                        string safeDay = "";


                        for (int col = outputRange.Start.Column; col <= outputRange.End.Column; col++)
                        {
                            
                            var cellValue = worksheet.Cells[row, col].Value?.ToString();
                            var cellWidth = columnWidths[col - outputRange.Start.Column] + 1;
                            var formattedCellValue = cellValue?.PadRight(cellWidth).Substring(0, cellWidth);

                            if (cellValue != null && cellValue.Length > 2)
                            {
                                if (Regex.IsMatch(cellValue, @"^[а-яА-ЯёЁ.\s-(),/""]+$"))
                                {
                                    strSec = daysOfWeek[(int)(col / 5) - 1] + "_" + cellValue;

                                    if (strFirst != strSec)
                                    {
                                        strFirst = daysOfWeek[(int)(col / 5) - 1] + "_" + cellValue;
                                        arrPara.Add($"|{daysOfWeek[(int)(col / 5) - 1] + "_" + formattedCellValue}");
                                    }
                                }

                                else if (!cellValue.Contains("-") && Regex.IsMatch(cellValue, @"\d+\.\d+") || cellValue.Contains("с/з"))
                                {

                                    if (!safeDay.Contains(daysOfWeek[(int)(col / 5) - 1]))
                                    {
                                        // если одинаковый даты то мы не добавляем разделитель иначе разделяем и получаем разные ячейки
                                        safeDay = daysOfWeek[(int)(col / 5) - 1] + "_" + formattedCellValue;
                                        arrPara.Add($"|{daysOfWeek[(int)(col / 5) - 1] + "_" + formattedCellValue}");
                                    }
                                    else 
                                    {
                                        safeDay = daysOfWeek[(int)(col / 5) - 1] + "_" + formattedCellValue;
                                        arrPara.Add($"{"/ "+ formattedCellValue}");
                                    }

                                }
                            }
                        }
                        arrPara.Add("|#");
                    }
                    resultString = string.Join("",arrPara);
                    return resultString;
                }

                return "Нет такой группы";
            }
        }
    } 
}

//можно попытаться дописать вывод преподов inWork
//// Output the column range to the console
//for (int row = outputRange.Start.Row; row <= outputRange.End.Row; row++) // only iterate over odd rows
//{
//    string strFirst = "";
//    string strSec = "";
//    string safeDay = "";
//    string safePrepod = "";
//    int oneGrupp = 0;
//    bool end = false;
//    for (int col = outputRange.Start.Column; col <= outputRange.End.Column; col++)
//    {

//        var cellValue = worksheet.Cells[row, col].Value?.ToString();
//        var cellWidth = columnWidths[col - outputRange.Start.Column] + 1;
//        var formattedCellValue = cellValue?.PadRight(cellWidth).Substring(0, cellWidth);

//        if (cellValue != null && cellValue.Length > 2)
//        {
//            oneGrupp++;

//            if (Regex.IsMatch(cellValue, @"^[а-яА-ЯёЁ.\s-(),.]+$") || cellValue.Contains("(2)"))
//            {
//                if (Regex.IsMatch(cellValue, @"\b[A-ЯЁ]\. ?[A-ЯЁ]\."))
//                {
//                    if (!safePrepod.Contains(daysOfWeek[(int)(col / 5) - 1]))
//                    {
//                        safePrepod = daysOfWeek[(int)(col / 5) - 1] + "_" + formattedCellValue;
//                        arrPara.Add($"|{daysOfWeek[(int)(col / 5) - 1] + "_" + formattedCellValue}");
//                    }

//                    else
//                    {
//                        safePrepod = daysOfWeek[(int)(col / 5) - 1] + "_" + formattedCellValue;
//                        arrPara.Add($"{"/ " + formattedCellValue}");
//                    }
//                }

//                else
//                {
//                    strSec = daysOfWeek[(int)(col / 5) - 1] + "_" + cellValue;

//                    if (strFirst != strSec)
//                    {
//                        if (daysOfWeek[(int)(col / 5) - 1] == "Пон" || oneGrupp % 2 != 0)
//                        {
//                            strFirst = daysOfWeek[(int)(col / 5) - 1] + "_" + cellValue;
//                            end = true;
//                            arrPara.Add($"|#|{daysOfWeek[(int)(col / 5) - 1] + "_" + formattedCellValue}");
//                        }
//                        else
//                        {
//                            strFirst = daysOfWeek[(int)(col / 5) - 1] + "_" + cellValue;
//                            arrPara.Add($"|{daysOfWeek[(int)(col / 5) - 1] + "_" + formattedCellValue}");
//                        }

//                    }

//                }
//            }

//            else if (!cellValue.Contains("-") && Regex.IsMatch(cellValue, @"\d+\.\d+") || cellValue.Contains("с/з"))
//            {
//                if (!safeDay.Contains(daysOfWeek[(int)(col / 5) - 1]))
//                {
//                    // если одинаковый даты то мы не добавляем разделитель иначе разделяем и получаем разные ячейки
//                    safeDay = daysOfWeek[(int)(col / 5) - 1] + "_" + formattedCellValue;
//                    arrPara.Add($"|{daysOfWeek[(int)(col / 5) - 1] + "_" + formattedCellValue}");
//                }
//                else
//                {
//                    safeDay = daysOfWeek[(int)(col / 5) - 1] + "_" + formattedCellValue;
//                    arrPara.Add($"{"/ " + formattedCellValue}");
//                }

//            }
//        }

//    }
//    if (oneGrupp % 2 == 0 && end == false)
//    {
//        arrPara.Add("|#");
//    }
//}

//resultString = string.Join("", arrPara);

//return resultString;
//                }

//                return "Нет такой группы";
//            }