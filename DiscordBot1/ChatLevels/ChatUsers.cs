using Discord.WebSocket;
using HeartFlame.GuildControl;
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

        public static async void OnpreProcessing()
        {
            ChatDelay.Interval = 300000;
            ChatDelay.Elapsed += ChatModuleIntegrator.ChatDelayElapsed;
            ChatDelay.Enabled = true;
        }
        /// <summary>
        /// Returns null if guild not found
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns>
        public static ChatDataType RetrieveOrCreateChatUser(SocketGuildUser User)
        {
            foreach(var Guild in GuildManager.Guilds)
            {
                if (User.Guild.Id == Guild.GuildID)
                {
                    foreach (var user in Guild.Chat)
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
                    Guild.Chat.Add(NewUser);
                    GuildManager.SaveChangesToJson();
                    return NewUser;
                }
            }
            return null;
            
        }

        public static ChatDataType UpdateUser(SocketGuildUser User, int AddMessages = 0, int AddExp = 0)
        {
            RetrieveOrCreateChatUser(User);

            foreach (var Guild in GuildManager.Guilds)
            {
                if (User.Guild.Id == Guild.GuildID)
                {
                    foreach (var user in Guild.Chat)
                    {
                        if (user.DiscordID == User.Id)
                        {
                            user.ChatExp += AddExp;
                            user.MessagesSent += AddMessages;
                            user.ChatLevel = LevelManagement.GetLevelAtExp(user.ChatExp);
                            GuildManager.SaveChangesToJson();
                            return user;
                        }
                    }
                }
            }
            return null;
        }

        public static Color GetUserColor(SocketGuildUser User)
        {
            foreach (var Guild in GuildManager.Guilds)
            {
                if (User.Guild.Id == Guild.GuildID)
                {
                    foreach (var user in Guild.Chat)
                    {
                        if (user.DiscordID == User.Id)
                            return ParseColor(user.ColorARGB);
                    }
                }
            }
                    return Color.Red;
        }

        private static Color ParseColor(int[] input)
        {
            return Color.FromArgb(input[0], input[1], input[2], input[3]);
        }

        public static void SetUserColor(SocketGuildUser User, Color color)
        {
            foreach (var Guild in GuildManager.Guilds)
            {
                if (User.Guild.Id == Guild.GuildID)
                {
                    foreach (var user in Guild.Chat)
                    {
                        if (user.DiscordID == User.Id)
                        {
                            user.ColorARGB = new int[] { (int)color.A, (int)color.R, (int)color.G, (int)color.B };
                            GuildManager.SaveChangesToJson();
                        }
                    }
                }
            }
        }

        public static void ToggleExpPending(SocketGuildUser User, bool pending = true)
        {
            foreach (var Guild in GuildManager.Guilds)
            {
                if (User.Guild.Id == Guild.GuildID)
                {
                    foreach (var user in Guild.Chat)
                    {
                        if (user.DiscordID == User.Id)
                        {
                            user.ExpPending = pending;
                            GuildManager.SaveChangesToJson();
                        }
                    }
                }
            }
        }

        public static void ToggleLevelPending(SocketGuildUser User, bool pending = true)
        {
            foreach (var Guild in GuildManager.Guilds)
            {
                if (User.Guild.Id == Guild.GuildID)
                {
                    foreach (var user in Guild.Chat)
                    {
                        if (user.DiscordID == User.Id)
                        {
                            user.LevelPending = pending;
                            GuildManager.SaveChangesToJson();
                        }
                    }
                }
            }
        }

        public static void SetLevel(SocketGuildUser User, int Level)
        {
            foreach (var Guild in GuildManager.Guilds)
            {
                if (User.Guild.Id == Guild.GuildID)
                {
                    foreach (var user in Guild.Chat)
                    {
                        if (user.DiscordID == User.Id)
                        {
                            user.ChatLevel = Level;
                            GuildManager.SaveChangesToJson();
                        }
                    }
                }
            }
        }

        public static void UpdateUsername(SocketGuildUser User)
        {
            foreach (var Guild in GuildManager.Guilds)
            {
                if (User.Guild.Id == Guild.GuildID)
                {
                    foreach (var user in Guild.Chat)
                    {
                        if (user.DiscordID == User.Id)
                        {
                            user.DiscordUsername = User.Nickname;
                            if (User.Nickname is null)
                                user.DiscordUsername = User.Username;
                            GuildManager.SaveChangesToJson();
                        }
                    }
                }
            }
        }

        public static void SetBanner(SocketGuildUser User, string name)
        {
            foreach (var Guild in GuildManager.Guilds)
            {
                if (User.Guild.Id == Guild.GuildID)
                {
                    foreach (var user in Guild.Chat)
                    {
                        if (user.DiscordID == User.Id)
                        {
                            user.BannerImage = name;
                            GuildManager.SaveChangesToJson();
                        }
                    }
                }
            }
        }

        public static void Setprofile(SocketGuildUser User, string name)
        {
            foreach (var Guild in GuildManager.Guilds)
            {
                if (User.Guild.Id == Guild.GuildID)
                {
                    foreach (var user in Guild.Chat)
                    {
                        if (user.DiscordID == User.Id)
                        {
                            user.ProfileImage = name;
                            GuildManager.SaveChangesToJson();
                        }
                    }
                }
            }
        }

        public static void SetBackground(SocketGuildUser User, bool active)
        {
            foreach (var Guild in GuildManager.Guilds)
            {
                if (User.Guild.Id == Guild.GuildID)
                {
                    foreach (var user in Guild.Chat)
                    {
                        if (user.DiscordID == User.Id)
                        {
                            user.TextBackground = active;
                            GuildManager.SaveChangesToJson();
                        }
                    }
                }
            }
        }

        public static void SetGreyscale(SocketGuildUser User, int Greyscale)
        {
            foreach (var Guild in GuildManager.Guilds)
            {
                if (User.Guild.Id == Guild.GuildID)
                {
                    foreach (var user in Guild.Chat)
                    {
                        if (user.DiscordID == User.Id)
                        {
                            user.Greyscale = Greyscale;
                            GuildManager.SaveChangesToJson();
                        }
                    }
                }
            }
        }
    }
}