using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DemoWebJob3
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();

        static void Main(string[] args)
        {
            while (1 == 1)
            {
                ProcessJob().Wait();
                Console.WriteLine("temperature saved");
                System.Threading.Thread.Sleep(60000);
            }
        }

        private static async Task ProcessJob()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("User-Agent", ".NET WebJob");
            String apiKey = ConfigurationManager.AppSettings["OpenWeatherApiKey"];
            var stringTask = client.GetStringAsync("https://api.openweathermap.org/data/2.5/weather?q=Kuopio,Finland&APPID=" + apiKey);

            var msg = await stringTask;

            SaveTemperatureBlob(msg);
        }

        private static void SaveTemperatureBlob(string msgTemp)
        {
            String strorageconn = ConfigurationManager.ConnectionStrings["AzureBlobStorage"].ConnectionString;
            CloudStorageAccount storageacc = CloudStorageAccount.Parse(strorageconn);

            CloudBlobClient blobClient = storageacc.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("temperatures");

            string strNow = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            CloudBlockBlob blockBlob = container.GetBlockBlobReference("temp3_" + strNow);
            blockBlob.UploadText(msgTemp);
        }
    }
}
