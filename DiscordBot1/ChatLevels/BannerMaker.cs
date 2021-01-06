﻿using Discord.WebSocket;
using HeartFlame.GuildControl;
using HeartFlame.Misc;
using ImageProcessor;
using ImageProcessor.Imaging;
using ImageProcessor.Imaging.Formats;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
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
        private static readonly Point AvatarLocation = new Point(Buffer.X + 8, Buffer.X + 8);
        private static readonly Size AvatarSize = new Size(128, 128);



        public static async Task<Image> BuildBannerAsync(SocketUser User, bool LevelUp)
        {
            return await BuildBannerAsync((SocketGuildUser)User, LevelUp);
        }

        public static async Task<Image> BuildBannerAsync(SocketGuildUser user, bool levelUp)
        {
            var Guild = GuildManager.GetGuild(user.Guild.Id);
            var User = Guild.GetUser(user);

            var color = User.Banner.GetColor();

            Image image = await GetBannerAsync(User.Banner.BannerImage);
            ImageFactory imf = new ImageFactory();//get banner
            imf.Load(image);
            image.Dispose();

            ImageLayer iL = await GetUserAvatarAsync(user);
            imf.Overlay(iL);//add avatar
            iL.Dispose();

            if (User.Banner.TextBackground)
            {
                int greyscale = User.Banner.Greyscale;
                SolidBrush bgColor = new SolidBrush(System.Drawing.Color.FromArgb(215, greyscale, greyscale, greyscale));
                Graphics.FromImage(imf.Image).FillRectangle(bgColor, GetTextBackground());//add background for text
                bgColor.Dispose();
            }

            string overlayText = user.Username;//build display string
            if (user.Nickname != null)
                overlayText = user.Nickname;

            if (levelUp)
            {
                overlayText += $" has just advanced to level {User.Chat.ChatLevel}. Congratulations!\n" +
                    $"Total Messages: {User.Chat.MessagesSent}\n" +
                    $"Current Experience: {User.Chat.ChatExp}/{LevelManagement.GetExpAtLevel(User.Chat.ChatLevel)}";
            }
            else
            {
                overlayText += $"\nLevel: {User.Chat.ChatLevel}\n" +
                    $"Total Messages: {User.Chat.MessagesSent}\n" +
                    $"Current Experience: {User.Chat.ChatExp}/{LevelManagement.GetExpAtLevel(User.Chat.ChatLevel)}";
            }

            TextLayer tl = GetTextLayer(overlayText, color);
            imf.Watermark(tl);//add text
            tl.Dispose();

            SolidBrush ExpBarColor = new SolidBrush(color);
            Graphics.FromImage(imf.Image).FillRectangle(ExpBarColor, GetExpBar(GetExpLength(User)));//add exp bar
            ExpBarColor.Dispose();
            imf.Format(new PngFormat());
            Image output = imf.Image;
            return output;
        }

        public static async Task<Image> GetBannerAsync(string ImageName = "default")
        {
            if (ImageName is null) ImageName = "default";

            WebClient wc = new WebClient();
            Uri imageUri = (await DownloadBanner(ImageName).ConfigureAwait(false)).PrimaryUri;
            byte[] bytes = wc.DownloadData(imageUri.OriginalString);
            MemoryStream ms = new MemoryStream(bytes);
            wc.Dispose();
            var img = Image.FromStream(ms);
            ms.Dispose();
            return img;
        }

        public static async Task<StorageUri> DownloadBanner(string imageName)
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(Properties.Resources.StorageToken);
            CloudBlobClient serviceClient = account.CreateCloudBlobClient();
            var container = serviceClient.GetContainerReference("images");
            await container.CreateIfNotExistsAsync().ConfigureAwait(false);
            CloudBlockBlob blob = container.GetBlockBlobReference(PersistentData.BotName + "/" + imageName);

            return blob.StorageUri;
        }

        public static async Task<ImageLayer> GetUserAvatarAsync(SocketGuildUser user)
        {
            var Guild = GuildManager.GetGuild(user.Guild.Id);
            var User = Guild.GetUser(user);
            if (User.Banner.ProfileImage != null && !User.Banner.ProfileImage.Equals("default"))
            {
                WebClient wc = new WebClient();
                Uri ImageUri = (await DownloadBanner(User.Banner.ProfileImage)).PrimaryUri;
                byte[] byteS = wc.DownloadData(ImageUri.OriginalString);
                MemoryStream MS = new MemoryStream(byteS);
                wc.Dispose();
                ImageLayer IL = new ImageLayer
                {
                    Image = System.Drawing.Image.FromStream(MS),
                    Position = new Point(12, 12),
                    Size = new Size(96, 96)
                };
                MS.Dispose();
                return IL;
            }

            byte[] bytes = new byte[1];
            using (WebClient client = new WebClient())
            {
                string url = user.GetAvatarUrl();
                if (user.AvatarId == null)
                    url = user.GetDefaultAvatarUrl();
                url = url.Substring(0, url.IndexOf("png") + 3);
                try
                {
                    bytes = client.DownloadData(url);
                }
                catch (WebException e)
                {
                    Console.WriteLine(e.Message);
                    url = user.GetDefaultAvatarUrl();
                    bytes = client.DownloadData(url);
                }
            }

            MemoryStream ms = new MemoryStream(bytes);
            ImageLayer il = new ImageLayer
            {
                Image = System.Drawing.Image.FromStream(ms),
                Position = new Point(12, 12),
                Size = new Size(96, 96)
            };
            ms.Dispose();
            return il;
        }

        public static Rectangle GetTextBackground()
        {
            Rectangle bg = new Rectangle
            {
                Height = 100,
                Width = 300,
                X = 120,
                Y = 10
            };

            return bg;
        }

        public static TextLayer GetTextLayer(string text, System.Drawing.Color color)
        {
            TextLayer tl = new TextLayer();
            tl.FontColor = color;
            tl.FontFamily = FontFamily.GenericSansSerif;
            tl.FontSize = 20;
            tl.Text = text;
            tl.Position = new Point(128, 12);

            return tl;
        }

        public static Rectangle GetExpBar(int length)
        {
            Rectangle bar = new Rectangle();
            bar.Height = 10;
            bar.Width = length;
            bar.X = 34;
            bar.Y = 113;

            return bar;
        }

        public static int GetExpLength(GuildUser User)
        {
            float maxW = 450;
            int userLevel = User.Chat.ChatLevel;
            if (userLevel == 0)
                userLevel++;
            float exp = User.Chat.ChatExp - LevelManagement.GetExpAtLevel(userLevel - 1);
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

        public static async Task<Image> Testing(SocketGuildUser User)
        {
            var image = GetBannerAsync("testbackground2").Result;
            var imf = new ImageFactory();
            imf.Load(image);


            if ((float)image.Height / (float)image.Width > 1f)
                imf.Rotate(270);

            imf.Flip(true);


            var res = new ResizeLayer(TotalSize)
            {
                ResizeMode = ResizeMode.Stretch
            };
            var rec = new Rectangle(Buffer, InnerSize);

            imf.Resize(res);
            SolidBrush RecColor = new SolidBrush(Color.FromArgb(GreyAlpha, 150, 150, 150));
            imf.Format(new PngFormat());
            Graphics.FromImage(imf.Image).FillRectangle(RecColor, rec);
            imf.Overlay(await GetTestAvatarAsync(User));

            RecColor.Dispose();
            Image output = imf.Image;
            return output;
        }

        public static async Task<ImageLayer> GetTestAvatarAsync(SocketGuildUser user)
        {
            var Guild = GuildManager.GetGuild(user.Guild.Id);
            byte[] bytes = new byte[1];
            using (WebClient client = new WebClient())
            {
                string url = user.GetAvatarUrl();
                if (user.AvatarId == null)
                    url = user.GetDefaultAvatarUrl();
                url = url.Substring(0, url.IndexOf("png") + 3);
                try
                {
                    bytes = client.DownloadData(url);
                }
                catch (WebException e)
                {
                    Console.WriteLine(e.Message);
                    url = user.GetDefaultAvatarUrl();
                    bytes = client.DownloadData(url);
                }
            }

            MemoryStream ms = new MemoryStream(bytes);
            var imf = new ImageFactory();
            imf.Load(Image.FromStream(ms));
            imf.RoundedCorners(AvatarSize.Height);
            imf.Format(new PngFormat());

            ImageLayer il = new ImageLayer
            {
                Image = imf.Image,
                Position = AvatarLocation,
                Size = AvatarSize
            };

            ms.Dispose();
            return il;
        }

        public static TextLayer RankAndLevel()
        {
            var RankLabel = new TextLayer();
            RankLabel.RightToLeft = false;
            RankLabel.Text = "Rank";
            RankLabel.FontSize = 20;
            RankLabel.FontColor = Color.White;
            return RankLabel;

        }
    }
}