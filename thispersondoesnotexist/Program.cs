using System;
using System.IO;
using System.Net;

namespace thispersondoesnotexist
{
    internal class Program
    {
        private const string SiteUrl = "https://thispersondoesnotexist.com/";

        private Stream _strResponse;
        private Stream _strLocal;
        private HttpWebRequest _webRequest;
        private HttpWebResponse _webResponse;
        private static int _percentProgress;
        private static readonly string _saveDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\ThisPersonDoesNotExist";

        private delegate void UpdatePProgressCallback(long bytesRead, long totalBytes);

        private static void Main(string[] args)
        {
            Console.WriteLine("I will save an image from thispersondoesnotexist.com each time you say to do so.");
            Console.WriteLine("They will be saved as jpg images in " + _saveDir + " directory in numerical order.");
            Console.WriteLine("");

            Console.WriteLine("Press F5 to get an image, any key to exit");

            while (Console.ReadKey().Key == ConsoleKey.F5)
            {
                Console.WriteLine("");
                Console.WriteLine("Download Starting");

                new Program().Download();

                Console.WriteLine("");
                Console.WriteLine("Press F5 to get an image, any key to exit");
            }
        }

        private void Download()
        {
            var count = 0;
            string downloadFileName;
            byte[] downBuffer = new byte[2048];

            if (!Directory.Exists(_saveDir))
            {
                Directory.CreateDirectory(_saveDir);
            }

            do
            {
                count += 1;
                downloadFileName = string.Format("{0}\\thispersondoesnotexist_{1}.jpg", _saveDir, count);
            } while (File.Exists(downloadFileName));

            Console.Write("\rDownloaded 0%");

            using (WebClient wcDownload = new WebClient())
            {
                try
                {
                    _webRequest = (HttpWebRequest) WebRequest.Create(SiteUrl);
                    _webResponse = (HttpWebResponse) _webRequest.GetResponse();

                    var fileSize = _webResponse.ContentLength;

                    _strResponse = wcDownload.OpenRead(SiteUrl);

                    _strLocal = new FileStream(downloadFileName, FileMode.Create, FileAccess.Write, FileShare.None);

                    var bytesSize = 0;
                    while (_strResponse != null && (bytesSize = _strResponse.Read(downBuffer, 0, downBuffer.Length)) > 0)
                    {
                        _strLocal.Write(downBuffer, 0, bytesSize);

                        _percentProgress = Convert.ToInt32((_strLocal.Length * 100) / fileSize);

                        Console.Write("\rDownloaded {0}%", _percentProgress);
                    }

                    Console.Write("\rDownloaded 100%");
                    Console.WriteLine("\nFile " + downloadFileName + " downloaded");
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0} could not get a response", SiteUrl);
                    Console.WriteLine(e.ToString());
                }
                finally
                {
                    _strResponse?.Close();
                    _strLocal?.Close();
                }
            }
        }
    }
}