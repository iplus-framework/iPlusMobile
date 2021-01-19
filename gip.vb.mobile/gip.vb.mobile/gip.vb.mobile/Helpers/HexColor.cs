using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace gip.vb.mobile.Helpers
{
    public struct HexColor
    {
        // Regex: This pattern matches hex codes in these two formats:
        // #000000 (no alpha value) and #FF000000 (alpha value at front).
        const string HEX_PATTERN = @"^\#([a-fA-F0-9]{6}|[a-fA-F0-9]{8})$";

        const int LENGTH_WITH_ALPHA = 8;

        Color _color;

        public HexColor(string hexCode)
        {
            if (hexCode == null)
            {
                throw new ArgumentNullException("hexCode");
            }

            if (!Regex.IsMatch(hexCode, HEX_PATTERN))
            {
                throw new ArgumentException(
                    "Format must be #000000 or #FF000000 (no extra whitespace)",
                    "hexCode");
            }

            // shave off '#' symbol
            hexCode = hexCode.TrimStart('#');

            // if no alpha value specified, assume no transparency (0xFF)
            if (hexCode.Length != LENGTH_WITH_ALPHA)
                hexCode = String.Format("FF{0}", hexCode);

            int colorA = byte.Parse(hexCode.Substring(0, 2), NumberStyles.AllowHexSpecifier);
            if (colorA < 50)
                colorA = 50;
            int colorR = byte.Parse(hexCode.Substring(2, 2), NumberStyles.AllowHexSpecifier);
            int colorG = byte.Parse(hexCode.Substring(4, 2), NumberStyles.AllowHexSpecifier);
            int colorB = byte.Parse(hexCode.Substring(6, 2), NumberStyles.AllowHexSpecifier);
            _color = Color.FromRgba(colorR, colorG, colorB, colorA);
        }

        public byte A
        {
            get { return ConvertToRGBByte(_color.A); }
        }

        public byte R
        {
            get { return ConvertToRGBByte(_color.R); }
        }

        public byte G
        {
            get { return ConvertToRGBByte(_color.G); }
        }

        public byte B
        {
            get { return ConvertToRGBByte(_color.B); }
        }

        private byte ConvertToRGBByte(double value)
        {
            if (value < 0.000000001)
                return 0;
            else if (value > 0.9999999999)
                return 255;
            return Convert.ToByte(Math.Round(value * 255));
        }

        // Implicit cast from HexColor to Color
        public static implicit operator Color(HexColor hexColor)
        {
            return hexColor._color;
        }

        // Implicit cast from Color to HexColor
        public static implicit operator HexColor(Color color)
        {
            HexColor c = new HexColor();
            c._color = color;
            return c;
        }

        // Just like with Color, ToString() prints out the hex value of the
        // color in #ARGB format (example: #FF000000) by default.
        public override string ToString()
        {
            return ToString(true);
        }

        // I don't always need the alpha value, so I added an overload here
        // that lets me return the hex value in #RBG format (example: #000000).
        public string ToString(bool includeAlpha)
        {
            if (includeAlpha)
            {
                return String.Format("#{0}{1}{2}{3}",
                    A.ToString("X2"),
                    R.ToString("X2"),
                    G.ToString("X2"),
                    B.ToString("X2"));
            }
            else
            {
                return String.Format("#{0}{1}{2}",
                    R.ToString("X2"),
                    G.ToString("X2"),
                    B.ToString("X2"));
            }
        }
    }

    public struct HSV
    {
        public float Hue;
        public float Saturation;
        public float Value;

        public HSV(float hue, float saturation, float value)
        {
            Hue = hue;
            Saturation = saturation;
            Value = value;
        }
    }
}
