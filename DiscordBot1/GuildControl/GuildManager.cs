using HeartFlame.Misc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HeartFlame.GuildControl
{
    public class GuildManager
    {
        private const string blobString = "permissions";

        public static List<GuildData> Guilds;

        public static async void OnpreProcessing()
        {
            await Constructor();
        }

        private static async Task<int> Constructor()
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(Configuration.Configuration.storageConnectionString);
            CloudBlobClient serviceClient = account.CreateCloudBlobClient();
            var container = serviceClient.GetContainerReference(ModuleControl.BotName);
            await container.CreateIfNotExistsAsync().ConfigureAwait(false);
            CloudBlockBlob blob = container.GetBlockBlobReference(blobString);
            string json;
            try
            {
                json = await blob.DownloadTextAsync().ConfigureAwait(true);
            }
            catch (StorageException)
            {
                Guilds = new List<GuildData>();
                json = JsonConvert.SerializeObject(Guilds, Formatting.Indented);
                blob.UploadTextAsync(json).Wait();
                return -1;
            }
            var data = JsonConvert.DeserializeObject<List<GuildData>>(json);
            Guilds = data;
            return -1;
        }

        public static void SaveChangesToJson()
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(Configuration.Configuration.storageConnectionString);
            CloudBlobClient serviceClient = account.CreateCloudBlobClient();

            var container = serviceClient.GetContainerReference(ModuleControl.BotName);
            container.CreateIfNotExistsAsync();
            CloudBlockBlob blob = container.GetBlockBlobReference(blobString);

            string json = JsonConvert.SerializeObject(Guilds, Formatting.Indented);
            blob.UploadTextAsync(json).Wait();
        }

        public static void AddGuild(ulong GuildID)
        {
            foreach(var Guild in Guilds)
            {
                if(Guild.GuildID == GuildID)
                    return;
            }

            Guilds.Add(new GuildData(GuildID));
            SaveChangesToJson();
        }

        public static void RemoveGuild(ulong GuildID)
        {
            foreach(var Guild in Guilds)
            {
                if (Guild.GuildID == GuildID)
                    Guilds.Remove(Guild);
            }
            SaveChangesToJson();
        }

        public static GuildData GetGuild(ulong GuildID)
        {
            foreach(var Guild in Guilds)
            {
                if (Guild.GuildID == GuildID)
                    return Guild;
            }
            return null;
        }
    }
}
