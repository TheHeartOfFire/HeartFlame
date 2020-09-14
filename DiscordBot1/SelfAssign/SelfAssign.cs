using Discord;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Discord.WebSocket;
using DiscordBot1.Misc;
using System.Threading.Tasks;

namespace DiscordBot1.SelfAssign
{
    public class SelfAssign
    {
        public static AllRoles roles;
        private static readonly string containerString = ModuleControl.BotName;
        private const string blobString = "selfassign";
        private static readonly GuildPermissions NoPerms = new GuildPermissions();

        public static async void ConstructorAsync()
        {
            string storageConnectionString = Configuration.Configuration.storageConnectionString;
            CloudStorageAccount account = CloudStorageAccount.Parse(storageConnectionString);
            CloudBlobClient serviceClient = account.CreateCloudBlobClient();

            var container = serviceClient.GetContainerReference(containerString);
            await container.CreateIfNotExistsAsync();
            CloudBlockBlob blob = container.GetBlockBlobReference(blobString);
            string json = "";
            try
            {
                json = await blob.DownloadTextAsync();
            }
            catch (StorageException)
            {
                roles = new AllRoles();

                json = JsonConvert.SerializeObject(roles, Formatting.Indented);
                blob.UploadTextAsync(json).Wait();
                return;
            }
            var data = JsonConvert.DeserializeObject<AllRoles>(json);
            roles = data;
        }

        public static void SaveChangesToJson()
        {
            string storageConnectionString = Configuration.Configuration.storageConnectionString;
            CloudStorageAccount account = CloudStorageAccount.Parse(storageConnectionString);
            CloudBlobClient serviceClient = account.CreateCloudBlobClient();

            var container = serviceClient.GetContainerReference(containerString);
            container.CreateIfNotExistsAsync();
            CloudBlockBlob blob = container.GetBlockBlobReference(blobString);

            string json = JsonConvert.SerializeObject(roles, Formatting.Indented);
            blob.UploadTextAsync(json).Wait();
        }

        public static async Task<Embed> PrefabConsoleAsync(ulong GuildID)
        {
            roles.Consoles = new RoleCategory<RoleObject>()
            {
                Roles = new List<RoleObject>()
            };

            var Guild = Program.Client.GetGuild(GuildID);
            var GuildRoles = new List<SocketRole>(Guild.Roles);

            if (GuildRoles is null || !GuildRoles.Exists(x => x.Name == "⁣ 	  	  	  	  	 ☚Consoles☛ 	  	  	  	  	 ⁣"))
                await Guild.CreateRoleAsync("⁣ 	  	  	  	  	 ☚Consoles☛ 	  	  	  	  	 ⁣", NoPerms, new Color(0x2f3136), false, false);
            if (GuildRoles is null || !GuildRoles.Exists(x => x.Name == "Xbox"))
                await Guild.CreateRoleAsync("Xbox", NoPerms, Color.Green, false, true);
            if (GuildRoles is null || !GuildRoles.Exists(x => x.Name == "PlayStation"))
                await Guild.CreateRoleAsync("PlayStation", NoPerms, Color.Blue, false, true);
            if (GuildRoles is null || !GuildRoles.Exists(x => x.Name == "PC"))
                await Guild.CreateRoleAsync("PC", NoPerms, Color.Default, false, true);
            if (GuildRoles is null || !GuildRoles.Exists(x => x.Name == "Nintendo"))
                await Guild.CreateRoleAsync("Nintendo", NoPerms, Color.Red, false, true);
            GuildRoles = new List<SocketRole>(Guild.Roles);

            roles.Consoles.Name = "Consoles";
            roles.Consoles.Title = "Consoles";

            AddConsole("Xbox", EmoteRef.Emotes.GetValueOrDefault("Xbox"), 1, GuildRoles.FirstOrDefault(x => x.Name == "Xbox").Id);
            AddConsole("PlayStation", EmoteRef.Emotes.GetValueOrDefault("PlayStation"), 2, GuildRoles.FirstOrDefault(x => x.Name == "PlayStation").Id);
            AddConsole("PC", EmoteRef.Emotes.GetValueOrDefault("PC"), 3, GuildRoles.FirstOrDefault(x => x.Name == "PC").Id);
            AddConsole("Nintendo", EmoteRef.Emotes.GetValueOrDefault("Nintendo"), 4, GuildRoles.FirstOrDefault(x => x.Name == "Nintendo").Id);

            SetConsoleDivider(GuildRoles.FirstOrDefault(x => x.Name == "⁣ 	  	  	  	  	 ☚Consoles☛ 	  	  	  	  	 ⁣").Id);

            return ConsoleEmbed();
        }

