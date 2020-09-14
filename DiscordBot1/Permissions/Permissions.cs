using Discord.WebSocket;
using HeartFlame.Misc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HeartFlame.Permissions
{
    public class Permissions
    {
        private static readonly string containerString = ModuleControl.BotName;
        private const string blobString = "permissions";

        public static PermissionsDataType Perm_Users;

        public static async void OnpreProcessing()
        {
            await Constructor();
        }

        private static async Task<int> Constructor()
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(Configuration.Configuration.storageConnectionString);
            CloudBlobClient serviceClient = account.CreateCloudBlobClient();
            var container = serviceClient.GetContainerReference(containerString);
            await container.CreateIfNotExistsAsync().ConfigureAwait(false);
            CloudBlockBlob blob = container.GetBlockBlobReference(blobString);
            string json;
            try
            {
                json = await blob.DownloadTextAsync().ConfigureAwait(true);
            }
            catch (StorageException)
            {
                Perm_Users = new PermissionsDataType();
                json = JsonConvert.SerializeObject(Perm_Users, Formatting.Indented);
                blob.UploadTextAsync(json).Wait();
                return -1;
            }
            var data = JsonConvert.DeserializeObject<PermissionsDataType>(json);
            Perm_Users = data;
            return -1;
        }

        public static void SaveChangesToJson()
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(Configuration.Configuration.storageConnectionString);
            CloudBlobClient serviceClient = account.CreateCloudBlobClient();

            var container = serviceClient.GetContainerReference(containerString);
            container.CreateIfNotExistsAsync();
            CloudBlockBlob blob = container.GetBlockBlobReference(blobString);

            string json = JsonConvert.SerializeObject(Perm_Users, Formatting.Indented);
            blob.UploadTextAsync(json).Wait();
        }

        public static void AddMod(SocketGuildUser User)
        {
            Perm_Users.Mods.Add(new Permissions_User
            {
                Name = User.Username,
                ID = User.Id
            });
            SaveChangesToJson();
        }

        public static void AddMod(List<SocketGuildUser> Users)
        {
            foreach (var User in Users)
            {
                Perm_Users.Mods.Add(new Permissions_User
                {
                    Name = User.Username,
                    ID = User.Id
                });
            }
            SaveChangesToJson();
        }

        public static void AddAdmin(SocketGuildUser User)
        {
            Perm_Users.Admins.Add(new Permissions_User
            {
                Name = User.Username,
                ID = User.Id
            });
            SaveChangesToJson();
        }

        public static void AddAdmin(List<SocketGuildUser> Users)
        {
            foreach (var User in Users)
            {
                Perm_Users.Admins.Add(new Permissions_User
                {
                    Name = User.Username,
                    ID = User.Id
                });
            }
            SaveChangesToJson();
        }

        public static void RemoveMod(SocketGuildUser User)
        {
            if (IsMod(User))
            {
                foreach (var user in Perm_Users.Mods)
                {
                    if (User.Id == user.ID)
                        Perm_Users.Mods.Remove(user);
                }
            }
            SaveChangesToJson();
        }

        public static void RemoveMod(List<SocketGuildUser> Users)
        {
            foreach (var User in Users)
            {
                if (IsMod(User))
                {
                    foreach (var user in Perm_Users.Mods)
                    {
                        if (User.Id == user.ID)
                            Perm_Users.Mods.Remove(user);
                    }
                }
            }
            SaveChangesToJson();
        }

        public static void RemoveAdmin(SocketGuildUser User)
        {
            if (IsMod(User))
            {
                foreach (var user in Perm_Users.Admins)
                {
                    if (User.Id == user.ID)
                        Perm_Users.Admins.Remove(user);
                }
            }
            SaveChangesToJson();
        }

        public static void RemoveAdmin(List<SocketGuildUser> Users)
        {
            foreach (var User in Users)
            {
                if (IsMod(User))
                {
                    foreach (var user in Perm_Users.Admins)
                    {
                        if (User.Id == user.ID)
                            Perm_Users.Admins.Remove(user);
                    }
                }
            }
            SaveChangesToJson();
        }

        public static bool IsMod(SocketGuildUser User)
        {
            if (User is null) return false;

            foreach (var user in Perm_Users.Mods)
            {
                if (user.ID == User.Id)
                    return true;
            }
            foreach (var user in Perm_Users.Admins)
            {
                if (user.ID == User.Id)
                    return true;
            }

            return false;
        }

        public static bool IsAdmin(SocketGuildUser User)
        {
            if (User is null) return false;

            foreach (var user in Perm_Users.Admins)
            {
                if (user.ID == User.Id)
                    return true;
            }

            return false;
        }
    }
}