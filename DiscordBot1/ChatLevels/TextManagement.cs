using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Linq;
using ImageProcessor.Imaging;

namespace HeartFlame.ChatLevels
{
    public static class TextManagement
    {
        public static readonly int Kerning = 1;
        public static readonly Size BigNormalCharacter = new Size(20, 29);
        public static readonly Size BigThinCharacter = new Size(4, 29);
        public static readonly Size BigMediumCharacter = new Size(11, 29);
        public static readonly Size BigWideCharacter = new Size(29, 29);
        public static readonly Size BigUpperNormalCharacter = new Size(22, 29);
        public static readonly Size BigUpperMediumCharacter = new Size(19, 29);
        public static readonly Size SmallNormalCharacter = new Size(10, 15);
        public static readonly Size SmallThinCharacter = new Size(3, 15);
        public static readonly Size SmallMediumCharacter = new Size(6, 15);
        public static readonly Size SmallWideCharacter = new Size(15, 15);
        public static readonly Size SmallUpperNormalCharacter = new Size(13, 15);
        public static readonly Size SmallUpperMediumCharacter = new Size(10, 15);
        public static readonly char[] NormalCharacters = new char[] {'2','3','4','5','6','7','8','9','0', 'a', 'b', 'c', 'd', 'e', 'g', 'h', 'k', 'n', 'o', 'p', 'q', 's', 'u', 'v', 'x', 'y', 'z' };
        public static readonly char[] MediumCharacters = new char[] {'f', 'r', 't', '1', '/' };
        public static readonly char[] ThinCharacters = new char[] { 'i', 'j', 'l', 'I' };
        public static readonly char[] WideCharacters = new char[] { 'm', 'w', 'M', 'W', 'O', 'Q', '#' };
        public static readonly char[] UpperNormalCharacters = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'K', 'N', 'P', 'R', 'S', 'T', 'U', 'V', 'X', 'Y', 'Z' };
        public static readonly char[] UpperMediumCharacters = new char[] { 'J', 'L' };

        public static Size GetCharacterSize(char Character, bool Large = false)
        {

            if (NormalCharacters.Contains(Character))
                if (Large)
                    return BigNormalCharacter;
                else
                    return SmallNormalCharacter;

            if (MediumCharacters.Contains(Character))
                if (Large)
                    return BigMediumCharacter;
                else
                    return SmallMediumCharacter;

            if (ThinCharacters.Contains(Character))
                if (Large)
                    return BigThinCharacter;
                else
                    return SmallThinCharacter;

            if (WideCharacters.Contains(Character))
                if (Large)
                    return BigWideCharacter;
                else
                    return SmallWideCharacter;

            if (UpperNormalCharacters.Contains(Character))
                if (Large)
                    return BigUpperNormalCharacter;
                else
                    return SmallUpperNormalCharacter;

            if (UpperMediumCharacters.Contains(Character))
                if (Large)
                    return BigUpperMediumCharacter;
                else
                    return SmallUpperMediumCharacter;

            return new Size();
        }

        public static Size GetSize(TextLayer Layer)
        {
            var Output = new Size();
            bool Large = false;
            if (Layer.FontSize == 20)
                Output.Height = 15;
            else if (Layer.FontSize == 40)
            {
                Output.Height = 29;
                Large = true;
            }
            else return Output;

            foreach(char Character in Layer.Text)
            {
                Output.Width += GetCharacterSize(Character, Large).Width + Kerning; 
            }
            return Output;
        }
    }
}
