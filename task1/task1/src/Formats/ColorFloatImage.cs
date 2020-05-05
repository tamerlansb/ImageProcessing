using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ImageReadCS
{
    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct ColorFloatPixel
    {
        public float b, g, r, a;
		public static ColorFloatPixel operator +(ColorFloatPixel P1, ColorFloatPixel P2)
		{
			ColorFloatPixel tmp = P1;
			tmp.r += P2.r;
			tmp.g += P2.g;
			tmp.b += P2.b;
			return tmp;
		}
		public static ColorFloatPixel operator +(ColorFloatPixel P1, float P2)
		{
			ColorFloatPixel tmp = P1;
			tmp.r += P2;
			tmp.g += P2;
			tmp.b += P2;
			return tmp;
		}
		public static ColorFloatPixel operator +(float P1, ColorFloatPixel P2)
		{
			return P2 + P1;
		}
		public static ColorFloatPixel operator -(ColorFloatPixel P1, float P2)
		{
			return P1 + (-P2);
		}
		public static ColorFloatPixel operator -(float P1, ColorFloatPixel P2)
		{
			return P1 + P2*(-1);
		}
		public static ColorFloatPixel operator -(ColorFloatPixel P1, ColorFloatPixel P2)
		{
			ColorFloatPixel tmp = P1;
			tmp.r -= P2.r;
			tmp.g -= P2.g;
			tmp.b -= P2.b;
			return tmp;
		}
		public static ColorFloatPixel operator *(float n, ColorFloatPixel P)
		{
			ColorFloatPixel tmp = new ColorFloatPixel();
			tmp.a = P.a;
			tmp.r = P.r * n;
			tmp.g = P.g * n;
			tmp.b = P.b * n;
			return tmp;
		}
		public static ColorFloatPixel operator *(ColorFloatPixel P, float n)
		{
			ColorFloatPixel tmp = new ColorFloatPixel();
			tmp.a = P.a;
			tmp.r = P.r * n;
			tmp.g = P.g * n;
			tmp.b = P.b * n;
			return tmp;
		}

		public static ColorFloatPixel operator /(ColorFloatPixel P, float n)
		{
			ColorFloatPixel tmp = new ColorFloatPixel();
			tmp.a = P.a;
			tmp.r = P.r / n;
			tmp.g = P.g / n;
			tmp.b = P.b / n;
			return tmp;
		}

		public static ColorFloatPixel Pow(ColorFloatPixel P, float deg)
		{
			ColorFloatPixel tmp = new ColorFloatPixel();
			tmp.a = P.a;
			tmp.r = (float)Math.Pow(P.r, deg);
			tmp.g = (float)Math.Pow(P.g, deg);
			tmp.b = (float)Math.Pow(P.b, deg);
			return tmp;
		}
	}

    public class ColorFloatImage
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public readonly ColorFloatPixel[] rawdata;

        public ColorFloatImage(int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;
            rawdata = new ColorFloatPixel[Width * Height];
        }

        public ColorFloatPixel this[int x, int y]
        {
            get
            {
#if DEBUG
                if (x < 0 || x >= Width || y < 0 || y >= Height)
                    throw new IndexOutOfRangeException(string.Format("Trying to access pixel ({0}, {1}) in {2}x{3} image", x, y, Width, Height));
#endif
                return rawdata[y * Width + x];
            }
            set
            {
#if DEBUG
                if (x < 0 || x >= Width || y < 0 || y >= Height)
                    throw new IndexOutOfRangeException(string.Format("Trying to access pixel ({0}, {1}) in {2}x{3} image", x, y, Width, Height));
#endif
                rawdata[y * Width + x] = value;
            }
        }
    }
}
