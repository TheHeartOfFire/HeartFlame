using Discord.WebSocket;
using HeartFlame.GuildControl;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeartFlame.ChatLevels
{
    public class ChatModuleIntegrator
    {
        public static async void OnMessagePosted(SocketMessage arg)
        {
            var BotGuild = GuildManager.GetGuild(((SocketGuildChannel)arg.Channel).Guild.Id);
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

                if (BotGuild.Configuration.UseChatChannel)
                {
                    var IDs = BotGuild.Configuration.ChatChannel;
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
            foreach (var Guild in GuildManager.Guilds)
            {
                foreach (var User in Guild.Chat)
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

                    }
                }
            }
            GuildManager.SaveChangesToJson();
        }
    }
}