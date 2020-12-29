using Discord;
using Discord.Commands;
using Discord.WebSocket;
using HeartFlame.GuildControl;
using HeartFlame.Misc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HeartFlame.ChatLevels
{
    [Group("Level"), Alias("lv", "lvl")]
    public class ChatCommands : ModuleBase<SocketCommandContext>
    {
        [Command(""), Summary("Get the user's current chat level. Optionally mention another user to get their level. Input: SocketGuildUser \"Mentioned Discord User\" ")]
        public async Task GetLevel(SocketGuildUser User = null)
        {
            var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
            if (!BotGuild.ModuleControl.IncludeChat)
            {
                await ReplyAsync(Properties.Resources.NotChat);
                return;
            }

            var DisUser = (SocketGuildUser)Context.User;
            if (User != null)
                DisUser = User;

            System.Drawing.Image img = null;

            try
            {
                img = await BannerMaker.BuildBannerAsync(DisUser, false);
            }
            catch (System.IO.FileLoadException e)
            {
                Console.WriteLine(e.InnerException.Message);
            }
            if (BotGuild.Configuration.UseChatChannel)
            {
                var IDs = BotGuild.Configuration.ChatChannel;
                foreach (var id in IDs)
                {
                    await (Program.Client.GetChannel(id) as ISocketMessageChannel).SendFileAsync(BannerMaker.ToStream(img, System.Drawing.Imaging.ImageFormat.Png), "banner.png");
                }
            }
            else
                await Context.Channel.SendFileAsync(BannerMaker.ToStream(img, System.Drawing.Imaging.ImageFormat.Png), "banner.png").ConfigureAwait(false);
        }

        [Command("Ranking"),Alias("Top", "Leaders", "LeaderBoard", "Leader Board", "rank", "r"), Summary("Get the top 10 chat levels.")]
        public async Task GetRankings()
        {
            var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
            if (!BotGuild.ModuleControl.IncludeChat)
            {
                await ReplyAsync(Properties.Resources.NotChat);
                return;
            }

            if (BotGuild.Configuration.UseChatChannel)
            {
                var IDs = BotGuild.Configuration.ChatChannel;
                foreach (var id in IDs)
                {
                    await (Program.Client.GetChannel(id) as ISocketMessageChannel).SendMessageAsync("", false, LevelManagement.Top10(Context.Guild.Id));
                }
            }
            else
                await Context.Channel.SendMessageAsync("", false, LevelManagement.Top10(Context.Guild.Id));
        }

        [Command("Help"), Alias("h", "?"), Summary("Get all of the commands in the Permissions Group"), Remarks("ChatCommandHelp"), Priority(1)]
        public async Task ChatCommandsHelp()
        {
            var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
            if (!BotGuild.ModuleControl.IncludeChat)
            {
                await ReplyAsync(Properties.Resources.NotChat);
                return;
            }

            var embeds = Configuration.Configuration_Command.HelpEmbed("Chat Help", "ChatCommandHelp", 0);
            foreach (var embed in embeds)
            {
                await ReplyAsync("", false, embed);
            }
        }

        [Group("Image"), Alias("img")]
        public class ChatCommandsImage : ModuleBase<SocketCommandContext>
        {
            [Command(""), Alias("h", "?", "help"), Summary("Get all of the commands in the ChatImage Group"), Remarks("ChatCommandImageHelp")]
            public async Task ChatCommandImageHelp()
            {
                var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
                if (!BotGuild.ModuleControl.IncludeChat)
                {
                    await ReplyAsync(Properties.Resources.NotChat);
                    return;
                }

                var embeds = Configuration.Configuration_Command.HelpEmbed("Chat Image Help", "ChatCommandImageHelp", 1);
                foreach (var embed in embeds)
                {
                    await ReplyAsync("", false, embed);
                }
            }

            [Command("Banner"),Alias("b"), Summary("Set the user's banner image. Input: SocketGuildUser \"Mentioned Discord User\" String \"Banner Image Name[Blank = default]\""), Priority(1)]
            public async Task BannerSet(SocketGuildUser User, string name = "default")
            {
                var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
                var GUser = BotGuild.GetUser((SocketGuildUser)Context.User);
                if (!BotGuild.ModuleControl.IncludeChat)
                {
                    await ReplyAsync(Properties.Resources.NotChat);
                    return;
                }

                if (!GUser.Perms.Admin)
                {
                    await ReplyAsync(Properties.Resources.NotAdmin);
                    return;
                }

                GUser.Banner.BannerImage = name;
                PersistentData.SaveChangesToJson();

                System.Drawing.Image img = null;

                try
                {
                    img = await BannerMaker.BuildBannerAsync(User, false);
                }
                catch (System.IO.FileLoadException e)
                {
                    Console.WriteLine(e.InnerException.Message);
                }

                if (BotGuild.Configuration.UseChatChannel)
                {
                    var IDs = BotGuild.Configuration.ChatChannel;
                    foreach (var id in IDs)
                    {
                        await (Program.Client.GetChannel(id) as ISocketMessageChannel).SendFileAsync(BannerMaker.ToStream(img, System.Drawing.Imaging.ImageFormat.Png), "banner.png");
                    }
                }
                else
                    await Context.Channel.SendFileAsync(BannerMaker.ToStream(img, System.Drawing.Imaging.ImageFormat.Png), "banner.png").ConfigureAwait(false);

                if (BotGuild.ModuleControl.IncludeLogging)
                    BotLogging.PrintLogMessage(
                        "ChatCommands.ChatCommandsImage.SetBanner(SocketGuildUser User, String ImageName)",
                        "Set the user's banner image.",
                        $"{User.Username}'s banner has been set to {name}.",
                        Context.Guild.Id,
                        (SocketGuildUser)Context.User);
            }

            [Command("Profile"),Alias("avatar"), Summary("Set the user's profile image. Input: SocketGuildUser \"Mentioned Discord User\" String \"Profile Image Name[Blank = default]\""), Priority(1)]
            public async Task ProfileSet(SocketGuildUser User, string name = "default")
            {
                var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
                var GUser = BotGuild.GetUser((SocketGuildUser)Context.User);
                if (!BotGuild.ModuleControl.IncludeChat)
                {
                    await ReplyAsync(Properties.Resources.NotChat);
                    return;
                }

                if (BotGuild.ModuleControl.IncludePermissions)
                    if (!GUser.Perms.Admin)
                {
                    await ReplyAsync(Properties.Resources.NotAdmin);
                    return;
                }

                GUser.Banner.BannerImage = name;
                PersistentData.SaveChangesToJson();

                System.Drawing.Image img = null;

                try
                {
                    img = await BannerMaker.BuildBannerAsync(User, false);
                }
                catch (System.IO.FileLoadException e)
                {
                    Console.WriteLine(e.InnerException.Message);
                }

                if (BotGuild.Configuration.UseChatChannel)
                {
                    var IDs = BotGuild.Configuration.ChatChannel;
                    foreach (var id in IDs)
                    {
                        await (Program.Client.GetChannel(id) as ISocketMessageChannel).SendFileAsync(BannerMaker.ToStream(img, System.Drawing.Imaging.ImageFormat.Png), "banner.png");
                    }
                }
                else
                    await Context.Channel.SendFileAsync(BannerMaker.ToStream(img, System.Drawing.Imaging.ImageFormat.Png), "banner.png").ConfigureAwait(false);

                if (BotGuild.ModuleControl.IncludeLogging)
                    BotLogging.PrintLogMessage(
                        "ChatCommands.ChatCommandsImage.SetProfile(SocketGuildUser User, String ImageName)",
                        "Set the user's profile image.",
                        $"{User.Username}'s profile has been set to {name}.",
                        Context.Guild.Id,
                        (SocketGuildUser)Context.User);
            }

            [Command("Background"), Alias("back"), Summary("Toggle the user's text background. Input: SocketGuildUser \"Mentioned Discord User\" Bool \"Background Active?\""), Priority(1)]
            public async Task BackgroundSet(SocketGuildUser User, bool Active = true)
            {
                var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
                var GUser = BotGuild.GetUser((SocketGuildUser)Context.User);
                if (!BotGuild.ModuleControl.IncludeChat)
                {
                    await ReplyAsync(Properties.Resources.NotChat);
                    return;
                }


                if (BotGuild.ModuleControl.IncludePermissions)
                    if (!GUser.Perms.Admin && User.Id != Context.User.Id)
                    {
                        await ReplyAsync(Properties.Resources.NotAdmin);
                        return;
                    }

                if (User.Id != Context.User.Id) GUser = BotGuild.GetUser(User);
                GUser.Banner.TextBackground = Active;
                PersistentData.SaveChangesToJson();

                System.Drawing.Image img = null;

                try
                {
                    img = await BannerMaker.BuildBannerAsync(User, false);
                }
                catch (System.IO.FileLoadException e)
                {
                    Console.WriteLine(e.InnerException.Message);
                }

                if (BotGuild.Configuration.UseChatChannel)
                {
                    var IDs = BotGuild.Configuration.ChatChannel;
                    foreach (var id in IDs)
                    {
                        await (Program.Client.GetChannel(id) as ISocketMessageChannel).SendFileAsync(BannerMaker.ToStream(img, System.Drawing.Imaging.ImageFormat.Png), "banner.png");
                    }
                }
                else
                    await Context.Channel.SendFileAsync(BannerMaker.ToStream(img, System.Drawing.Imaging.ImageFormat.Png), "banner.png").ConfigureAwait(false);

                string msg = "off";
                if (Active)
                    msg = "on";

                if (BotGuild.ModuleControl.IncludeLogging)
                    BotLogging.PrintLogMessage(
                        "ChatCommands.ChatCommandsImage.SetBackground(SocketGuildUser User, Bool Active)",
                        "Toggle the user's text background.",
                        $"{User.Username}'s text background has been turned {msg}.",
                        Context.Guild.Id,
                        (SocketGuildUser)Context.User);
            }

            [Command("Greyscale"),Alias("grey"), Summary("Set the user's text background greyscale value. Input: SocketGuildUser \"Mentioned Discord User\" int \"Greyscale value 0-255[Default = 227]\""), Priority(1)]
            public async Task grayscaleSet(SocketGuildUser User, int Greyscale = 227)
            {
                var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
                var GUser = BotGuild.GetUser((SocketGuildUser)Context.User);
                if (!BotGuild.ModuleControl.IncludeChat)
                {
                    await ReplyAsync(Properties.Resources.NotChat);
                    return;
                }

                if (BotGuild.ModuleControl.IncludePermissions)
                    if (!GUser.Perms.Admin && User.Id != Context.User.Id)
                    {
                        await ReplyAsync(Properties.Resources.NotAdmin);
                        return;
                    }

                if (User.Id != Context.User.Id) GUser = BotGuild.GetUser(User);

                GUser.Banner.Greyscale = Greyscale;
                PersistentData.SaveChangesToJson();

                System.Drawing.Image img = null;

                try
                {
                    img = await BannerMaker.BuildBannerAsync(User, false);
                }
                catch (System.IO.FileLoadException e)
                {
                    Console.WriteLine(e.InnerException.Message);
                }

                if (BotGuild.Configuration.UseChatChannel)
                {
                    var IDs = BotGuild.Configuration.ChatChannel;
                    foreach (var id in IDs)
                    {
                        await (Program.Client.GetChannel(id) as ISocketMessageChannel).SendFileAsync(BannerMaker.ToStream(img, System.Drawing.Imaging.ImageFormat.Png), "banner.png");
                    }
                }
                else
                    await Context.Channel.SendFileAsync(BannerMaker.ToStream(img, System.Drawing.Imaging.ImageFormat.Png), "banner.png").ConfigureAwait(false);

                if (BotGuild.ModuleControl.IncludeLogging)
                    BotLogging.PrintLogMessage(
                        "ChatCommands.ChatCommandsImage.SetGreyscale(SocketGuildUser User, int Greyscale = 227)",
                        "Set the user's text background greyscale value.",
                        $"{User.Username}'s greyscale value has been set to {Greyscale}.",
                        Context.Guild.Id,
                        (SocketGuildUser)Context.User);
            }
        }

        [Group("Color")]
        public class ChatCommandsColor : ModuleBase<SocketCommandContext>
        {
            [Command(""), Alias("h", "?", "help"), Summary("Get all of the commands in the Chat Color Group"), Remarks("ChatCommandColorHelp")]
            public async Task ChatCommandColorHelp()
            {
                var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
                if (!BotGuild.ModuleControl.IncludeChat)
                {
                    await ReplyAsync(Properties.Resources.NotChat);
                    return;
                }

                var embeds = Configuration.Configuration_Command.HelpEmbed("Chat Color Help", "ChatCommandColorHelp", 1);
                foreach (var embed in embeds)
                {
                    await ReplyAsync("", false, embed);
                }
            }

            [Command("hex"), Alias("h"), Summary("Set the user's chat text color. Optionally mention a user to set their Chat Color. Input:String \"Color Hex Code i.e. FF00FF\" SocketGuildUser \"Mentioned Discord User\""), Priority(1)]
            public async Task SetColor(string hex, SocketGuildUser User = null)
            {
                var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
                var GUser = BotGuild.GetUser(User);
                if (!BotGuild.ModuleControl.IncludeChat)
                {
                    await ReplyAsync(Properties.Resources.NotChat);
                    return;
                }

                var DisUser = User;
                if (User is null) 
                {
                    DisUser = (SocketGuildUser)Context.User;
                    GUser = BotGuild.GetUser((SocketGuildUser)Context.User);
                } 
                else if (BotGuild.ModuleControl.IncludePermissions && !BotGuild.GetUser((SocketGuildUser)Context.User).Perms.Mod)
                {
                    await ReplyAsync(Properties.Resources.NotMod);
                    return;
                }

                if (hex.Length != 6 && !OnlyHexInString(hex))
                {
                    await Context.Channel.SendMessageAsync("The wasn't the right kind of data. \n" +
                        "Please make sure to use a hexadecimal color code like FFFFFF. \n" +
                        "If you typed #FFFFFF please omit the #.").ConfigureAwait(false);
                    return;
                }


                GUser.Banner.SetColor(ColorTranslator.FromHtml("#" + hex));
                PersistentData.SaveChangesToJson();

                if (BotGuild.Configuration.UseChatChannel)
                {
                    var IDs = BotGuild.Configuration.ChatChannel;
                    foreach (var id in IDs)
                    {
                        await (Program.Client.GetChannel(id) as ISocketMessageChannel).SendMessageAsync($"Color set to #{hex}").ConfigureAwait(false);
                    }
                }
                else
                    await Context.Channel.SendMessageAsync($"Color set to #{hex}").ConfigureAwait(false);

                System.Drawing.Image img = null;

                try
                {
                    img = await BannerMaker.BuildBannerAsync(DisUser, false);
                }
                catch (System.IO.FileLoadException e)
                {
                    Console.WriteLine(e.InnerException.Message);
                }

                if (BotGuild.Configuration.UseChatChannel)
                {
                    var IDs = BotGuild.Configuration.ChatChannel;
                    foreach (var id in IDs)
                    {
                        await (Program.Client.GetChannel(id) as ISocketMessageChannel).SendFileAsync(BannerMaker.ToStream(img, System.Drawing.Imaging.ImageFormat.Png), "banner.png");
                    }
                }
                else
                    await Context.Channel.SendFileAsync(BannerMaker.ToStream(img, System.Drawing.Imaging.ImageFormat.Png), "banner.png").ConfigureAwait(false);

                if (BotGuild.ModuleControl.IncludeLogging)
                    BotLogging.PrintLogMessage(
                        "ChatCommands.ChatCommandsColor.SetColor(string Hex Code, SocketGuildUser User)",
                        "Set the user's chat text color. Optionally mention a user to set their Chat Color",
                        $"{DisUser.Username}'s chat color has been set to {hex}.",
                        Context.Guild.Id,
                        (SocketGuildUser)Context.User);
            }

            [Command("argb"), Alias("rgb"), Summary("Set the user's chat text color. Optionally mention a user to set their Chat Color. Input:  String \"Color RGB Code i.e. 255 0 255\" SocketGuildUser \"Mentioned Discord User\""), Priority(1)]
            public async Task SetColorByARGB(int R, int G, int B, SocketGuildUser User = null)
            {
                var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
                var GUser = BotGuild.GetUser(User);
                if (!BotGuild.ModuleControl.IncludeChat)
                {
                    await ReplyAsync(Properties.Resources.NotChat);
                    return;
                }

                var DisUser = User;
                if (User is null)
                {
                    DisUser = (SocketGuildUser)Context.User;
                    GUser = BotGuild.GetUser((SocketGuildUser)Context.User);
                }
                else if (BotGuild.ModuleControl.IncludePermissions && !BotGuild.GetUser((SocketGuildUser)Context.User).Perms.Mod)
                {
                    await ReplyAsync(Properties.Resources.NotMod);
                    return;
                }

                if (
                    R < 0 || R > 255 ||
                    G < 0 || G > 255 ||
                    B < 0 || B > 255
                    )
                {
                    await Context.Channel.SendMessageAsync("That wasn't the right king of data.\n" +
                        "Please make sure to type an rgb color code like 255 255 255.\n" +
                        "Make sure to leave a space between each value.").ConfigureAwait(false);
                    return;
                }

                GUser.Banner.SetColor(System.Drawing.Color.FromArgb(R, G, B));
                PersistentData.SaveChangesToJson();

                if (BotGuild.Configuration.UseChatChannel)
                {
                    var IDs = BotGuild.Configuration.ChatChannel;
                    foreach (var id in IDs)
                    {
                        await (Program.Client.GetChannel(id) as ISocketMessageChannel).SendMessageAsync("Color Set").ConfigureAwait(false);
                    }
                }
                else
                    await Context.Channel.SendMessageAsync("Color Set").ConfigureAwait(false);

                System.Drawing.Image img = null;

                try
                {
                    img = await BannerMaker.BuildBannerAsync(DisUser, false);
                }
                catch (System.IO.FileLoadException e)
                {
                    Console.WriteLine(e.InnerException.Message);
                }
                if (BotGuild.Configuration.UseChatChannel)
                {
                    var IDs = BotGuild.Configuration.ChatChannel;
                    foreach (var id in IDs)
                    {
                        await (Program.Client.GetChannel(id) as ISocketMessageChannel).SendFileAsync(BannerMaker.ToStream(img, System.Drawing.Imaging.ImageFormat.Png), "banner.png");
                    }
                }
                else
                    await Context.Channel.SendFileAsync(BannerMaker.ToStream(img, System.Drawing.Imaging.ImageFormat.Png), "banner.png").ConfigureAwait(false);

                if (BotGuild.ModuleControl.IncludeLogging)
                    BotLogging.PrintLogMessage(
                        "ChatCommands.ChatCommandsColor.SetColor(int Red, int Green, int Blue, SocketGuildUser User)",
                        "Set the user's chat text color. Optionally mention a user to set their Chat Color",
                        $"{DisUser.Username}'s chat color has been set to {R} {G} {B}.",
                        Context.Guild.Id,
                        (SocketGuildUser)Context.User);
            }

            [Command("name"), Summary("Set the user's chat text color. Optionally mention a user to set their Chat Color. Input:  String \"Color RGB Code i.e. 255 0 255\" SocketGuildUser \"Mentioned Discord User\""), Priority(1)]
            public async Task SetColorByName(string name, SocketGuildUser User = null)
            {

                var BotGuild = GuildManager.GetGuild(Context.Guild.Id);
                var GUser = BotGuild.GetUser(User);
                if (!BotGuild.ModuleControl.IncludeChat)
                {
                    await ReplyAsync(Properties.Resources.NotChat);
                    return;
                }

                var DisUser = User;
                if (User is null)
                {
                    DisUser = (SocketGuildUser)Context.User;
                    GUser = BotGuild.GetUser((SocketGuildUser)Context.User);
                }
                else if (BotGuild.ModuleControl.IncludePermissions && !BotGuild.GetUser((SocketGuildUser)Context.User).Perms.Mod)
                {
                    await ReplyAsync(Properties.Resources.NotMod);
                    return;
                }

                Type colorType = typeof(System.Drawing.Color);
                PropertyInfo ColorInfo = colorType.GetProperty(name);

                if (ColorInfo is null)
                {
                    var Colors = GetColorNames();
                    foreach (var Color in Colors)
                    {
                        await ReplyAsync("", false, Color);
                    }
                    return;
                }

                GUser.Banner.SetColor((System.Drawing.Color)ColorInfo.GetValue(colorType));
                PersistentData.SaveChangesToJson();

                if (BotGuild.Configuration.UseChatChannel)
                {
                    var IDs = BotGuild.Configuration.ChatChannel;
                    foreach (var id in IDs)
                    {
                        await (Program.Client.GetChannel(id) as ISocketMessageChannel).SendMessageAsync("Color Set").ConfigureAwait(false);
                    }
                }
                else
                    await Context.Channel.SendMessageAsync("Color Set").ConfigureAwait(false);

                System.Drawing.Image img = null;

                try
                {
                    img = await BannerMaker.BuildBannerAsync(DisUser, false);
                }
                catch (System.IO.FileLoadException e)
                {
                    Console.WriteLine(e.InnerException.Message);
                }

                GUser.Banner.SetColor((System.Drawing.Color)ColorInfo.GetValue(colorType));
                PersistentData.SaveChangesToJson();

                if (BotGuild.Configuration.UseChatChannel)
                {
                    var IDs = BotGuild.Configuration.ChatChannel;
                    foreach (var id in IDs)
                    {
                        await (Program.Client.GetChannel(id) as ISocketMessageChannel).SendFileAsync(BannerMaker.ToStream(img, System.Drawing.Imaging.ImageFormat.Png), "banner.png");
                    }
                }
                else
                    await Context.Channel.SendFileAsync(BannerMaker.ToStream(img, System.Drawing.Imaging.ImageFormat.Png), "banner.png").ConfigureAwait(false);

                if (BotGuild.ModuleControl.IncludeLogging)
                    BotLogging.PrintLogMessage(
                        "ChatCommands.ChatCommandsColor.SetColor(int Red, int Green, int Blue, SocketGuildUser User)",
                        "Set the user's chat text color. Optionally mention a user to set their Chat Color",
                        $"{DisUser.Username}'s chat color has been set to {name}.",
                        Context.Guild.Id,
                        (SocketGuildUser)Context.User);
            }

            [Command("Chart"), Summary("Display a color codes chart"), Priority(1)]
            public async Task ColorChart()
            {
                EmbedBuilder embed = new EmbedBuilder();
                embed.WithImageUrl(@"https://i.imgur.com/OOmVmMz.png");
                await Context.Channel.SendMessageAsync("", false, embed.Build()).ConfigureAwait(false);
            }
        }

        public static bool OnlyHexInString(string test)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(test, @"\A\b[0-9a-fA-F]+\b\Z");
        }

        public static List<Embed> GetColorNames()
        {
            var output = new List<Embed>();

            Type colorType = typeof(System.Drawing.Color);
            PropertyInfo[] ColorInfo = colorType.GetProperties();

            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithAuthor("Faitie");
            Embed.WithTitle("The following color names are recognized.");

            foreach (var color in ColorInfo)
            {
                Embed.AddField($"Name: {color.Name}", ".");
                if (Embed.Fields.Count >= 20)
                {
                    output.Add(Embed.Build());
                    Embed = new EmbedBuilder();
                }
            }

            return output;
        }
    }
}