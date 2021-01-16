using Discord.WebSocket;
using HeartFlame.GuildControl;
using HeartFlame.Misc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Timers;

namespace HeartFlame.ChatLevels
{
    public class ChatModuleIntegrator
    {
        private static Timer ChatDelay = new Timer();

        public static void OnpreProcessing()
        {
            ChatDelay.Interval = 300000;
            ChatDelay.Elapsed += ChatDelayElapsed;
            ChatDelay.Enabled = true;
        }

        public static async void OnMessagePosted(SocketMessage arg)
        {
            var BotGuild = GuildManager.GetGuild(((SocketGuildChannel)arg.Channel).Guild.Id);
            var user = (SocketGuildUser)arg.Author;
            var GUser = BotGuild.GetUser(user);

            if (!GUser.Chat.ExpPending)
            {
                GUser.Chat.ExpPending = true;
            }

            GUser.Chat.MessagesSent++;
            GUser.UpdateName(user);

            if (!GUser.Chat.LevelPending)
            {
                PersistentData.SaveChangesToJson();
                return;
            }


            GUser.Chat.LevelPending = false;
            GUser.Chat.ChatLevel = LevelManagement.GetLevelAtExp(GUser.Chat.ChatExp);


            await BotGuild.GetChatChannel(arg.Channel).SendFileAsync(
                BannerMaker.ToStream(
                    await BannerMaker.BuildBannerAsync(user),
                    System.Drawing.Imaging.ImageFormat.Png),
                "banner.png",
                $"{user.Mention} Has just advanced to level {GUser.Chat.ChatLevel}");
            
            PersistentData.SaveChangesToJson();
        }

        public static void ChatDelayElapsed(object sender, ElapsedEventArgs e)
        {
            foreach (var Guild in PersistentData.Data.Guilds)
            {
                foreach (var User in Guild.Users)
                {
                    if (User.Chat.ExpPending)
                    {
                        var level = User.Chat.ChatLevel;
                        User.Chat.ExpPending = false;
                        User.Chat.ChatExp += 5;
                        var newLevel = LevelManagement.GetLevelAtExp(User.Chat.ChatExp);
                        if (newLevel > level)
                        {
                            User.Chat.LevelPending = true;
                            User.Chat.ChatLevel = newLevel;
                        }

                    }
                }
            }
            PersistentData.SaveChangesToJson();
        }
    }
}