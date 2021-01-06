using Discord;
using Discord.Commands;
using Discord.WebSocket;
using HeartFlame.GuildControl;
using HeartFlame.Misc;
using HeartFlame.ModuleControl;
using HeartFlame.Permissions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HeartFlame.ChatLevels
{
    public partial class ChatCommands
    {
        [Group("Color")]
        public class ChatCommandsColor : ModuleBase<SocketCommandContext>
        {
            [Command(""), Alias("h", "?", "help"), Summary("Get all of the commands in the Chat Color Group"), Remarks("ChatCommandColorHelp")]
            public async Task ChatCommandColorHelp()
            {
                var embeds = Configuration.Configuration_Command.HelpEmbed("Chat Color Help", "ChatCommandColorHelp", 1);
                foreach (var embed in embeds)
                {
                    await ReplyAsync("", false, embed);
                }
            }

            [Command("hex"), Alias("h"), Summary("Set the user's chat text color. Optionally mention a user to set their Chat Color. Input:String \"Color Hex Code i.e. FF00FF\""), Priority(1)]
            public async Task SetColor(string hex)
            {
                var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
                var GUser = BotGuild.GetUser(Context.User);

                if (BadColor(hex))
                {
                    await Context.Channel.SendMessageAsync(Properties.Resources.BadHex);
                    return;
                }


                GUser.Banner.SetColor(ColorTranslator.FromHtml("#" + hex));
                PersistentData.SaveChangesToJson();

                await BotGuild.GetChatChannel(Context).SendMessageAsync($"Color set to #{hex}").ConfigureAwait(false);


                var img = await BannerMaker.BuildBannerAsync(Context.User, false);

                await BotGuild.GetChatChannel(Context).SendFileAsync(BannerMaker.ToStream(img, System.Drawing.Imaging.ImageFormat.Png), "banner.png");

                if (BotGuild.ModuleControl.IncludeLogging)
                    BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod(),
                        $"{GUser.Name}'s chat color has been set to {hex}.",
                        Context);
            }

            [Command("hex"), Alias("h"), Summary("Set the user's chat text color. Optionally mention a user to set their Chat Color. Input:String \"Color Hex Code i.e. FF00FF\" SocketGuildUser \"Mentioned Discord User\""), Priority(1)]
            [RequirePermission(Roles.MOD)]
            public async Task SetColor(string hex, SocketGuildUser User)
            {
                var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
                var GUser = BotGuild.GetUser(User);
                var DisUser = User;

                if (BadColor(hex))
                {
                    await Context.Channel.SendMessageAsync(Properties.Resources.BadHex);
                    return;
                }

                GUser.Banner.SetColor(ColorTranslator.FromHtml("#" + hex));
                PersistentData.SaveChangesToJson();

                await BotGuild.GetChatChannel(Context).SendMessageAsync($"Color set to #{hex}").ConfigureAwait(false);

                var img = await BannerMaker.BuildBannerAsync(DisUser, false);

                await BotGuild.GetChatChannel(Context).SendFileAsync(BannerMaker.ToStream(img, System.Drawing.Imaging.ImageFormat.Png), "banner.png").ConfigureAwait(false);

                if (BotGuild.ModuleControl.IncludeLogging)
                    BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod(),
                        $"{GUser.Name}'s chat color has been set to {hex}.",
                        Context);
            }

            [Command("argb"), Alias("rgb"), Summary("Set the user's chat text color. Optionally mention a user to set their Chat Color. Input:  String \"Color RGB Code i.e. 255 0 255\" "), Priority(1)]
            public async Task SetColorByARGB(uint R, uint G, uint B)
            {
                var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
                var GUser = BotGuild.GetUser(Context.User);

                if (BadColor(R, G, B))
                {
                    await Context.Channel.SendMessageAsync(Properties.Resources.BadRGB);
                    return;
                }

                GUser.Banner.SetColor(System.Drawing.Color.FromArgb((int)R, (int)G, (int)B));
                PersistentData.SaveChangesToJson();

                await BotGuild.GetChatChannel(Context).SendMessageAsync("Color Set").ConfigureAwait(false);

                var img = await BannerMaker.BuildBannerAsync(Context.User, false);

                await BotGuild.GetChatChannel(Context).SendFileAsync(BannerMaker.ToStream(img, System.Drawing.Imaging.ImageFormat.Png), "banner.png");

                if (BotGuild.ModuleControl.IncludeLogging)
                    BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod(),
                        $"{GUser.Name}'s chat color has been set to {R} {G} {B}.",
                        Context);
            }

            [Command("argb"), Alias("rgb"), Summary("Set the user's chat text color. Optionally mention a user to set their Chat Color. Input:  String \"Color RGB Code i.e. 255 0 255\" SocketGuildUser \"Mentioned Discord User\""), Priority(1)]
            [RequirePermission(Roles.MOD)]
            public async Task SetColorByARGB(uint R, uint G, uint B, SocketGuildUser User = null)
            {
                var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
                var GUser = BotGuild.GetUser(User);
                var DisUser = User;

                if (BadColor(R,G,B))
                {
                    await Context.Channel.SendMessageAsync(Properties.Resources.BadRGB);
                    return;
                }

                GUser.Banner.SetColor(System.Drawing.Color.FromArgb((int)R, (int)G, (int)B));
                PersistentData.SaveChangesToJson();

                await BotGuild.GetChatChannel(Context).SendMessageAsync("Color Set").ConfigureAwait(false);

                var img = await BannerMaker.BuildBannerAsync(DisUser, false);

                await BotGuild.GetChatChannel(Context).SendFileAsync(BannerMaker.ToStream(img, System.Drawing.Imaging.ImageFormat.Png), "banner.png");

                if (BotGuild.ModuleControl.IncludeLogging)
                    BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod(),
                        $"{DisUser.Username}'s chat color has been set to {R} {G} {B}.",
                        Context);
            }

            [Command("name"), Summary("Set the user's chat text color. Input:  String \"Color RGB Code i.e. 255 0 255\""), Priority(1)]
            public async Task SetColorByName(string name)
            {

                var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
                var GUser = BotGuild.GetUser(Context.User);

                var Color = System.Drawing.Color.FromName(name);

                if (Color.Equals(System.Drawing.Color.FromArgb(0)))
                {
                    var Colors = GetColorNames();
                    foreach (var Col in Colors)
                    {
                        await ReplyAsync("", false, Col);
                    }
                    return;
                }

                GUser.Banner.SetColor(Color);
                PersistentData.SaveChangesToJson();

                await BotGuild.GetChatChannel(Context).SendMessageAsync("Color Set");

                var img = await BannerMaker.BuildBannerAsync(Context.User, false);
                await BotGuild.GetChatChannel(Context).SendFileAsync(BannerMaker.ToStream(img, System.Drawing.Imaging.ImageFormat.Png), "banner.png");

                if (BotGuild.ModuleControl.IncludeLogging)
                    BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod(),
                        $"{GUser.Name}'s chat color has been set to {name}.",
                        Context.Guild.Id,
                        (SocketGuildUser)Context.User);
            }

            [Command("name"), Summary("Set the mentioned user's chat text color. Input:  String \"Color RGB Code i.e. 255 0 255\" SocketGuildUser \"Mentioned Discord User\""), Priority(1)]
            [RequirePermission(Roles.MOD)]
            public async Task SetColorByName(string name, SocketGuildUser User)
            {

                var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
                var GUser = BotGuild.GetUser(User);

                var Color = System.Drawing.Color.FromName(name);

                if (Color.Equals(System.Drawing.Color.FromArgb(0)))
                {
                    var Colors = GetColorNames();
                    foreach (var Col in Colors)
                    {
                        await ReplyAsync("", false, Col);
                    }
                    return;
                }

                GUser.Banner.SetColor(Color);
                PersistentData.SaveChangesToJson();

                await BotGuild.GetChatChannel(Context).SendMessageAsync("Color Set");

                var img = await BannerMaker.BuildBannerAsync(User, false);
                await BotGuild.GetChatChannel(Context).SendFileAsync(BannerMaker.ToStream(img, System.Drawing.Imaging.ImageFormat.Png), "banner.png");

                if (BotGuild.ModuleControl.IncludeLogging)
                    BotLogging.PrintLogMessage(
                        MethodBase.GetCurrentMethod(),
                        $"{GUser.Name}'s chat color has been set to {name}.",
                        Context);
            }

            [Command("Chart"), Summary("Display a color codes chart"), Priority(1)]
            public async Task ColorChart()
            {
                EmbedBuilder embed = new EmbedBuilder();
                embed.WithImageUrl(@"https://i.imgur.com/OOmVmMz.png");
                await Context.Channel.SendMessageAsync("", false, embed.Build()).ConfigureAwait(false);
            }
        }
    }
}
