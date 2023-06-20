using System.Text.RegularExpressions;
using Newtonsoft.Json;
using OfficeOpenXml;

class makeJson
{
    public static async Task ParseExcelToJson()
    {
        
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        string filePath = Path.Combine(Environment.CurrentDirectory, @"расписание.xlsx");
        FileInfo file = new FileInfo(filePath);

        using (ExcelPackage package = new ExcelPackage(file))
        {
            
            ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

            
            List<string> columnValues = new List<string>();

            for (int row = 1; row <= worksheet.Dimension.End.Row; row++)
            {
                string cellValue = Convert.ToString(worksheet.Cells[row, 1].Value);
                if (!string.IsNullOrWhiteSpace(cellValue) && !cellValue.Contains("."))
                {
                    Regex regex = new Regex(@"\d+-\d+");
                    if (regex.IsMatch(cellValue))
                    {
                        columnValues.Add(cellValue);
                    }
                }
            }
            using (StreamWriter fileStream = File.CreateText(Path.Combine(Environment.CurrentDirectory, @"output.json")))
            {
                JsonSerializer serializer = new JsonSerializer
                {
                    Formatting = Formatting.Indented
                };
                foreach (var value in columnValues)
                {
                    serializer.Serialize(fileStream, value);
                    fileStream.WriteLine();
                }
                Console.WriteLine("save json");
            }

        }
    }
}
