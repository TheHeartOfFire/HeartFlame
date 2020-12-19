using System.Drawing;

namespace HeartFlame.ChatLevels
{
    public class BannerData
    {
        private int[] ColorARGB { get; set; }
        public string ProfileImage { get; set; }
        public string BannerImage { get; set; }
        public bool TextBackground { get; set; }
        public int Greyscale { get; set; }

        private Color ParseColor(int[] input)
        {
            return Color.FromArgb(input[0], input[1], input[2], input[3]);
        }
        public Color GetColor()
        {
            return ParseColor(ColorARGB);
        }

        public void SetColor(Color color)
        {
            ColorARGB = new int[] { color.A, color.R, color.G, color.B };
        }
    }
}
