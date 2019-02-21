using System;
using System.IO;
using System.Net;
using System.Threading;

namespace thispersondoesnotexist
{
    internal class Program
    {
        private Stream _strResponse;
        private Stream _strLocal;
        private HttpWebRequest _webRequest;
        private HttpWebResponse _webResponse;
        private static int _percentProgress;
        private delegate void UpdatePProgressCallback(long bytesRead, long totalBytes);
        private static string _saveDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\ThisPersonDoesNotExist";

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
            var downloadFileName = string.Empty;
            int bytesSize = 0;
            byte[] downBuffer = new byte[2048];

            if (!Directory.Exists(_saveDir))
            {
                Directory.CreateDirectory(_saveDir);
            }

            do
            {
                count += 1;
                downloadFileName = _saveDir + "\\thispersondoesnotexist_" + count.ToString() + ".jpg";
            } while (File.Exists(downloadFileName));

            Console.Write("\rDownloaded 0%   ");

            using (WebClient wcDownload = new WebClient())
            {
                try
                {
                    _webRequest = (HttpWebRequest)WebRequest.Create("https://thispersondoesnotexist.com/");
                    _webRequest.Credentials = CredentialCache.DefaultCredentials;
                    _webResponse = (HttpWebResponse)_webRequest.GetResponse();
                    var fileSize = _webResponse.ContentLength;
                    _strResponse = wcDownload.OpenRead("https://thispersondoesnotexist.com/");
                    _strLocal = new FileStream(downloadFileName, FileMode.Create, FileAccess.Write, FileShare.None);

                    while ((bytesSize = _strResponse.Read(downBuffer, 0, downBuffer.Length)) > 0)
                    {
                        _strLocal.Write(downBuffer, 0, bytesSize);

                        _percentProgress = Convert.ToInt32((_strLocal.Length * 100) / fileSize);

                        Console.Write("\rDownloaded {0}%   ", _percentProgress);
                    }
                    Console.Write("\rDownloaded 100%   ");
                    Console.WriteLine("\nFile " + downloadFileName + " downloaded");
                }
                finally
                {
                    _strResponse.Close();
                    _strLocal.Close();
                }
            }
        }
    }
}