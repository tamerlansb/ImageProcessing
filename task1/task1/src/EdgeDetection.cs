using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageReadCS
{
	class EdgeDetection
	{
		#region canny edge detector
		public static void GradientSobel(ref GrayscaleFloatImage image)
		{
			GrayscaleFloatImage tempImg = new GrayscaleFloatImage(image.Width, image.Height);
			for (int y = 0; y < image.Height; y++)
				for (int x = 0; x < image.Width; x++)
				{
					float dx = SobelX(image, x, y),
						dy = SobelY(image, x, y);
					tempImg[x, y] = (float)Math.Sqrt(dx * dx + dy * dy);
					//tempImg[x, y] = Math.Abs(dx) + Math.Abs(dy);
				}
			image = tempImg;
		}
		private static GrayscaleFloatImage ToGrayscaleImage(ColorFloatImage image)
		{
			GrayscaleFloatImage tmp = new GrayscaleFloatImage(image.Width, image.Height);
			for (int y = 0; y < image.Height; y++)
				for (int x = 0; x < image.Width; x++)
				{
					tmp[x, y] = image[x, y].b * 0.114f + image[x, y].g * 0.587f + image[x, y].r * 0.299f;
				}
			return tmp;
		}
		private static float SobelY(GrayscaleFloatImage image, int x, int y)
		{
			int w = image.Width - 1, h = image.Height - 1;
			return
				(image[x - 1 < 0 ? 0 : x - 1, y + 1 > h ? h : y + 1] - image[x - 1 < 0 ? 0 : x - 1, y - 1 < 0 ? 0 : y - 1] +
				2 * image[x, y + 1 > h ? h : y + 1] - 2 * image[x, y - 1 < 0 ? 0 : y - 1] +
				image[x + 1 > w ? w : x + 1, y + 1 > h ? h : y + 1] - image[x + 1 > w ? w : x + 1, y - 1 < 0 ? 0 : y - 1]) / 2;
		}
		private static float SobelX(GrayscaleFloatImage image, int x, int y)
		{
			int w = image.Width - 1, h = image.Height - 1;
			return
				(image[x + 1 > w ? w : x + 1, y - 1 < 0 ? 0 : y - 1] - image[x - 1 < 0 ? 0 : x - 1, y - 1 < 0 ? 0 : y - 1] +
						 2 * image[x + 1 > w ? w : x + 1, y] - 2 * image[x - 1 < 0 ? 0 : x - 1, y] +
				image[x + 1 > w ? w : x + 1, y + 1 > h ? h : y + 1] - image[x - 1 < 0 ? 0 : x - 1, y + 1 > h ? h : y + 1]) / 2;
		}
		public static void Susperss_NonMaxima(ref GrayscaleFloatImage Mag, GrayscaleFloatImage Dir)
		{
			GrayscaleFloatImage tmp = new GrayscaleFloatImage(Mag.Width, Mag.Height);
			for (int x = 0; x < Mag.Width; x++)
				for (int y = 0; y < Mag.Height; y++)
				{
					float dx = SobelX(Dir, x, y),
						dy = SobelY(Dir, x, y),
						v1 = 0,
						v2 = 0;
					int x_plus1 = x + 1 > Mag.Width - 1 ? Mag.Width - 1 : x + 1,
						x_minus1 = x - 1 < 0 ? 0 : x - 1,
						y_plus1 = y + 1 > Mag.Height - 1 ? Mag.Height - 1 : y + 1,
						y_minus1 = y - 1 < 0 ? 0 : y - 1;
					if (Math.Abs(dx) < Math.Abs(dy))
					{
						float th = dx / dy;
						if (th < 0)
						{
							v1 = (1 + th) * Mag[x, y_minus1] - th * Mag[x_plus1, y_minus1];
							v2 = (1 + th) * Mag[x, y_plus1] - th * Mag[x_minus1, y_plus1];
						}
						else
						{
							v1 = (1 - th) * Mag[x, y_minus1] + th * Mag[x_plus1, y_minus1];
							v2 = (1 - th) * Mag[x, y_plus1] + th * Mag[x_minus1, y_plus1];
						}

					}
					else
					{
						float th = dy / dx;
						if (th < 0)
						{
							v1 = (1 + th) * Mag[x_minus1, y] - th * Mag[x_minus1, y_plus1];
							v2 = (1 + th) * Mag[x_plus1, y] - th * Mag[x_plus1, y_minus1];
						}
						else
						{
							v1 = (1 + th) * Mag[x_minus1, y] - th * Mag[x_minus1, y_plus1];
							v2 = (1 + th) * Mag[x_plus1, y] - th * Mag[x_plus1, y_minus1];
						}
					}
					if (Mag[x, y] > v1 && Mag[x, y] > v2)
					{
						tmp[x, y] = Mag[x, y];
					}
					else tmp[x, y] = 0;
				}
			Mag = tmp;
		}

		static public GrayscaleFloatImage Canny(ColorFloatImage image, float sigma, float thigh, float tlow)
		{
			thigh *= 255;
			tlow *= 255;
			GrayscaleFloatImage EdgeMap = new GrayscaleFloatImage(image.Width, image.Height);
			image = ImageProcessing.Gauss(image, sigma);
			GrayscaleFloatImage GradientMap = ToGrayscaleImage(image);
			GrayscaleFloatImage Dir = ToGrayscaleImage(image);
			GradientSobel(ref GradientMap);
			Susperss_NonMaxima(ref GradientMap, Dir);
			EdgeDetect(GradientMap, thigh, tlow, EdgeMap);
			// Result = Mag;
			return EdgeMap;
		}

		static GrayscaleFloatImage VisitedMap;
		static private void EdgeDetect(GrayscaleFloatImage Mag, float thigh, float tlow, GrayscaleFloatImage Result)
		{
			VisitedMap = new GrayscaleFloatImage(Mag.Width, Mag.Height);
			for (int x = 0; x < Mag.Width; x++)
				for (int y = 0; y < Mag.Height; y++)
				{
					VisitedMap[x, y] = 0;
				}
			for (int x = 0; x < Mag.Width; x++)
				for (int y = 0; y < Mag.Height; y++)
				{
					if (Mag[x, y] >= thigh)
					{
						Result[x, y] = 255;
						FollowEdge(x, y, Mag, thigh, tlow, ref Result);
					}
					else if (Mag[x, y] < tlow)
						Result[x, y] = 0;
				}
		}
		/*
        private static void FollowEdge(int x, int y, GrayscaleFloatImage Mag, float thigh, float tlow, ref GrayscaleFloatImage Result)
        {
            if (VisitedMap[x, y] == 1) return;
            VisitedMap[x, y] = 1;
            int x_plus1 = x + 1 > Mag.Width - 1 ? Mag.Width - 1 : x + 1,
                x_minus1 = x - 1 < 0 ? 0 : x - 1,
                y_plus1 = y + 1 > Mag.Height - 1 ? Mag.Height - 1 : y + 1,
                y_minus1 = y - 1 < 0 ? 0 : y - 1;
            int w = Mag.Width - 1, h = Mag.Height - 1;
            for (int i = x_minus1; i <= x_plus1; ++i)
                for (int j = y_minus1; j <= y_plus1; ++j)
                {
                    if ((i != x || j != y) && Mag[x, y] >= tlow && VisitedMap[x,y]!= 1)
                    {
                        Result[x, y] = 255;
                        Mag[i, j] = Mag[x, y];
                        FollowEdge(i, j, Mag, thigh, tlow, ref Result);
                    }
                }
        }*/
		private static void FollowEdge(int x, int y, GrayscaleFloatImage Mag, float thigh, float tlow, ref GrayscaleFloatImage Result)
		{
			if (VisitedMap[x, y] == 1) return;
			VisitedMap[x, y] = 1;
			int x_plus1 = x + 1 > Mag.Width - 1 ? Mag.Width - 1 : x + 1,
				x_minus1 = x - 1 < 0 ? 0 : x - 1,
				y_plus1 = y + 1 > Mag.Height - 1 ? Mag.Height - 1 : y + 1,
				y_minus1 = y - 1 < 0 ? 0 : y - 1;
			int w = Mag.Width - 1, h = Mag.Height - 1;
			// 1
			if (Mag[x - 1 < 0 ? 0 : x - 1, y - 1 < 0 ? 0 : y - 1] >= tlow)
			{
				if (x - 1 >= 0 && y - 1 >= 0)
				{
					Mag[x - 1, y - 1] = Mag[x, y];
					Result[x - 1, y - 1] = 255;
					FollowEdge(x - 1, y - 1, Mag, thigh, tlow, ref Result);
				}
			}
			// 2
			if (Mag[x, y - 1 < 0 ? 0 : y - 1] >= tlow)
			{
				if (y - 1 >= 0)
				{
					Mag[x, y - 1] = Mag[x, y];
					Result[x, y - 1] = 255;
					FollowEdge(x, y - 1, Mag, thigh, tlow, ref Result);
				}
			}
			// 3
			if (Mag[x - 1 < 0 ? 0 : x - 1, y] >= tlow)
			{
				if (x - 1 >= 0)
				{
					Mag[x - 1, y] = Mag[x, y];
					Result[x - 1, y] = 255;
					FollowEdge(x - 1, y, Mag, thigh, tlow, ref Result);
				}
			}
			// 4
			if (Mag[x, y + 1 > h ? h : y + 1] >= tlow)
			{
				if (y + 1 <= h)
				{
					Mag[x, y + 1] = Mag[x, y];
					Result[x, y + 1] = 255;
					FollowEdge(x, y + 1, Mag, thigh, tlow, ref Result);
				}
			}
			// 5
			if (Mag[x + 1 > w ? w : x + 1, y] >= tlow)
			{
				if (x + 1 <= w)
				{
					Mag[x + 1, y] = Mag[x, y];
					Result[x + 1, y] = 255;
					FollowEdge(x + 1, y, Mag, thigh, tlow, ref Result);
				}
			}
			// 6
			if (Mag[x - 1 < 0 ? 0 : x - 1, y + 1 > h ? h : y + 1] >= tlow)
			{
				if (x - 1 >= 0 && y + 1 <= h)
				{
					Mag[x - 1, y + 1] = Mag[x, y];
					Result[x - 1, y + 1] = 255;
					FollowEdge(x - 1, y + 1, Mag, thigh, tlow, ref Result);
				}
			}
			// 7
			if (Mag[x + 1 > w ? w : x + 1, y - 1 < 0 ? 0 : y - 1] >= tlow)
			{
				if (x + 1 <= w && y - 1 >= 0)
				{
					Mag[x + 1, y - 1] = Mag[x, y];
					Result[x + 1, y - 1] = 255;
					FollowEdge(x + 1, y - 1, Mag, thigh, tlow, ref Result);
				}
			}
			// 8
			if (Mag[x + 1 > w ? w : x + 1, y + 1 > h ? h : y + 1] >= tlow)
			{
				if (x + 1 <= w && y + 1 <= h)
				{
					Mag[x + 1, y + 1] = Mag[x, y];
					Result[x + 1, y + 1] = 255;
					FollowEdge(x + 1, y + 1, Mag, thigh, tlow, ref Result);
				}
			}
		}

		#endregion

		#region harris detector
		private static float harrisConver(GrayscaleFloatImage image, int x, int y, float[,] matr, float normValue)
		{
			float Res = 0;
			int n = matr.GetLength(0);

			for (int i = 0; i < n; ++i)
				for (int j = 0; j < n; ++j)
				{
					Res += matr[i, j] * image[((x + i - (n - 1) / 2) < 0 || (x + i - (n - 1) / 2) >= image.Width ? x : (x + i - (n - 1) / 2)), ((y + j - (n - 1) / 2) < 0 || (y + j - (n - 1) / 2) >= image.Height ? y : (y + j - (n - 1) / 2))];
				}
			Res = Res / normValue;
			return Res;
		}
		public static GrayscaleFloatImage harris(GrayscaleFloatImage image, float sigma)
		{
			int n = (int)(6 * Math.Abs(sigma) + 1);
			n = n == 1 || n % 2 == 0 ? 3 : n;
			float[,] matrX = new float[n, n],
				matrY = new float[n, n],
				matrXY = new float[n, n];
			float normValueX = 0, normValueY = 0, normValueXY = 0;
			for (int i = 0; i < n; ++i)
				for (int j = 0; j < n; ++j)
				{
					matrXY[i, j] = ((i - (n - 1) / 2) * (j - (n - 1) / 2) / (sigma * sigma * sigma * sigma)) * (float)Math.Exp(-((i - (n - 1) / 2) * (i - (n - 1) / 2) + (j - (n - 1) / 2) * (j - (n - 1) / 2)) / (2 * sigma * sigma));
					normValueXY += Math.Abs(matrXY[i, j]);
					matrY[i, j] = (-(j - (n - 1) / 2) / (sigma * sigma)) * (float)Math.Exp(-((i - (n - 1) / 2) * (i - (n - 1) / 2) + (j - (n - 1) / 2) * (j - (n - 1) / 2)) / (2 * sigma * sigma));
					normValueY += Math.Abs(matrY[i, j]);
					matrX[i, j] = (-(i - (n - 1) / 2) / (sigma * sigma)) * (float)Math.Exp(-((i - (n - 1) / 2) * (i - (n - 1) / 2) + (j - (n - 1) / 2) * (j - (n - 1) / 2)) / (2 * sigma * sigma));
					normValueX += Math.Abs(matrX[i, j]);
				}

			GrayscaleFloatImage Result = new GrayscaleFloatImage(image.Width, image.Height);
			for (int y = 0; y < image.Height; y++)
				for (int x = 0; x < image.Width; x++)
				{
					//Result[x, y] = image[x, y];
					float[,] A = new float[2, 2];
					A[0, 0] = harrisConver(image, x, y, matrX, normValueX);
					A[0, 0] = A[0, 0] * A[0, 0];
					A[1, 1] = harrisConver(image, x, y, matrY, normValueY);
					A[1, 1] = A[1, 1] * A[1, 1];
					A[0, 1] = harrisConver(image, x, y, matrXY, normValueXY);
					A[1, 0] = A[0, 1];
					float k = 0.05f;
					double detA = A[0, 0] * A[1, 1] - A[0, 1] * A[1, 0],
						trA = A[0, 0] + A[1, 1];
					double R = detA - k * trA * trA;
					if (R > 10000) Result[x, y] = 255;

					/*double lambda1 = (trA + Math.Sqrt(trA * trA - 4 * detA))/2,
                        lambda2 = (trA - Math.Sqrt(trA * trA - 4 * detA))/2;
                    if (lambda1 > 128 && lambda2 > 128)     Result[x, y] = 255;
   */
				}
			Susperss_NonMaxima(ref Result, image);
			for (int y = 0; y < image.Height; y++)
				for (int x = 0; x < image.Width; x++)
				{
					Result[x, y] += image[x, y];
				}
			return Result;
		}
		#endregion
	}
}
