using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageReadCS
{
	class ImageInterpolation
	{
		public static void up_bilinear(ref ColorFloatImage image, float s)
		{
			ColorFloatImage tempImg = new ColorFloatImage((int)(image.Width * s), image.Height);
			float n = (image.Width * s - 1) / ((float)image.Width - 1);
			for (int y = 0; y < image.Height; ++y)
			{
				tempImg[0, y] = image[0, y];
				tempImg[tempImg.Width - 1, y] = image[image.Width - 1, y];
				for (int x = 1; x < tempImg.Width - 1; ++x)
				{
					ColorFloatPixel p = new ColorFloatPixel();
					float x1 = (float)Math.Truncate(x / n),
						x2 = (int)(x / n) + 1;
					p.r = (x2 - (float)x / n) * image[(int)x1, y].r + ((float)x / n - x1) * image[(int)x2, y].r;
					p.g = (x2 - (float)x / n) * image[(int)x1, y].g + ((float)x / n - x1) * image[(int)x2, y].g;
					p.b = (x2 - (float)x / n) * image[(int)x1, y].b + ((float)x / n - x1) * image[(int)x2, y].b;
					tempImg[x, y] = p;
				}
			}
			image = tempImg;

			tempImg = new ColorFloatImage(image.Width, (int)(image.Height * s));
			n = (image.Height * s - 1) / ((float)image.Height - 1);
			for (int x = 0; x < image.Width; ++x)
			{
				tempImg[x, 0] = image[x, 0];
				tempImg[x, tempImg.Height - 1] = image[x, image.Height - 1];
				for (int y = 1; y < tempImg.Height - 1; ++y)
				{
					ColorFloatPixel p = new ColorFloatPixel();
					float y1 = (float)Math.Truncate(y / n),
						 y2 = (int)(y / n) + 1;
					p.r = (y2 - (float)y / n) * image[x, (int)y1].r + ((float)y / n - y1) * image[x, (int)y2].r;
					p.g = (y2 - (float)y / n) * image[x, (int)y1].g + ((float)y / n - y1) * image[x, (int)y2].g;
					p.b = (y2 - (float)y / n) * image[x, (int)y1].b + ((float)y / n - y1) * image[x, (int)y2].b;
					tempImg[x, y] = p;
				}
			}
			image = tempImg;
		}
		public static void downsample(ref ColorFloatImage image, float s, string paramoff = "")
		{
			ColorFloatImage tempImg = new ColorFloatImage((int)(image.Width / s), image.Height);
			if (paramoff != "-off")
				image = ImageProcessing.Gauss(image, (float)Math.Sqrt(Math.Abs(s * s - 1)));
			float n = (image.Width * s - s) / ((float)image.Width - s);
			for (int y = 0; y < image.Height; ++y)
			{
				tempImg[0, y] = image[0, y];
				tempImg[tempImg.Width - 1, y] = image[image.Width - 1, y];
				for (int x = 1; x < tempImg.Width - 1; ++x)
				{
					ColorFloatPixel p = new ColorFloatPixel();
					float x1 = (float)Math.Truncate(x * n),
						x2 = (int)(x * n) + 1;
					p.r = (x2 - (float)x * n) * image[(int)x1, y].r + ((float)x * n - x1) * image[(int)x2, y].r;
					p.g = (x2 - (float)x * n) * image[(int)x1, y].g + ((float)x * n - x1) * image[(int)x2, y].g;
					p.b = (x2 - (float)x * n) * image[(int)x1, y].b + ((float)x * n - x1) * image[(int)x2, y].b;
					tempImg[x, y] = p;
				}
			}
			image = tempImg;

			tempImg = new ColorFloatImage(image.Width, (int)(image.Height / s));
			n = (image.Height * s - s) / ((float)image.Height - s);
			for (int x = 0; x < image.Width; ++x)
			{
				tempImg[x, 0] = image[x, 0];
				tempImg[x, tempImg.Height - 1] = image[x, image.Height - 1];
				for (int y = 1; y < tempImg.Height - 1; ++y)
				{
					ColorFloatPixel p = new ColorFloatPixel();
					float y1 = (float)Math.Truncate(y * n) > image.Height - 1 ? image.Height - 1 : (float)Math.Truncate(y * n),
						 y2 = (int)(y * n) + 1 > image.Height - 1 ? image.Height - 1 : (int)(y * n) + 1;
					p.r = (y2 - (float)y * n) * image[x, (int)y1].r + ((float)y * n - y1) * image[x, (int)y2].r;
					p.g = (y2 - (float)y * n) * image[x, (int)y1].g + ((float)y * n - y1) * image[x, (int)y2].g;
					p.b = (y2 - (float)y * n) * image[x, (int)y1].b + ((float)y * n - y1) * image[x, (int)y2].b;
					tempImg[x, y] = p;
				}
			}
			image = tempImg;
		}

		public static void up_bicubic(ref ColorFloatImage image, float s)
		{
			ColorFloatImage tempImg = new ColorFloatImage((int)(image.Width * s), (int)(image.Height * s));
			float nx = (image.Width * s - 1) / ((float)image.Width - 1),
				ny = (image.Height * s - 1) / ((float)image.Height - 1);
			for (int y = 0; y < tempImg.Height; ++y)
				for (int x = 0; x < tempImg.Width; ++x)
				{
					float X = ((float)x / nx) - (float)Math.Truncate((float)x / nx),
						Y = ((float)y / ny) - (float)Math.Truncate((float)y / ny);
					float
						b1 = (X - 1) * (X - 2) * (X + 1) * (Y - 1) * (Y - 2) * (Y + 1) / 4,
						b2 = -X * (X + 1) * (X - 2) * (Y - 1) * (Y - 2) * (Y + 1) / 4,
						b3 = -Y * (X - 1) * (X - 2) * (X + 1) * (Y + 1) * (Y - 2) / 4,
						b4 = X * Y * (X + 1) * (X - 2) * (Y + 1) * (Y - 2) / 4,
						b5 = -X * (X - 1) * (X - 2) * (Y - 1) * (Y - 2) * (Y + 1) / 12,
						b6 = -Y * (X - 1) * (X - 2) * (X + 1) * (Y - 1) * (Y - 2) / 12,
						b7 = X * Y * (X - 1) * (X - 2) * (Y + 1) * (Y - 2) / 12,
						b8 = X * Y * (X + 1) * (X - 2) * (Y - 1) * (Y - 2) / 12,
						b9 = X * (X - 1) * (X + 1) * (Y - 1) * (Y - 2) * (Y + 1) / 12,
						b10 = Y * (X - 1) * (X - 2) * (X + 1) * (Y - 1) * (Y + 1) / 12,
						b11 = X * Y * (X - 1) * (X - 2) * (Y - 1) * (Y - 2) / 36,
						b12 = -X * Y * (X - 1) * (X + 1) * (Y + 1) * (Y - 2) / 12,
						b13 = -X * Y * (X + 1) * (X - 2) * (Y - 1) * (Y + 1) / 12,
						b14 = -X * Y * (X - 1) * (X + 1) * (Y - 1) * (Y - 2) / 36,
						b15 = -X * Y * (X - 1) * (X - 2) * (Y - 1) * (Y + 1) / 36,
						b16 = -X * Y * (X - 1) * (X + 1) * (Y - 1) * (Y + 1) / 36;
					int x0 = (int)Math.Truncate((float)x / nx),
						y0 = (int)Math.Truncate((float)y / ny),
						x_plus1 = x0 + 1 > image.Width - 1 ? image.Width - 1 : x0 + 1,
						x_plus2 = x0 + 2 > image.Width - 1 ? image.Width - 1 : x0 + 2,
						y_plus1 = y0 + 1 > image.Height - 1 ? image.Height - 1 : y0 + 1,
						y_plus2 = y0 + 2 > image.Height - 1 ? image.Height - 1 : y0 + 2,
						y_minus1 = y0 - 1 < 0 ? 0 : y0 - 1,
						x_minus1 = x0 - 1 < 0 ? 0 : x0 - 1;

					tempImg[x, y] = b1 * image[x0, y0] + b3 * image[x0, y_plus1] + b2 * image[x_plus1, y0] + b4 * image[x_plus1, y_plus1] +
						b6 * image[x0, y_minus1] + b5 * image[x_minus1, y0] + b8 * image[x_plus1, y_minus1] + b7 * image[x_minus1, y_plus1] +
						b10 * image[x0, y_plus2] + b9 * image[x_plus2, y0] + b11 * image[x_minus1, y_minus1] + b13 * image[x_plus1, y_plus2] +
						b12 * image[x_plus2, y_plus1] + b15 * image[x_minus1, y_plus2] + b14 * image[x_plus2, y_minus1] + b16 * image[x_plus2, y_plus2];
				}

			image = tempImg;
		}
		public static float MSE(ColorFloatImage image1, ColorFloatImage image2)
		{
			float mse = 0;
			for (int y = 0; y < image1.Height; ++y)
			{
				for (int x = 0; x < image1.Width; ++x)
				{
					mse += ((image1[x, y].r - image2[x, y].r) * (image1[x, y].r - image2[x, y].r) +
							(image1[x, y].g - image2[x, y].g) * (image1[x, y].g - image2[x, y].g) +
							(image1[x, y].b - image2[x, y].b) * (image1[x, y].b - image2[x, y].b));
				}
			}
			mse /= (3 * (float)image1.Height * image1.Width);
			return mse;
		}
		public static float PSNR(ColorFloatImage image1, ColorFloatImage image2)
		{
			float max = 255;
			return 20 * (float)Math.Log10(max / Math.Sqrt(MSE(image1, image2)));

		}

		public static float SSIM(ColorFloatImage image1, ColorFloatImage image2)
		{
			float ssim = 0;
			float X = 0, sigmaX2 = 0, Y = 0, sigmaY2 = 0, sigmaXY = 0;
			for (int y = 0; y < image1.Height; ++y)
			{
				for (int x = 0; x < image1.Width; ++x)
				{
					X += image1[x, y].r + image1[x, y].g + image1[x, y].b;
					Y += image2[x, y].r + image2[x, y].g + image2[x, y].b;
				}
			}
			X /= (3 * (float)image1.Height * image1.Width);
			Y /= (3 * (float)image2.Height * image2.Width);
			for (int y = 0; y < image2.Height; ++y)
			{
				for (int x = 0; x < image2.Width; ++x)
				{
					sigmaX2 += (((image1[x, y].r + image1[x, y].g + image1[x, y].b) / 3 - X) * ((image1[x, y].r + image1[x, y].g + image1[x, y].b) / 3 - X));
					sigmaY2 += (((image2[x, y].r + image2[x, y].g + image2[x, y].b) / 3 - Y) * ((image2[x, y].r + image2[x, y].g + image2[x, y].b) / 3 - Y));
					sigmaXY += (((image1[x, y].r + image1[x, y].g + image1[x, y].b) / 3 - X) * ((image2[x, y].r + image2[x, y].g + image2[x, y].b) / 3 - Y));
				}
			}
			sigmaX2 /= ((float)image1.Height * image1.Width);
			sigmaY2 /= ((float)image1.Height * image1.Width);
			sigmaXY /= ((float)image1.Height * image1.Width);

			float C1 = (0.01f * 255) * (0.01f * 255), C2 = (0.03f * 255) * (0.03f * 255);
			ssim = ((2 * X * Y + C1) * (2 * sigmaXY + C2)) / ((X * X + Y * Y + C1) * (sigmaX2 + sigmaY2 + C2));

			return ssim;
		}
		private static ColorFloatImage Generate8X8(ColorFloatImage image, int fromX, int fromY)
		{
			ColorFloatImage G = new ColorFloatImage(8, 8);
			for (int y = fromY; y < fromY + 8; ++y)
			{
				for (int x = fromX; x < fromX + 8; ++x)
				{
					G[x - fromX, y - fromY] = image[x, y];
				}
			}
			return G;
		}
		public static float MSSIM(ColorFloatImage image1, ColorFloatImage image2)
		{
			float mssim = 0;
			int P = 0;
			for (int y = 0; y < image1.Height - 8; ++y)
			{
				for (int x = 0; x < image1.Width - 8; ++x)
				{
					++P;
					mssim += SSIM(Generate8X8(image1, x, y), Generate8X8(image2, x, y));
				}
			}
			mssim = mssim / P;
			return mssim;
		}

	}
}
