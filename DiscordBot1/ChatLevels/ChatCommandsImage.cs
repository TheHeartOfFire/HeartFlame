using Discord.Commands;
using Discord.WebSocket;
using HeartFlame.GuildControl;
using HeartFlame.Misc;
using HeartFlame.Permissions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HeartFlame.ChatLevels
{
    public partial class ChatCommands
    {
        [Group("Image"), Alias("img")]
        public class ChatCommandsImage : ModuleBase<SocketCommandContext>
        {
            [Command(""), Alias("h", "?", "help"), Summary("Get all of the commands in the ChatImage Group"), Remarks("ChatCommandImageHelp")]
            public async Task ChatCommandImageHelp()
            {
                var embeds = Configuration.Configuration_Command.HelpEmbed("Chat Image Help", "ChatCommandImageHelp", 1);
                foreach (var embed in embeds)
                {
                    await ReplyAsync("", false, embed);
                }
            }

            [Command("Banner"), Alias("b"), Summary("Set the user's banner image. Input: SocketGuildUser \"Mentioned Discord User\" String \"Banner Image Name\""), Priority(1)]
            [RequirePermission(Roles.CREATOR)]
            public async Task BannerSet(string BannerName, bool FlipHorizontally, bool FlipVertically, SocketGuildUser User)
            {
                var BotGuild = GuildManager.GetGuild(Context.Guild);
                var GUser = BotGuild.GetUser(User);

                GUser.Banner.BannerImage = BannerName;
                GUser.Banner.VerticalFlip = FlipVertically;
                GUser.Banner.HorizontalFlip = FlipHorizontally;
                PersistentData.SaveChangesToJson();

                var img = await BannerMaker.BuildBannerAsync(User, false);
                await BotGuild.GetChatChannel(Context).SendFileAsync(BannerMaker.ToStream(img, System.Drawing.Imaging.ImageFormat.Png), "banner.png");

                if (BotGuild.ModuleControl.IncludeLogging)
                    BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod(),
                        $"{GUser.Name}'s banner has been set to {BannerName}.",
                        Context);
            }


            [Command("Profile"), Alias("avatar"), Summary("Set the user's profile image. Input: SocketGuildUser \"Mentioned Discord User\" String \"Profile Image Name\""), Priority(1)]
            [RequirePermission(Roles.CREATOR)]
            public async Task ProfileSet(string name, SocketGuildUser User)
            {
                var BotGuild = GuildManager.GetGuild(Context.Guild);
                var GUser = BotGuild.GetUser(Context.User);

                GUser.Banner.BannerImage = name;
                PersistentData.SaveChangesToJson();

                var img = await BannerMaker.BuildBannerAsync(User, false);


                await BotGuild.GetChatChannel(Context).SendFileAsync(BannerMaker.ToStream(img, System.Drawing.Imaging.ImageFormat.Png), "banner.png").ConfigureAwait(false);

                if (BotGuild.ModuleControl.IncludeLogging)
                    BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod(),
                        $"{GUser.Name}'s profile has been set to {name}.",
                        Context);
            }

            [Command("Background"), Alias("back"), Summary("Toggle the user's text background. Input: Bool \"Background Active?\""), Priority(1)]
            public async Task BackgroundSet(bool Active = true)
            {
                var BotGuild = GuildManager.GetGuild(Context.Guild);
                var GUser = BotGuild.GetUser(Context.User);

                GUser.Banner.TextBackground = Active;
                PersistentData.SaveChangesToJson();

                var img = await BannerMaker.BuildBannerAsync(Context.User, false);

                await BotGuild.GetChatChannel(Context).SendFileAsync(BannerMaker.ToStream(img, System.Drawing.Imaging.ImageFormat.Png), "banner.png").ConfigureAwait(false);

                string msg = "off";
                if (Active)
                    msg = "on";

                if (BotGuild.ModuleControl.IncludeLogging)
                    BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod(),
                        $"{GUser.Name}'s text background has been turned {msg}.",
                        Context);
            }

            [Command("Background"), Alias("back"), Summary("Toggle the user's text background. Input: Bool \"Background Active?\" SocketGuildUser \"Mentioned Discord User\""), Priority(1)]
            [RequirePermission(Roles.MOD)]
            public async Task BackgroundSet(bool Active, SocketGuildUser User)
            {
                var BotGuild = GuildManager.GetGuild(Context.Guild);
                var GUser = BotGuild.GetUser(User);

                GUser.Banner.TextBackground = Active;
                PersistentData.SaveChangesToJson();


                var img = await BannerMaker.BuildBannerAsync(User, false);
                await BotGuild.GetChatChannel(Context).SendFileAsync(BannerMaker.ToStream(img, System.Drawing.Imaging.ImageFormat.Png), "banner.png").ConfigureAwait(false);

                string msg = "off";
                if (Active)
                    msg = "on";

                if (BotGuild.ModuleControl.IncludeLogging)
                    BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod(),
                        $"{GUser.Name}'s text background has been turned {msg}.",
                        Context);
            }

            [Command("Greyscale"), Alias("grey"), Summary("Set the user's text background greyscale value. Input: int \"Greyscale value 0-255[Default = 227]\""), Priority(1)]
            public async Task grayscaleSet(int Greyscale = 227)
            {
                var BotGuild = GuildManager.GetGuild(Context.Guild);
                var GUser = BotGuild.GetUser(Context.User);

                GUser.Banner.Greyscale = Greyscale;
                PersistentData.SaveChangesToJson();


                var img = await BannerMaker.BuildBannerAsync(Context.User, false);

                await BotGuild.GetChatChannel(Context).SendFileAsync(BannerMaker.ToStream(img, System.Drawing.Imaging.ImageFormat.Png), "banner.png");

                if (BotGuild.ModuleControl.IncludeLogging)
                    BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod(),
                        $"{GUser.Name}'s greyscale value has been set to {Greyscale}.",
                        Context);
            }


            [Command("Greyscale"), Alias("grey"), Summary("Set the mentioned user's text background greyscale value. Input: int \"Greyscale value 0-255\" SocketGuildUser \"Mentioned Discord User\""), Priority(1)]
            [RequirePermission(Roles.MOD)]
            public async Task grayscaleSet(int Greyscale, SocketGuildUser User)
            {
                var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
                var GUser = BotGuild.GetUser(User);

                GUser.Banner.Greyscale = Greyscale;
                PersistentData.SaveChangesToJson();

                var img = await BannerMaker.BuildBannerAsync(User, false);

                await BotGuild.GetChatChannel(Context).SendFileAsync(BannerMaker.ToStream(img, System.Drawing.Imaging.ImageFormat.Png), "banner.png");

                if (BotGuild.ModuleControl.IncludeLogging)
                    BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod(),
                        $"{GUser.Name}'s greyscale value has been set to {Greyscale}.",
                        Context);
            }
        }
    }
}
