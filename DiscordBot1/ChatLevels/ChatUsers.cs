using Discord.WebSocket;
using HeartFlame.Misc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace HeartFlame.ChatLevels
{
    public class ChatUsers
    {
        private static Timer ChatDelay = new Timer();
        private static readonly string containerString = ModuleControl.BotName;
        private const string blobString = "chat";

        public static List<ChatDataType> Chat_Users;

        public static async void OnpreProcessing()
        {
            await Constructor();

            ChatDelay.Interval = 300000;
            ChatDelay.Elapsed += ChatModuleIntegrator.ChatDelayElapsed;
            ChatDelay.Enabled = true;
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
                Chat_Users = new List<ChatDataType>();
                json = JsonConvert.SerializeObject(Chat_Users, Formatting.Indented);
                blob.UploadTextAsync(json).Wait();
                return -1;
            }
            var data = JsonConvert.DeserializeObject<List<ChatDataType>>(json);
            Chat_Users = data;
            return -1;
        }

        public static void SaveChangesToJson()
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(Configuration.Configuration.storageConnectionString);
            CloudBlobClient serviceClient = account.CreateCloudBlobClient();

            var container = serviceClient.GetContainerReference(containerString);
            container.CreateIfNotExistsAsync();
            CloudBlockBlob blob = container.GetBlockBlobReference(blobString);

            string json = JsonConvert.SerializeObject(Chat_Users, Formatting.Indented);
            blob.UploadTextAsync(json).Wait();
        }

        public static ChatDataType RetrieveOrCreateChatUser(SocketGuildUser User)
        {
            foreach (var user in Chat_Users)
            {
                if (user.DiscordID == User.Id)
                    return user;
            }

            var name = User.Nickname;
            if (name is null)
                name = User.Username;

            var NewUser = new ChatDataType()
            {
                DiscordID = User.Id,
                ChatExp = 0,
                ChatLevel = 1,
                MessagesSent = 0,
                ColorARGB = new int[] { 255, 255, 0, 0 },//initialize Red
                DiscordUsername = name,
                BannerImage = "default",
                ProfileImage = "default"
            };
            Chat_Users.Add(NewUser);
            SaveChangesToJson();
            return NewUser;
        }

        public static ChatDataType UpdateUser(SocketGuildUser User, int AddMessages = 0, int AddExp = 0)
        {
            RetrieveOrCreateChatUser(User);
            foreach (var user in Chat_Users)
            {
                if (user.DiscordID == User.Id)
                {
                    user.ChatExp += AddExp;
                    user.MessagesSent += AddMessages;
                    user.ChatLevel = LevelManagement.GetLevelAtExp(user.ChatExp);
                    SaveChangesToJson();
                    return user;
                }
            }
            return null;
        }

        public static Color GetUserColor(SocketGuildUser User)
        {
            foreach (var user in Chat_Users)
            {
                if (user.DiscordID == User.Id)
                    return ParseColor(user.ColorARGB);
            }
            return Color.Red;
        }

        private static Color ParseColor(int[] input)
        {
            return Color.FromArgb(input[0], input[1], input[2], input[3]);
        }

        public static void SetUserColor(SocketGuildUser User, Color color)
        {
            foreach (var user in Chat_Users)
            {
                if (user.DiscordID == User.Id)
                {
                    user.ColorARGB = new int[] { (int)color.A, (int)color.R, (int)color.G, (int)color.B };
                    SaveChangesToJson();
                }
            }
        }

        public static void ToggleExpPending(SocketGuildUser User, bool pending = true)
        {
            foreach (var user in Chat_Users)
            {
                if (user.DiscordID == User.Id)
                {
                    user.ExpPending = pending;
                    SaveChangesToJson();
                }
            }
        }

        public static void ToggleLevelPending(SocketGuildUser User, bool pending = true)
        {
            foreach (var user in Chat_Users)
            {
                if (user.DiscordID == User.Id)
                {
                    user.LevelPending = pending;
                    SaveChangesToJson();
                }
            }
        }

        public static void SetLevel(SocketGuildUser User, int Level)
        {
            foreach (var user in Chat_Users)
            {
                if (user.DiscordID == User.Id)
                {
                    user.ChatLevel = Level;
                    SaveChangesToJson();
                }
            }
        }

        public static void UpdateUsername(SocketGuildUser User)
        {
            foreach (var user in Chat_Users)
            {
                if (user.DiscordID == User.Id)
                {
                    user.DiscordUsername = User.Nickname;
                    if (User.Nickname is null)
                        user.DiscordUsername = User.Username;
                    SaveChangesToJson();
                }
            }
        }

        public static void SetBanner(SocketGuildUser User, string name)
        {
            foreach (var user in Chat_Users)
            {
                if (user.DiscordID == User.Id)
                {
                    user.BannerImage = name;
                    SaveChangesToJson();
                }
            }
        }

        public static void Setprofile(SocketGuildUser User, string name)
        {
            foreach (var user in Chat_Users)
            {
                if (user.DiscordID == User.Id)
                {
                    user.ProfileImage = name;
                    SaveChangesToJson();
                }
            }
        }

        public static void SetBackground(SocketGuildUser User, bool active)
        {
            foreach (var user in Chat_Users)
            {
                if (user.DiscordID == User.Id)
                {
                    user.TextBackground = active;
                    SaveChangesToJson();
                }
            }
        }

        public static void SetGreyscale(SocketGuildUser User, int Greyscale)
        {
            foreach (var user in Chat_Users)
            {
                if (user.DiscordID == User.Id)
                {
                    user.Greyscale = Greyscale;
                    SaveChangesToJson();
                }
            }
        }
    }
}