using Discord.WebSocket;
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
        public static async Task<Image> BuildBannerAsync(SocketGuildUser user, bool levelUp)
        {
            var color = ChatUsers.GetUserColor(user);

            Image image = await GetBannerAsync(ChatUsers.RetrieveOrCreateChatUser(user).BannerImage);
            ImageFactory imf = new ImageFactory();//get banner
            imf.Load(image);
            image.Dispose();

            ImageLayer iL = await GetUserAvatarAsync(user);
            imf.Overlay(iL);//add avatar
            iL.Dispose();

            if (ChatUsers.RetrieveOrCreateChatUser(user).TextBackground)
            {
                int greyscale = ChatUsers.RetrieveOrCreateChatUser(user).Greyscale;
                SolidBrush bgColor = new SolidBrush(System.Drawing.Color.FromArgb(215, greyscale, greyscale, greyscale));
                Graphics.FromImage(imf.Image).FillRectangle(bgColor, GetTextBackground());//add background for text
                bgColor.Dispose();
            }

            var UserInfo = ChatUsers.RetrieveOrCreateChatUser(user);
            string overlayText = user.Username;//build display string
            if (user.Nickname != null)
                overlayText = user.Nickname;

            if (levelUp)
            {
                overlayText += $" has just advanced to level {UserInfo.ChatLevel}. Congratulations!\n" +
                    $"Total Messages: {UserInfo.MessagesSent}\n" +
                    $"Current Experience: {UserInfo.ChatExp}/{LevelManagement.GetExpAtLevel(UserInfo.ChatLevel)}";
            }
            else
            {
                overlayText += $"\nLevel: {UserInfo.ChatLevel}\n" +
                    $"Total Messages: {UserInfo.MessagesSent}\n" +
                    $"Current Experience: {UserInfo.ChatExp}/{LevelManagement.GetExpAtLevel(UserInfo.ChatLevel)}";
            }

            TextLayer tl = GetTextLayer(overlayText, color);
            imf.Watermark(tl);//add text
            tl.Dispose();

            SolidBrush ExpBarColor = new SolidBrush(color);
            Graphics.FromImage(imf.Image).FillRectangle(ExpBarColor, GetExpBar(GetExpLength(UserInfo)));//add exp bar
            ExpBarColor.Dispose();
            imf.Format(new PngFormat());
            Image output = imf.Image;
            return output;
        }

        public static async Task<Image> GetBannerAsync(string ImageName)
        {
            string IName = "1";
            if (ImageName != null && !ImageName.Equals("default"))
                IName = ImageName;

            WebClient wc = new WebClient();
            Uri imageUri = (await DownloadBanner(IName).ConfigureAwait(false)).PrimaryUri;
            byte[] bytes = wc.DownloadData(imageUri.OriginalString);
            MemoryStream ms = new MemoryStream(bytes);
            wc.Dispose();
            var img = Image.FromStream(ms);
            ms.Dispose();
            return img;
        }

        public static async Task<StorageUri> DownloadBanner(string imageName)
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(Configuration.Configuration.storageConnectionString);
            CloudBlobClient serviceClient = account.CreateCloudBlobClient();
            var container = serviceClient.GetContainerReference("images");
            await container.CreateIfNotExistsAsync().ConfigureAwait(false);
            CloudBlockBlob blob = container.GetBlockBlobReference("faitie/" + imageName);

            return blob.StorageUri;
        }

        public static async Task<ImageLayer> GetUserAvatarAsync(SocketGuildUser user)
        {
            if (ChatUsers.RetrieveOrCreateChatUser(user).ProfileImage != null && !ChatUsers.RetrieveOrCreateChatUser(user).ProfileImage.Equals("default"))
            {
                WebClient wc = new WebClient();
                Uri ImageUri = (await DownloadBanner(ChatUsers.RetrieveOrCreateChatUser(user).ProfileImage)).PrimaryUri;
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

        public static int GetExpLength(ChatDataType User)
        {
            float maxW = 450;
            int userLevel = User.ChatLevel;
            if (userLevel == 0)
                userLevel++;
            float exp = User.ChatExp - LevelManagement.GetExpAtLevel(userLevel - 1);
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
    }
}