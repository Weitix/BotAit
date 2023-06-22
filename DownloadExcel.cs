using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace BotAit
{
    internal class DownloadExcel
    {
        public static async Task Download()
        {
            string url = "https://aitanapa.ru/расписание-занятий/";

            string fileName = "расписание";
            string fileExtension = ".xlsx";

            string localFilePath = Path.Combine(Environment.CurrentDirectory, @"расписание.xlsx");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)await request.GetResponseAsync();
            }
            catch (WebException ex)
            {
                response = (HttpWebResponse)ex.Response;
            }

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);

                string htmlCode = await reader.ReadToEndAsync();

                Regex fileRegex = new Regex(@"<a\s+(?:[^>]*?\s+)?href=([""'])(.*?)\1", RegexOptions.IgnoreCase);
                MatchCollection matches = fileRegex.Matches(htmlCode);
                string fileUrl = "";
                List<string> fileUrls = new List<string>();
                string latestFileUrl;
                foreach (Match match in matches)
                {
                    string fileMatch = match.Groups[2].Value;
                    if (fileMatch.Contains(fileName) && Path.GetExtension(fileMatch).Equals(fileExtension))
                    {
                        fileUrls.Add(fileMatch);
                    }
                }
                latestFileUrl = fileUrls[0];

                


                if (!File.Exists(localFilePath) && !string.IsNullOrEmpty(latestFileUrl))
                {
                    WebClient client = new WebClient();
                    await client.DownloadFileTaskAsync(latestFileUrl, localFilePath);
                    Console.WriteLine("Файл успешно загружен впервые.");
                    return;
                }
                else
                {
                    HttpWebRequest requestFile = (HttpWebRequest)WebRequest.Create(latestFileUrl);
                    requestFile.Method = "HEAD";

                    HttpWebResponse responseFile;
                    try
                    {
                        responseFile = (HttpWebResponse)await requestFile.GetResponseAsync();
                    }
                    catch (WebException ex)
                    {
                        responseFile = (HttpWebResponse)ex.Response;
                    }

                    if (responseFile.StatusCode == HttpStatusCode.OK)
                    {

                        FileInfo fileInfo = new FileInfo(localFilePath);

                        long sizeLocal = fileInfo.Length;
                        long sizeRemote = responseFile.ContentLength;

                        if (!string.IsNullOrEmpty(latestFileUrl) && sizeRemote != sizeLocal)
                        {
                            WebClient client = new WebClient();
                            await client.DownloadFileTaskAsync(latestFileUrl, localFilePath);
                            Console.WriteLine("Файл успешно загружен и заменен.");
                        }
                        else
                        {
                            Console.WriteLine("Download");
                            Console.WriteLine("Файл на сайте не изменен.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Не удалось получить файл с сервера.");
                    }
                }
            }
            else
            {
                Console.WriteLine("Не удалось получить файл с сервера.");
            }

        }
    }
}

