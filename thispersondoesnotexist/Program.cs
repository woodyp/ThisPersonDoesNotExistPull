using System;
using System.IO;
using System.Net;
using System.Threading;

namespace thispersondoesnotexist
{
    internal class Program
    {
        // The thread inside which the download happens
        private Thread thrDownload;

        // The stream of data retrieved from the web server
        private Stream strResponse;

        // The stream of data that we write to the harddrive
        private Stream strLocal;

        // The request to the web server for file information
        private HttpWebRequest webRequest;

        // The response from the web server containing information about the file
        private HttpWebResponse webResponse;

        // The progress of the download in percentage
        private static int PercentProgress;

        // The delegate which we will call from the thread to update the form
        private delegate void UpdateProgessCallback(Int64 BytesRead, Int64 TotalBytes);

        private static string saveDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\ThisPersonDoesNotExist";

        private static void Main(string[] args)
        {
            Console.WriteLine("I will save an image from thispersondoesnotexist.com each time you say to do so.");
            Console.WriteLine("They will be saved as jpg images in " + saveDir + " directory in numerical order.");

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

            if (!Directory.Exists(saveDir))
            {
                Directory.CreateDirectory(saveDir);
            }

            do
            {
                count += 1;
                downloadFileName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\thispersondoesnotexist\\thispersondoesnotexist_" + count.ToString() + ".jpg";
            } while (File.Exists(downloadFileName));

            using (WebClient wcDownload = new WebClient())
            {
                try
                {
                    // Create a request to the file we are downloading
                    webRequest = (HttpWebRequest)WebRequest.Create("https://thispersondoesnotexist.com/");

                    // Set default authentication for retrieving the file
                    webRequest.Credentials = CredentialCache.DefaultCredentials;

                    // Retrieve the response from the server
                    webResponse = (HttpWebResponse)webRequest.GetResponse();

                    // Ask the server for the file size and store it
                    Int64 fileSize = webResponse.ContentLength;

                    // Open the URL for download
                    strResponse = wcDownload.OpenRead("https://thispersondoesnotexist.com/");

                    // Create a new file stream where we will be saving the data (local drive)
                    strLocal = new FileStream(downloadFileName, FileMode.Create, FileAccess.Write, FileShare.None);

                    // It will store the current number of bytes we retrieved from the server
                    int bytesSize = 0;

                    // A buffer for storing and writing the data retrieved from the server
                    byte[] downBuffer = new byte[2048];

                    // Loop through the buffer until the buffer is empty
                    while ((bytesSize = strResponse.Read(downBuffer, 0, downBuffer.Length)) > 0)
                    {
                        // Write the data from the buffer to the local hard drive
                        strLocal.Write(downBuffer, 0, bytesSize);


                        // Calculate the download progress in percentages
                        PercentProgress = Convert.ToInt32((strLocal.Length * 100) / fileSize);
                        // Make progress on the progress bar

                        // Display the current progress on the form
                        Console.Write("-");
                        //lblProgress.Text = "Downloaded " + (strLocal.Length * 100) + " out of " + fileSize + " (" + PercentProgress + "%)";
                    }


                }
                finally
                {
                    // When the above code has ended, close the streams
                    strResponse.Close();
                    strLocal.Close();
                }
            }
        }
    }
}