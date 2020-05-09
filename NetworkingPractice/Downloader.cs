using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkingPractice
{
    class Downloader
    {

        // Where to download from, and where to save it to
        public static string urlToDownload = "Https://16bpp.net/";
        public static string filename = "index.html";

        public static async Task DownloadWebPage()
        {
            Console.WriteLine("Starting download...");

            // setup the HTTPClient
            using(HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage resp = await httpClient.GetAsync(urlToDownload);
                

                if (resp.IsSuccessStatusCode)
                {
                    Console.WriteLine("Got it...");

                    // Get the data
                    byte[] data = await resp.Content.ReadAsByteArrayAsync();

                    // Save it to a file    
                    FileStream fStream = File.Create(filename);
                    await fStream.WriteAsync(data, 0, data.Length);
                    fStream.Close();

                    Console.WriteLine("Done!");
                }
            }
        }

        static public void Run()
        {
            Task dlTask = DownloadWebPage();

            Console.WriteLine("Holding for at least 5 seconds...");
            Thread.Sleep(TimeSpan.FromSeconds(5));

            dlTask.GetAwaiter().GetResult();
        }
    }
}
