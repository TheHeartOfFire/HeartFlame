using Discord.WebSocket;
using HeartFlame.GuildControl;
using HeartFlame.Misc;
using ImageProcessor;
using ImageProcessor.Imaging;
using ImageProcessor.Imaging.Formats;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HeartFlame.ChatLevels
{
    public class BannerMaker
    {
        private static readonly Point Buffer = new Point(25, 25);
        private static readonly Size TotalSize = new Size(512, 192);
        private static readonly Size InnerSize = new Size(TotalSize.Width - (2 * Buffer.X), TotalSize.Height - (2 * Buffer.X));
        private static readonly int GreyAlpha = 215;
        private static readonly int TestGreyScale = 50;
        private static readonly Point AvatarLocation = new Point(Buffer.X + 8, Buffer.X + 8);
        private static readonly Size AvatarSize = new Size(128, 128);
        private static readonly Size ExpBarSize = new Size(300, 16);
        private static readonly Point ExpBarLocation = new Point(AvatarLocation.X + AvatarSize.Width + 12, (TotalSize.Height / 2) + 12);
        private static readonly Point ExpLocation = new Point(TotalSize.Width - Buffer.X - 13, (TotalSize.Height / 2) - 10);
        private static readonly Point MessagesLocation = new Point(AvatarLocation.X + AvatarSize.Width - 12, TotalSize.Height - Buffer.Y - 10);
        private static readonly Point BadgesLocation = new Point(TotalSize.Width - Buffer.X, TotalSize.Height - Buffer.Y);
        private static readonly Size BadgesBuffer = new Size(8, 6);


        public static async Task<Image> BuildBannerAsync(SocketUser User)
        {
            return await BuildBannerAsync((SocketGuildUser)User);
        }

        public static async Task<Image> BuildBannerAsync(SocketGuildUser user)
        {
            var Guild = GuildManager.GetGuild(user.Guild.Id);
            var User = Guild.GetUser(user); 
            
            
            var imf = new ImageFactory();
            GetBackground(ref imf, User);//user's banner image
            GetOverlay(ref imf);//semitransparent content area
            imf.Overlay(AvatarMask(true));//semitransparent avatar backing
            imf.Overlay(await GetAvatarAsync(user));//Masked avatar image
            imf.Watermark(GetName(User));//User's Name
            imf.Overlay(GetExpBar(User, false));//Exp bar Background
            imf.Overlay(GetExpBar(User, true));//Exp Bar actual Exp
            imf.Watermark(GetExp(User));//current exp / xp to next level
            RankAndLevel(user, ref imf);//Rank and level
            imf.Watermark(GetMessages(User));//User's message count
            AddBadges(User, ref imf);
            Image output = imf.Image;
            PersistentData.SaveChangesToJson();
            return output;

        }

        private static void GetBackground(ref ImageFactory imf, GuildUser User)
        {
            var image = GetBannerAsync(User.Banner.BannerImage).Result;
            imf.Load(image);

            if ((float)image.Height / (float)image.Width > 1f)
                imf.Rotate(270);
            if(User.Banner.HorizontalFlip)
                imf.Flip();
            if (User.Banner.VerticalFlip)
                imf.Flip(true);
            var res = new ResizeLayer(TotalSize)
            {
                ResizeMode = ResizeMode.Stretch
            };
            imf.Resize(res);
        }

        private static void GetOverlay(ref ImageFactory imf)
        {
            var rec = new Rectangle(Buffer, InnerSize);
            SolidBrush RecColor = new SolidBrush(Color.FromArgb(GreyAlpha, TestGreyScale, TestGreyScale, TestGreyScale));
            imf.Format(new PngFormat());
            Graphics.FromImage(imf.Image).FillRectangle(RecColor, rec);
        }

        private static TextLayer GetName(GuildUser User)
        {
            string Name = User.Name;
            if (Name.Length > 10)//TODOL: Make Banner name text size a function of it's length
                Name = Name.Substring(0, 10);

            return new TextLayer()
            {
                Text = Name,
                FontColor = User.Banner.GetColor(),
                FontFamily = new FontFamily("Arial"),
                Position = new Point(AvatarLocation.X + AvatarSize.Width + 10, TotalSize.Height / 2 - TextManagement.SmallNormalCharacter.Height - 5),
                FontSize = 30
            };
        }

        private static async Task<Image> GetBannerAsync(string ImageName = "default")
        {
            if (ImageName is null || ImageName.Equals("default")) return DefaultImage();

            WebClient wc = new WebClient();
            Uri imageUri = (await DownloadBanner(ImageName).ConfigureAwait(false)).PrimaryUri;
            byte[] bytes = wc.DownloadData(imageUri.OriginalString);
            MemoryStream ms = new MemoryStream(bytes);
            wc.Dispose();
            var img = Image.FromStream(ms);
            ms.Dispose();
            return img;
        }

        private static Image DefaultImage()
        {
            var Bit = new Bitmap(TotalSize.Width, TotalSize.Height); 
            for (int x = 0; x < Bit.Width; x++)
            {
                for (int y = 0; y < Bit.Height; y++)
                {
                    Bit.SetPixel(x, y, Color.Black);
                }
            }

            var Stream = new MemoryStream();
            Bit.Save(Stream, ImageFormat.Png);


            return Image.FromStream(Stream);

        }

        private static async Task<StorageUri> DownloadBanner(string imageName)
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(Properties.Resources.StorageToken);
            CloudBlobClient serviceClient = account.CreateCloudBlobClient();
            var container = serviceClient.GetContainerReference("images");
            await container.CreateIfNotExistsAsync().ConfigureAwait(false);
            CloudBlockBlob blob = container.GetBlockBlobReference(PersistentData.BotName + "/" + imageName);

            return blob.StorageUri;
        }

        private static int GetExpLength(GuildUser User)
        {
            float maxW = ExpBarSize.Width;
            int userLevel = User.Chat.ChatLevel;
            if (userLevel == 0)
            {
                userLevel++;
                User.Chat.ChatLevel++;
            }

            float exp = 0;
            if (User.Chat.ChatExp > 0)
                exp = User.Chat.ChatExp - LevelManagement.GetExpAtLevel(userLevel - 1);

            float nextLv = LevelManagement.GetExpAtLevel(userLevel) - LevelManagement.GetExpAtLevel(userLevel - 1);
            exp /= nextLv;
            exp *= maxW;
            return (int)exp;
        }

        public static Stream ToStream(Image image, ImageFormat format)
        {
            var stream = new MemoryStream();
            try { image.Save(stream, format); }
            catch (ArgumentException e)
            { Console.WriteLine(e.InnerException.Message); }

            stream.Position = 0;
            return stream;
        }
        
        private static async Task<byte[]> DownloadAvatar(SocketGuildUser User)
        {
            var GUser = GuildManager.GetGuild(User).GetUser(User);
            var DownloadString = User.GetAvatarUrl();
            if (User.AvatarId == null)
                DownloadString = User.GetDefaultAvatarUrl();
            DownloadString = DownloadString.Substring(0, DownloadString.IndexOf("png") + 3);

            if(GUser.Banner.ProfileImage != null && !GUser.Banner.ProfileImage.Equals("default"))
            {
                Uri uri = (await DownloadBanner(GUser.Banner.ProfileImage)).PrimaryUri;
                DownloadString = uri.OriginalString;
            }

            byte[] bytes;
            using WebClient client = new WebClient();
            try
            {
                bytes = client.DownloadData(DownloadString);
            }
            catch (WebException e)
            {
                Console.WriteLine(e.Message);
                DownloadString = User.GetDefaultAvatarUrl();
                bytes = client.DownloadData(DownloadString);
            }
            client.Dispose();
            return bytes;
        }

        private static async Task<ImageLayer> GetAvatarAsync(SocketGuildUser user)
        {

            byte[] bytes = await DownloadAvatar(user);
            
            MemoryStream ms = new MemoryStream(bytes);
            var imf = new ImageFactory();
            imf.Load(Image.FromStream(ms));
            imf.Resize(new ResizeLayer(AvatarSize) {ResizeMode = ResizeMode.Stretch });
            imf.Format(new PngFormat());
            imf.Mask(AvatarMask(false));
            ImageLayer il = new ImageLayer
            {
                Image = imf.Image,
                Position = AvatarLocation,
                Size = AvatarSize
            };
            ms.Dispose();
            return il;
        }

        private static void RankAndLevel(SocketGuildUser User, ref ImageFactory imf)
        {
            var Guild = GuildManager.GetGuild(User);
            var GUser = Guild.GetUser(User);
            Guild.Users.Sort();
            var Rank = Guild.Users.IndexOf(GUser) + 1;
            if (Reporting.ReportingManager.HighestChat().User.Equals(GUser))
                Rank = 0;
            //Rank = 0;
            var RankDisplay = Rank.ToString();
            if (Rank == 0)
                RankDisplay = "1";
            GetRnLLayers(GUser, true, -1, GUser.Chat.ChatLevel.ToString(), new Point(TotalSize.Width - Buffer.X - 8, Buffer.Y), ref imf, out Point LevelValue);
            GetRnLLayers(GUser, false, -1, "Level", new Point(LevelValue.X - 5, Buffer.Y + LevelValue.Y + 4), ref imf, out Point LevelLabel);
            GetRnLLayers(GUser, true, Rank, $"#{RankDisplay}", new Point(LevelLabel.X - 10, Buffer.Y), ref imf, out Point RankValue); 
            GetRnLLayers(GUser, false, Rank, $"Rank", new Point(RankValue.X - 5, Buffer.Y + RankValue.Y + 4), ref imf, out _);

        }

        private static void GetRnLLayers(GuildUser User, bool Big, int Rank, string Text, Point Offset, ref ImageFactory imf, out Point Location)
        {
            int Size = 20;
            int Y = 0;
            if (Big)
                Size = 40;

            var Layer = new TextLayer()
            {
                Text = Text,
                FontSize = Size,
                FontColor = Color.White,
                FontFamily = new FontFamily("Arial")
            };
            if (!Big)
                Y = TextManagement.GetSize(Layer).Height;

            if (Rank < 0)
                Layer.FontColor = User.Banner.GetColor();
            else
                GetRankColor(Rank, User, ref Layer);


            Layer.Position = new Point(Offset.X - TextManagement.GetSize(Layer).Width, Offset.Y - Y);
            imf.Watermark(Layer);

            Location = Layer.Position.Value;
        }


        private static void GetRankColor(int Rank, GuildUser User, ref TextLayer Layer)
        {
            if (Rank == 0)
            {
                Layer.FontColor = Color.RoyalBlue;
                User.Banner.Badges.Global.Rank1 = true;
                User.Banner.Badges.Rank1 = true;
            }
            if (Rank == 1)
            {
                Layer.FontColor = Color.Goldenrod;
                User.Banner.Badges.Rank1 = true;
            }
            if (Rank == 2)
            {
                Layer.FontColor = Color.FromArgb(255, 145, 145, 145);
                User.Banner.Badges.Rank2 = true;
            }
            if (Rank == 3)
            {
                Layer.FontColor = Color.FromArgb(255, 164, 102, 40);
                User.Banner.Badges.Rank3 = true;
            }
        }

        private static ImageLayer AvatarMask(bool background)
        {
            var bit = new Bitmap(AvatarSize.Width, AvatarSize.Height, PixelFormat.Format32bppPArgb);
            var offset = AvatarSize.Width / 2;
            var col = Color.FromArgb(0, 0, 0, 0);
            if (background)
                col = Color.FromArgb(100, 0, 0, 0);
            for (int x = 0; x < AvatarSize.Width; x++)
            {
                for(int y = 0; y < AvatarSize.Height; y++)
                {
                    if (Square(x - offset) + Square(y - offset) <= Square(offset))
                        bit.SetPixel(x, y, col);
                    else
                        bit.SetPixel(x,y,Color.Empty);
                }
            }
            var stream = new MemoryStream();
            bit.Save(stream, ImageFormat.Png);

            ImageFactory imf = new ImageFactory();
            var img = Image.FromStream(stream);
            stream.Dispose();
            imf.Load(img);
            imf.Format(new PngFormat());
            var il = new ImageLayer
            {
                Image = imf.Image,
                Position = AvatarLocation
            };
            return il;

            
        }

        private static float Square(int NumToSquare)
        {
            return (float)NumToSquare * (float)NumToSquare;
        }

        private static ImageLayer GetExpBar(GuildUser User, bool fore)
        {
            var Background = new Rectangle(0,0, ExpBarSize.Width, ExpBarSize.Height);
            var width = ExpBarSize.Width;
            if (fore)
                width = GetExpLength(User);
            if (width == 0)
                width++;

            var Bit = new Bitmap(width, ExpBarSize.Height, PixelFormat.Format32bppArgb); 
            for (int x = 0; x < Bit.Width; x++)
            {
                for (int y = 0; y < Bit.Height; y++)
                {
                    Bit.SetPixel(x, y, Color.Black);
                }
            }

            var Stream = new MemoryStream();
            Bit.Save(Stream, ImageFormat.Png);


            var imf = new ImageFactory();
            imf.Load(Image.FromStream(Stream));
            Stream.Dispose();
            var Brush = new SolidBrush(Color.Gray);
            if(fore)
                Brush = new SolidBrush(User.Banner.GetColor());
            Graphics.FromImage(imf.Image).FillRectangle(Brush, Background);
            Brush.Dispose();
            imf.RoundedCorners(7);
            var il = new ImageLayer() { Image = imf.Image, Position = ExpBarLocation };

            return il;
        }

        private static TextLayer GetExp(GuildUser User)
        {
            var Output = new TextLayer
            {
                Text = $"{User.Chat.ChatExp}/{LevelManagement.GetExpAtLevel(User.Chat.ChatLevel)}",
                FontColor = Color.White,
                FontFamily = new FontFamily("Arial"),
                Position = ExpLocation,
                FontSize = 20
            };
            Output.Position = new Point(Output.Position.Value.X - TextManagement.GetSize(Output).Width, ExpLocation.Y);
            return Output;

        }

        private static TextLayer GetMessages(GuildUser User)
        {
            var Output = new TextLayer
            {
                Text = $"Messages: {User.Chat.MessagesSent}",
                FontColor = Color.White,
                FontFamily = new FontFamily("Arial"),
                Position = MessagesLocation,
                FontSize = 20
            };
            Output.Position = new Point(Output.Position.Value.X, MessagesLocation.Y - TextManagement.GetSize(Output).Height);
            return Output;

        }

        private static async Task<List<Image>> GetBadges(GuildUser User)
        {
            var output = new List<Image>();

            if (User.Banner.Badges.Global.Patreon)
                output.Add(await GetBannerAsync("patreon24"));
            if (User.Banner.Badges.Global.BetaTester)
                output.Add(await GetBannerAsync("betatester24"));
            if (User.Banner.Badges.Global.Rank1)
                output.Add(await GetBannerAsync("goldenglobe24"));

            if (User.Banner.Badges.Rank1)
                output.Add(await GetBannerAsync("goldmedal24"));
            if (User.Banner.Badges.Rank2)
                output.Add(await GetBannerAsync("silvermedal24"));
            if (User.Banner.Badges.Rank3)
                output.Add(await GetBannerAsync("bronzemedal24"));

            return output;
        }

        private static void AddBadges(GuildUser User, ref ImageFactory imf)
        {
            var Badges = GetBadges(User).Result;
            if (Badges.Count == 0) return;
            Badges.Reverse();
            for (var x = Badges.Count; x > 0; x--)
            {
                int BadgesOffset = x * Badges[0].Size.Width;
                int BufferOffset = x * BadgesBuffer.Width;
                int XCoord = BadgesLocation.X - BadgesOffset - BufferOffset;
                int YCoord = BadgesLocation.Y - Badges[0].Size.Height;

                var BadgeLayer = new ImageLayer
                {
                    Position = new Point(XCoord, YCoord),
                    Image = Badges[x - 1]
                };
                imf.Overlay(BadgeLayer);
            }

        }
    }
}