        public static async Task<Embed> PrefabTimeAsync(ulong GuildID)
        {
            roles.TimeZones = new RoleCategory<RoleObject>()
            {
                Roles = new List<RoleObject>()
            };

            var Guild = Program.Client.GetGuild(GuildID);
            var GuildRoles = new List<SocketRole>(Guild.Roles);

            if (GuildRoles is null || !GuildRoles.Exists(x => x.Name == "⁣⁣ 	  	  	  	  	 ☚TimeZone☛ 	  	  	  	  	 ⁣"))
                await Guild.CreateRoleAsync("⁣⁣ 	  	  	  	  	 ☚TimeZone☛ 	  	  	  	  	 ⁣", NoPerms, new Color(0x2f3136), false, false);
            if (GuildRoles is null || !GuildRoles.Exists(x => x.Name == "US Pacific (-7)")) await Guild.CreateRoleAsync("US Pacific (-7)", NoPerms, Color.Default, false, true);
            if (GuildRoles is null || !GuildRoles.Exists(x => x.Name == "US Mountain (-6)")) await Guild.CreateRoleAsync("US Mountain (-6)", NoPerms, Color.Default, false, true);
            if (GuildRoles is null || !GuildRoles.Exists(x => x.Name == "US Central (-5)")) await Guild.CreateRoleAsync("US Central (-5)", NoPerms, Color.Default, false, true);
            if (GuildRoles is null || !GuildRoles.Exists(x => x.Name == "US Eastern (-4)")) await Guild.CreateRoleAsync("US Eastern (-4)", NoPerms, Color.Default, false, true);
            if (GuildRoles is null || !GuildRoles.Exists(x => x.Name == "Greenwich (+0)")) await Guild.CreateRoleAsync("Greenwich (+0)", NoPerms, Color.Default, false, true);
            if (GuildRoles is null || !GuildRoles.Exists(x => x.Name == "UK (+1)")) await Guild.CreateRoleAsync("UK (+1)", NoPerms, Color.Default, false, true);
            if (GuildRoles is null || !GuildRoles.Exists(x => x.Name == "Australia (+10)")) await Guild.CreateRoleAsync("Australia (+10)", NoPerms, Color.Default, false, true);

            GuildRoles = new List<SocketRole>(Guild.Roles);

            roles.TimeZones.Name = "TimeZones";
            roles.TimeZones.Title = "TimeZones";

            AddTime("US Pacific (-7)", EmoteRef.Emotes.GetValueOrDefault("1"), 1, GuildRoles.FirstOrDefault(x => x.Name == "US Pacific (-7)").Id);
            AddTime("US Mountain (-6)", EmoteRef.Emotes.GetValueOrDefault("2"), 2, GuildRoles.FirstOrDefault(x => x.Name == "US Mountain (-6)").Id);
            AddTime("US Central (-5)", EmoteRef.Emotes.GetValueOrDefault("3"), 3, GuildRoles.FirstOrDefault(x => x.Name == "US Central (-5)").Id);
            AddTime("US Eastern (-4)", EmoteRef.Emotes.GetValueOrDefault("4"), 4, GuildRoles.FirstOrDefault(x => x.Name == "US Eastern (-4)").Id);
            AddTime("Greenwich (+0)", EmoteRef.Emotes.GetValueOrDefault("5"), 5, GuildRoles.FirstOrDefault(x => x.Name == "Greenwich (+0)").Id);
            AddTime("UK (+1)", EmoteRef.Emotes.GetValueOrDefault("6"), 6, GuildRoles.FirstOrDefault(x => x.Name == "UK (+1)").Id);
            AddTime("Australia (+10)", EmoteRef.Emotes.GetValueOrDefault("7"), 7, GuildRoles.FirstOrDefault(x => x.Name == "Australia (+10)").Id);

            SetTimeDivider(GuildRoles.FirstOrDefault(x => x.Name == "⁣⁣ 	  	  	  	  	 ☚TimeZone☛ 	  	  	  	  	 ⁣").Id);

            return TimeEmbed();
        }

        public static void AddConsole(string Name, string Emoji, int Position, ulong RoleID)
        {
            foreach (var role in roles.Consoles.Roles)
            {
                if (role.Position >= Position)
                    role.Position++;
            }

            roles.Consoles.Roles.Add(new RoleObject
            {
                Name = Name,
                Emoji = Emoji,
                Position = Position,
                RoleID = RoleID
            });

            roles.Consoles.Roles.Sort();
            SaveChangesToJson();
        }

        public static void AddTime(string Name, string Emoji, int Position, ulong RoleID)
        {
            foreach (var role in roles.TimeZones.Roles)
            {
                if (role.Position >= Position)
                {
                    role.Position++;
                    Emoji = EmoteRef.Emotes.GetValueOrDefault(Position.ToString());
                }
            }
            roles.TimeZones.Roles.Add(new RoleObject
            {
                Name = Name,
                Emoji = Emoji,
                Position = Position,
                RoleID = RoleID
            });

            roles.TimeZones.Roles.Sort();
            SaveChangesToJson();
        }

        public static Embed ConsoleEmbed()
        {
            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithDescription($"Please select the reaction corresponding to the consoles you use. Removing your reaction will remove the role from you.");

            foreach (var role in roles.Consoles.Roles)
            {
                Embed.AddField($"{role.Emoji} for the {role.Name} role.", ".");
            }
            Embed.WithFooter("");//edit information
            return Embed.Build();
        }

        public static Embed TimeEmbed()
        {
            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithDescription($"Please select the reaction corresponding to your timezone. Removing your reaction will remove the role from you.");

            foreach (var role in roles.TimeZones.Roles)
            {
                Embed.AddField($"{role.Emoji} for the {role.Name} role.", ".");
            }
            Embed.WithFooter("");//edit information
            return Embed.Build();
        }

        public static void SetConsoleMessageID(ulong MessageID)
        {
            roles.Consoles.MsgID = MessageID;
            SaveChangesToJson();
        }

        public static void SetTimeMessageID(ulong MessageID)
        {
            roles.TimeZones.MsgID = MessageID;
            SaveChangesToJson();
        }

        public static void SetConsoleDivider(ulong RoleID)
        {
            roles.Consoles.DividerRoleID = RoleID;
            SaveChangesToJson();
        }

        public static void SetTimeDivider(ulong RoleID)
        {
            roles.TimeZones.DividerRoleID = RoleID;
            SaveChangesToJson();
        }
    }
}