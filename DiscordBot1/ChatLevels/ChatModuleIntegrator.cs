using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot1.ChatLevels
{
    public class ChatModuleIntegrator
    {
        public static async void OnMessagePosted(SocketMessage arg)
        {
            var user = (SocketGuildUser)arg.Author;
            if (!ChatUsers.RetrieveOrCreateChatUser(user).ExpPending)
            {
                ChatUsers.ToggleExpPending(user);
            }
            ChatUsers.UpdateUser(user, 1);
            ChatUsers.UpdateUsername(user);
            if (ChatUsers.RetrieveOrCreateChatUser(user).LevelPending)
            {
                ChatUsers.ToggleLevelPending(user, false);
                ChatUsers.SetLevel(user, LevelManagement.GetLevelAtExp(ChatUsers.RetrieveOrCreateChatUser(user).ChatExp));

                if (Configuration.Configuration.bot.UseChatChannel)
                {
                    var IDs = Configuration.Configuration.bot.ChatChannel;
                    foreach (var id in IDs)
                    {
                        await (Program.Client.GetChannel(id) as ISocketMessageChannel).SendFileAsync(BannerMaker.ToStream(await BannerMaker.BuildBannerAsync(user, false), System.Drawing.Imaging.ImageFormat.Png), "banner.png", $"{user.Mention} Has just advanced to level {ChatUsers.RetrieveOrCreateChatUser(user).ChatLevel}");
                    }
                }
                else
                    await arg.Channel.SendFileAsync(BannerMaker.ToStream(await BannerMaker.BuildBannerAsync(user, false), System.Drawing.Imaging.ImageFormat.Png), "banner.png", $"{user.Mention} Has just advanced to level {ChatUsers.RetrieveOrCreateChatUser(user).ChatLevel}").ConfigureAwait(false);
            }
        }

        public static void ChatDelayElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            foreach (var User in ChatUsers.Chat_Users)
            {
                if (User.ExpPending)
                {
                    var level = User.ChatLevel;
                    User.ExpPending = false;
                    User.ChatExp += 5;
                    var newLevel = LevelManagement.GetLevelAtExp(User.ChatExp);
                    if (newLevel > level)
                    {
                        User.LevelPending = true;
                        User.ChatLevel = newLevel;
                    }

                    ChatUsers.SaveChangesToJson();
                }
            }
        }
    }
}