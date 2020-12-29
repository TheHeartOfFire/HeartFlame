using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HeartFlame.Misc
{
    public class PersistentData
    {
        public static readonly string BotName = "heartflame";
        private const string blobString = "botdata";

        public static readonly string storageConnectionString = Properties.Resources.StorageToken;

        public static PersistentDataType Data;
        public static async Task<int> Constructor()
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(storageConnectionString);
            CloudBlobClient serviceClient = account.CreateCloudBlobClient();
            var container = serviceClient.GetContainerReference(BotName);
            await container.CreateIfNotExistsAsync();
            CloudBlockBlob blob = container.GetBlockBlobReference(blobString);
            string json = "";
            try
            {
                json = await blob.DownloadTextAsync();
            }
            catch (StorageException)
            {
                Data = new PersistentDataType();
                json = JsonConvert.SerializeObject(Data, Formatting.Indented);
                blob.UploadTextAsync(json).Wait();
                return -1;
            }
            var data = JsonConvert.DeserializeObject<PersistentDataType>(json);
            Data = data;
            return -1;
        }

        public static void SaveChangesToJson()
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(storageConnectionString);
            CloudBlobClient serviceClient = account.CreateCloudBlobClient();

            var container = serviceClient.GetContainerReference(BotName);
            container.CreateIfNotExistsAsync();
            CloudBlockBlob blob = container.GetBlockBlobReference(blobString);

            string json = JsonConvert.SerializeObject(Data, Formatting.Indented);
            blob.UploadTextAsync(json).Wait();
        }
    }
}
