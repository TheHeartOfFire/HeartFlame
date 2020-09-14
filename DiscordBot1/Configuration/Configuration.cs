using HeartFlame.Misc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace HeartFlame.Configuration
{
    public class Configuration
    {
        private static readonly string containerString = ModuleControl.BotName;
        private const string blobString = "config";

        public const string storageConnectionString = "" +
            "DefaultEndpointsProtocol=https;" +
            "AccountName=aimbotdata;" +
            "AccountKey=didJFE0mtpW1vDGWbEe+So8rp9Om2NXbdG4lRp5oBzo7aORz8Rb2tjhbljHiof37QJUlutyEHT8AwcHxXtXEiA==;" +
            "EndpointSuffix=core.windows.net";

        public static Configuration_DataType bot;

        public static async Task<int> Constructor()
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(storageConnectionString);
            CloudBlobClient serviceClient = account.CreateCloudBlobClient();
            var container = serviceClient.GetContainerReference(containerString);
            await container.CreateIfNotExistsAsync();
            CloudBlockBlob blob = container.GetBlockBlobReference(blobString);
            string json = "";
            try
            {
                json = await blob.DownloadTextAsync();
            }
            catch (StorageException)
            {
                bot = new Configuration_DataType();
                json = JsonConvert.SerializeObject(bot, Formatting.Indented);
                blob.UploadTextAsync(json).Wait();
                return -1;
            }
            var data = JsonConvert.DeserializeObject<Configuration_DataType>(json);
            bot = data;
            return -1;
        }

        public static void SaveChangesToJson()
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(storageConnectionString);
            CloudBlobClient serviceClient = account.CreateCloudBlobClient();

            var container = serviceClient.GetContainerReference(containerString);
            container.CreateIfNotExistsAsync();
            CloudBlockBlob blob = container.GetBlockBlobReference(blobString);

            string json = JsonConvert.SerializeObject(bot, Formatting.Indented);
            blob.UploadTextAsync(json).Wait();
        }
    }
}