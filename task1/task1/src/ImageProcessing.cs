using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageReadCS
{
	class ImageProcessing
	{
		public static ColorFloatImage unsharp(ColorFloatImage image)
		{
			var result = new ColorFloatImage(image.Width, image.Height);
			var blurred = Gauss(image, 1.5f);
			for (int i = 0; i < image.Width; ++i)
				for (int j = 0; j < image.Height; ++j)
				{
					result[i, j] = blurred[i, j] + (image[i, j] - blurred[i, j]);
				}

			return result;
		}
		#region rotate  
		public static void Rotate(ref ColorFloatImage image,string param ,float angle)
		{
			if (param == "ccw")
			{
				angle = -angle;
			}
			if (angle % 90 == 0)
			{
				for (int i = 0; i < Math.Abs(angle) / 90; ++i)
				{
					if (angle < 0)
					{
						RotateCw90(ref image);
						RotateCw90(ref image);
						RotateCw90(ref image);
					}
					else
					{
						RotateCw90(ref image);
					}
						
				}
			}
			else
			{
				rotate(ref image, angle);
			}
		}
		private static void rotate(ref ColorFloatImage image, float angle)
		{
			angle *= (float)Math.PI / 180.0f;
			double x1 = -image.Width / 2, y1 = -image.Height / 2,
				x2 = -image.Width / 2, y2 = image.Height / 2,
				x3 = image.Width / 2, y3 = -image.Height / 2,
				x4 = image.Width / 2, y4 = image.Height / 2;
			double tempX = x1, tempY = y1;
			x1 = tempX*Math.Cos(angle) - tempY*Math.Sin(angle);
			y1 = tempX*Math.Sin(angle) + tempY*Math.Cos(angle);

			tempX = x2; tempY = y2;
			x2 = tempX * Math.Cos(angle) - tempY * Math.Sin(angle);
			y2 = tempX * Math.Sin(angle) + tempY * Math.Cos(angle);

			tempX = x3; tempY = y3;
			x3 = tempX * Math.Cos(angle) - tempY * Math.Sin(angle);
			y3 = tempX * Math.Sin(angle) + tempY * Math.Cos(angle);

			tempX = x4; tempY = y4;
			x4 = tempX * Math.Cos(angle) - tempY * Math.Sin(angle);
			y4 = tempX * Math.Sin(angle) + tempY * Math.Cos(angle);

			int w = (int)Math.Floor(Math.Max(Math.Abs(x1 - x4), Math.Abs(x2 - x3))) + 1,
				h = (int)Math.Floor(Math.Max(Math.Abs(y1 - y4), Math.Abs(y2 - y3))) + 1;
			ColorFloatImage newIm = new ColorFloatImage(w,h);

			for (int i = 0; i < newIm.Width; ++i)
				for (int j = 0; j < newIm.Height; ++j)
				{
					double x = (i - newIm.Width / 2) * Math.Cos(angle) + (j - newIm.Height / 2) * Math.Sin(angle), 
						y = - (i - newIm.Width / 2) * Math.Sin(angle) + (j - newIm.Height / 2) * Math.Cos(angle);

					x += image.Width / 2.0;
					y += image.Height / 2.0;

					if (x < 0 || y < 0 || x > image.Width - 1 || y > image.Height - 1)
					{
						ColorFloatPixel p = new ColorFloatPixel();
						p.r = 0;
						p.g = 0;
						p.b = 0;
						newIm[i, j] = p;
					}
					else
					{
						x1 = (int)x;
						y1 = (int)y;
						x2 = x1 + 1;
						y2 = y1 + 1;

						newIm[i, j] = image[(int)x1, (int)y1] * (float)((y2 - y) * (x2 - x)) +
									  image[(int)x2, (int)y1] * (float)((y2 - y) * (x - x1)) +
								      image[(int)x1, (int)y2] * (float)((y - y1) * (x2 - x)) +
							          image[(int)x2, (int)y2] * (float)((y - y1) * (x - x1));
					}

				}
			image = newIm;
		}

		public static void RotateCw90(ref ColorFloatImage image)
		{
			ColorFloatImage tempImg = new ColorFloatImage(image.Height, image.Width);
			for (int y = 0; y < image.Height; y++)
				for (int x = 0; x < image.Width; x++)
				{
					tempImg[image.Height - 1 - y, image.Width - 1 - x] = image[x, y];
				}
			FlipImageX(tempImg);
			image = tempImg;
		}
		#endregion

		#region Gauss, Gabor and find vessels

		private static float Conv(GrayscaleFloatImage image, int x, int y, float[,] matr, float normValue)
		{
			float tempPixel = image[x, y];
			int n = matr.GetLength(0);

			for (int i = 0; i < n; ++i)
				for (int j = 0; j < n; ++j)
				{
					tempPixel += matr[j, i] * image[((x + i - (n - 1) / 2) < 0 || (x + i - (n - 1) / 2) >= image.Width ? x : (x + i - (n - 1) / 2)), ((y + j - (n - 1) / 2) < 0 || (y + j - (n - 1) / 2) >= image.Height ? y : (y + j - (n - 1) / 2))];
				}
			tempPixel = tempPixel / normValue;
			return tempPixel;
		}
		private static ColorFloatPixel Conv(ColorFloatImage image, int x, int y, float[,] matr, float normValue)
		{
			ColorFloatPixel tempPixel = image[x, y];
			tempPixel.r = 0;
			tempPixel.g = 0;
			tempPixel.b = 0;
			int n = matr.GetLength(0);

			for (int i = 0; i < n; ++i)
				for (int j = 0; j < n; ++j)
				{
					tempPixel += matr[j, i] * image[((x + i - (n - 1) / 2) < 0 || (x + i - (n - 1) / 2) >= image.Width ? x : (x + i - (n - 1) / 2)), ((y + j - (n - 1) / 2) < 0 || (y + j - (n - 1) / 2) >= image.Height ? y : (y + j - (n - 1) / 2))];
				}
			tempPixel = tempPixel / normValue;
			return tempPixel;
		}
		private static GrayscaleFloatImage add(GrayscaleFloatImage image, GrayscaleFloatImage image2)
		{
			for (int y = 0; y < image.Height; y++)
				for (int x = 0; x < image.Width; x++)
				{
					image[x, y] = Math.Abs(image[x, y]) > Math.Abs(image2[x, y]) ? image[x, y] : image2[x, y];
				}
			return image;
		}
		public static GrayscaleFloatImage vessels(ColorFloatImage ColorImage,float sigma)
		{
			var blurred = Gauss(ColorImage, 5.0f);
			var image = new GrayscaleFloatImage(ColorImage.Width, ColorImage.Height);
			for (int y = 0; y < ColorImage.Height; y++)
				for (int x = 0; x < ColorImage.Width; x++)
				{
					image[x, y] = (ColorImage[x, y]  - blurred[x,y]).g;
				}

			float lambda = 6, psi = 0, gamma = 1;

			GrayscaleFloatImage result = new GrayscaleFloatImage(image.Width, image.Height);
			for (int theta = 0; theta < 180; theta+=30)
			{
				GrayscaleFloatImage tempIm = gabor(image, sigma, gamma, theta, lambda, psi);
				result = add( result, tempIm);
			}

			float min = 1000, max = -1000;
			for (int y = 0; y < image.Height; y++)
				for (int x = 0; x < image.Width; x++)
				{
					if (result[x, y] > 0)
					{
						result[x, y] = 0;
					}
					else
					{
						result[x, y] = -result[x, y];
					}
					if (result[x, y] > max)
						max = result[x, y];
					if (result[x, y] < min)
						min = result[x, y];
				}
			for (int y = 0; y < image.Height; y++)
				for (int x = 0; x < image.Width; x++)
				{
					result[x, y] = (result[x, y] -min)*5 > (max-min)*5.0f/7.5? (result[x, y] - min) * 255/(max-min) :0 ;
				}
			return result;
		}
		public static GrayscaleFloatImage Gabor(GrayscaleFloatImage image, float sigma, float gamma, float theta, float lambda, float psi)
		{
			var tempImg = gabor(image, sigma, gamma, theta, lambda, psi);

			float max = -1000,
			min = 1000;
			for (int y = 0; y < image.Height; y++)
				for (int x = 0; x < image.Width; x++)
				{
					if (tempImg[x, y] > max)
						max = tempImg[x, y];
					if (tempImg[x, y] < min)
						min = tempImg[x, y];
				}
			for (int y = 0; y < image.Height; y++)
				for (int x = 0; x < image.Width; x++)
				{
					tempImg[x, y] = (tempImg[x, y] - min) * 255 / (max - min);
				}
			return tempImg;
		}
		private static GrayscaleFloatImage gabor(GrayscaleFloatImage image,float sigma,float gamma,float theta,float lambda, float psi)
        {
			GrayscaleFloatImage tempImg = new GrayscaleFloatImage(image.Width, image.Height);
            int n = (int)(6 * Math.Abs(sigma) + 1);
            n = n == 1 || n % 2 == 0 ? 3 : n;
            float[,] matr = new float[n, n];
            float normValue = 0;
			theta *= (float)Math.PI / 180.0f;
			for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                {
                    double x = (i - (n - 1) / 2) * Math.Cos(theta) + (j - (n - 1) / 2) * Math.Sin(theta);
                    double y = -(i - (n - 1) / 2) * Math.Sin(theta) + (j - (n - 1) / 2) * Math.Cos(theta);
                    matr[i, j] = (float)(Math.Exp(-(x*x + y*y*gamma*gamma)/(2*sigma*sigma)) *Math.Cos(2*x*Math.PI / lambda + psi));
					normValue += (float)(Math.Exp(-(x * x + y * y * gamma * gamma) / (2 * sigma * sigma))); //matr[i, j];
																																						 //normValue = 1;
				}

            for (int y = 0; y < image.Height; y++)
                for (int x = 0; x < image.Width; x++)
                {
                    tempImg[x, y] = Conv(image, x, y, matr, normValue);
				}
			return tempImg;
        } 

		public static void gradient(ref ColorFloatImage image, float sigma)
		{
			ColorFloatImage tempImg = new ColorFloatImage(image.Width, image.Height);
			int n = (int)(6 * Math.Abs(sigma) + 1);
			n = n == 1 || n % 2 == 0 ? 3 : n;
			float[,] matrX = new float[n, n],
				matrY = new float[n, n];
			float normValueX = 0, normValueY = 0;
			for (int i = 0; i < n; ++i)
				for (int j = 0; j < n; ++j)
				{
					matrX[i, j] = (-(i - (n - 1) / 2) / (sigma * sigma)) * (float)Math.Exp(-((i - (n - 1) / 2) * (i - (n - 1) / 2) + (j - (n - 1) / 2) * (j - (n - 1) / 2)) / (2 * sigma * sigma));
					normValueX += Math.Abs(matrX[i, j]);

					matrY[i, j] = (-(j - (n - 1) / 2) / (sigma * sigma)) * (float)Math.Exp(-((i - (n - 1) / 2) * (i - (n - 1) / 2) + (j - (n - 1) / 2) * (j - (n - 1) / 2)) / (2 * sigma * sigma));
					normValueY += Math.Abs(matrY[i, j]);
				}

			for (int y = 0; y < image.Height; y++)
				for (int x = 0; x < image.Width; x++)
				{
					tempImg[x, y] = ColorFloatPixel.Pow(ColorFloatPixel.Pow(Conv(image, x, y, matrX, normValueX),2.0f)  + ColorFloatPixel.Pow(Conv(image, x, y, matrY, normValueY), 2.0f), 0.5f);
					//tempImg[x, y] = (float)Math.Pow(Math.Pow(Conv(image, x, y, matrX, normValueX),2.0f)  + Math.Pow(Conv(image, x, y, matrY, normValueY), 2.0f), 0.5f);
				}

			image = tempImg;
		}

		public static ColorFloatImage Gauss(ColorFloatImage image, float sigma, float gamma = 1)
        {
            ColorFloatImage tempImg = new ColorFloatImage(image.Width, image.Height);
            int n = (int)(6 * Math.Abs(sigma) + 1);
            n = n == 1 || n % 2 == 0 ? 3 : n;
            float[,] matr = new float[n, n];
            float normValue = 0;
            for (int i = 0; i < n; ++i) 
                for (int j = 0; j < n; ++j)
                {
                    matr[i, j] = (float)Math.Exp(-((i - (n - 1) / 2) * (i - (n - 1) / 2) + (j - (n - 1) / 2) * (j - (n - 1) / 2) ) / (2 * sigma * sigma));
                    normValue += matr[i, j];
                }

			for (int y = 0; y < image.Height; y++)
				for (int x = 0; x < image.Width; x++)
				{
					image[x, y] = 255.0f * ColorFloatPixel.Pow(image[x,y] / 255.0f, gamma);
				}

			for (int y = 0; y < image.Height; y++)
                for (int x = 0; x < image.Width; x++)
                {
					tempImg[x, y] = 255.0f * ColorFloatPixel.Pow(Conv(image,x,y,matr,normValue)/ 255.0f, 1.0f /gamma);
				}

			return tempImg;
        }

		#endregion

		#region Robrets, Media, Sobel filters, and flips X or Y
		public static void Median(ColorFloatImage image, int rad)
        {
            float[] med = new float[(rad * 2 + 1) * (rad * 2 + 1)];
            ColorFloatImage tempImg = new ColorFloatImage(image.Width, image.Height);
            int index = 0;
            for (int y = 0; y < image.Height; y++)
                for (int x = 0; x < image.Width; x++)
                {
                    ColorFloatPixel tempPixel = image[x, y];
                    index = 0;
                    for (int i = -rad; i <= rad; i++)
                        for (int j = -rad; j <= rad; j++)
                        {
                           med[index] = image[((x + i) < 0 || (x + i) >= image.Width ? x : (x + i)), ((y + j) < 0 || (y + j) >= image.Height ? y : (y + j))].g;
                            index++;
                        }
                    Array.Sort(med);
                    tempPixel.g = med[2 * rad * rad + 2 * rad + 1];

                    index = 0;
                    for (int i = -rad; i <= rad; i++)
                        for (int j = -rad; j <= rad; j++)
                        {
                            med[index] = image[((x + i) < 0 || (x + i) >= image.Width ? x : (x + i)), ((y + j) < 0 || (y + j) >= image.Height ? y : (y + j))].r;
                            index++;
                        }
                    Array.Sort(med);
                    tempPixel.r = med[2 * rad * rad + 2 * rad + 1];

                    index = 0;
                    for (int i = -rad; i <= rad; i++)
                        for (int j = -rad; j <= rad; j++)
                        {
                            med[index] = image[((x + i) < 0 || (x + i) >= image.Width ? x : (x + i)), ((y + j) < 0 || (y + j) >= image.Height ? y : (y + j))].b;
                            index++;
                        }
                    Array.Sort(med);
                    tempPixel.b = med[2 * rad * rad + 2 * rad + 1];
                    tempImg[x, y] = tempPixel;
                }

             for (int y = 0; y < image.Height; y++)
                for (int x = 0; x < image.Width; x++)
                {
                    image[x, y] = tempImg[x, y];
                }
        }
        
        private static ColorFloatPixel Roberts2(ColorFloatImage image, int x, int y)
        {
            ColorFloatPixel tempPixel;
            tempPixel.a = image[x, y].a;
            tempPixel.r = (image[x, y + 1].r - image[x + 1, y ].r) / 2;
            tempPixel.g = (image[x, y + 1].g - image[x + 1, y ].g) / 2;
            tempPixel.b = (image[x, y + 1].b - image[x + 1, y ].b) / 2;
            return tempPixel;
        }

        private static ColorFloatPixel Roberts1(ColorFloatImage image, int x, int y)
        {
            ColorFloatPixel tempPixel;
            tempPixel.a = image[x, y].a;
            tempPixel = (image[x, y] - image[x + 1, y + 1])/2;
            return tempPixel;
        }

        public static void Roberts(ColorFloatImage image,int type = 0)
        {
            ColorFloatImage tempImg = new ColorFloatImage(image.Width + 2, image.Height + 2);
            ResizeImage(image, tempImg);
            if (type == 1)
                for (int y = 0; y < image.Height; y++)
                    for (int x = 0; x < image.Width; x++)
                    {
                        ColorFloatPixel tempP = Roberts1(tempImg, x + 1, y + 1);
                        tempP.a += 128;
                        tempP.r += 128;
                        tempP.g += 128;
                        tempP.b += 128;
                        image[x, y] = tempP;
                    }
            else if (type == 2)
            {
                for (int y = 0; y < image.Height; y++)
                    for (int x = 0; x < image.Width; x++)
                    {
                        ColorFloatPixel tempP = Roberts2(tempImg, x + 1, y + 1);
                        tempP.a += 128;
                        tempP.r += 128;
                        tempP.g += 128;
                        tempP.b += 128;
                        image[x, y] = tempP;
                    }
            }
            else
            {
                for (int y = 0; y < image.Height; y++)
                    for (int x = 0; x < image.Width; x++)
                    {
                        ColorFloatPixel tempPX = Roberts1(tempImg, x + 1, y + 1),
                            tempPY = Roberts2(tempImg, x + 1, y + 1), tempP;
                        tempP.a = 128 + image[x, y].a;
                        tempP.r = 128 + (float)Math.Sqrt(tempPX.r * tempPX.r + tempPY.r * tempPY.r);
                        tempP.g = 128 + (float)Math.Sqrt(tempPX.g * tempPX.g + tempPY.g * tempPY.g);
                        tempP.b = 128 + (float)Math.Sqrt(tempPX.b * tempPX.b + tempPY.b * tempPY.b);
                        image[x, y] = tempP;
                    }
            }
        }

        public static void FlipImageY(ColorFloatImage image)
        {
            for (int y = 0; y < image.Height; y++)
                for (int x = 0; x < image.Width / 2; x++)
                {
                    ColorFloatPixel p = image[x, y];
                    image[x, y] = image[image.Width - 1 - x, y];
                    image[image.Width - 1 - x, y] = p;
                }
        }

        public static void FlipImageX(ColorFloatImage image)
        {
            for (int y = 0; y < image.Height / 2; y++)
                for (int x = 0; x < image.Width; x++)
                {
                    ColorFloatPixel p = image[x, y];
                    image[x, y] = image[x, image.Height - 1 - y];
                    image[x, image.Height - 1 - y] = p;
                }
        }

        public static void InvertImage(ColorFloatImage image)
        {
            for (int y = 0; y < image.Height; y++)
                for (int x = 0; x < image.Width; x++)
                {
                    ColorFloatPixel p = image[x, y];
                    p.r = 255 - image[x, y].r;
                    p.g = 255 - image[x, y].g;
                    p.b = 255 - image[x, y].b;
                    image[x, y] = p;
                }
        }

        private static void ResizeImage(ColorFloatImage image, ColorFloatImage resizedImg)
        {
            resizedImg[0, 0] = image[0, 0];
            resizedImg[0, resizedImg.Height - 1] = image[0, image.Height - 1];
            resizedImg[resizedImg.Width - 1, 0] = image[image.Width - 1, 0];
            resizedImg[resizedImg.Width - 1, resizedImg.Height - 1] = image[image.Width - 1, image.Height - 1];
            for (int y = 0; y < image.Height; y++)
                for (int x = 0; x < image.Width; x++)
                {
                    resizedImg[x + 1, y + 1] = image[x, y];
                }
            for (int x = 0; x < image.Width; ++x)
            {
                resizedImg[x + 1, 0] = image[x , 0];
                resizedImg[x + 1, image.Height + 1] = image[x, image.Height - 1];
            }
            for (int y = 0; y < image.Height ; ++y)
            {
                resizedImg[0, y + 1] = image[0, y ];
                resizedImg[image.Width + 1, y + 1] = image[image.Width - 1, y];
            }
        }
        private static ColorFloatPixel PrewittY(ColorFloatImage image,int x, int y)
        {
            ColorFloatPixel tempPixel;
            tempPixel.a = image[x, y].a;
            tempPixel = ((image[x, y + 1] - image[x, y - 1]) / 2 + (image[x - 1, y + 1] - image[x - 1, y - 1]) / 2 + (image[x + 1, y + 1] - image[x + 1, y - 1]) / 2) / 3;
            return tempPixel;
        }

        private static ColorFloatPixel PrewittX(ColorFloatImage image,int x,int y)
        {
            ColorFloatPixel tempPixel;
            tempPixel.a = image[x, y].a;
            tempPixel = ((image[x + 1, y] - image[x - 1, y]) / 2 + (image[x + 1, y - 1] - image[x - 1, y - 1]) / 2 + (image[x + 1, y + 1] - image[x - 1, y + 1]) / 2) / 3;
            return tempPixel;
        }

        public static void Prewitt(ColorFloatImage image, string type = "xy")
        {
            ColorFloatImage tempImg = new ColorFloatImage(image.Width + 2, image.Height + 2);
            ResizeImage(image, tempImg);

            if (type == "x")
            {
                for (int y = 0; y < image.Height ; y++)
                    for (int x = 0; x < image.Width ; x++)
                    {
                        ColorFloatPixel tempP = PrewittX(tempImg, x + 1, y + 1);
                        tempP.a += 128;
                        tempP.r += 128;
                        tempP.g += 128;
                        tempP.b += 128;
                        image[x, y] = tempP;
                    }
            } else if (type == "y")
            {
                for (int y = 0; y < image.Height ; y++)
                    for (int x = 0; x < image.Width; x++)
                    {
                        ColorFloatPixel tempP = PrewittY(tempImg, x + 1, y + 1);
                        tempP.a += 128;
                        tempP.r += 128;
                        tempP.g += 128;
                        tempP.b += 128;
                        image[x, y] = tempP;
                    }
            }
            else
            {
               for (int y = 0; y < image.Height; y++)
                    for (int x = 0; x < image.Width ; x++)
                    {
                        ColorFloatPixel tempPX = PrewittX(tempImg, x + 1, y + 1), 
                            tempPY = PrewittY(tempImg,x + 1,y + 1), tempP;
                        tempP.a = 128 + image[x,y].a;
                        tempP.r = 128 + (float)Math.Sqrt(tempPX.r * tempPX.r + tempPY.r * tempPY.r);
                        tempP.g = 128 + (float)Math.Sqrt(tempPX.g * tempPX.g + tempPY.g * tempPY.g);
                        tempP.b = 128 + (float)Math.Sqrt(tempPX.b * tempPX.b + tempPY.b * tempPY.b);
                        image[x, y] = tempP;
                    }
            }
        }

        private static ColorFloatPixel SobelY(ColorFloatImage image, int x, int y)
        {
            ColorFloatPixel tempPixel;
            tempPixel.a = image[x, y].a;
            tempPixel = ((image[x, y + 1] - image[x, y - 1]) + (image[x - 1, y + 1] - image[x - 1, y - 1]) / 2 + (image[x + 1, y + 1] - image[x + 1, y - 1]) / 2) / 3;
            return tempPixel;
        }

        private static ColorFloatPixel SobelX(ColorFloatImage image, int x, int y)
        {
            ColorFloatPixel tempPixel;
            tempPixel.a = image[x, y].a;
            tempPixel = ((image[x + 1, y] - image[x - 1, y]) + (image[x + 1, y - 1] - image[x - 1, y - 1]) / 2 + (image[x + 1, y + 1] - image[x - 1, y + 1]) / 2) / 3;
            return tempPixel;
        }

        public static void Sobel(ColorFloatImage image,string type = "xy")
        {
            ColorFloatImage tempImg = new ColorFloatImage(image.Width + 2, image.Height + 2);
            ResizeImage(image, tempImg);

            if (type == "x")
            {
                for (int y = 0; y < image.Height; y++)
                    for (int x = 0; x < image.Width; x++)
                    {
                        ColorFloatPixel tempP = SobelX(tempImg, x + 1, y + 1);
                        tempP.a += 128;
                        tempP.r += 128;
                        tempP.g += 128;
                        tempP.b += 128;
                        image[x, y] = tempP;
                    }
            }
            else if (type == "y")
            {
                for (int y = 0; y < image.Height; y++)
                    for (int x = 0; x < image.Width; x++)
                    {
                        ColorFloatPixel tempP = SobelY(tempImg, x + 1, y + 1);
                        tempP.a += 128;
                        tempP.r += 128;
                        tempP.g += 128;
                        tempP.b += 128;
                        image[x, y] = tempP;
                    }
            }
            else
            {
                for (int y = 0; y < image.Height; y++)
                    for (int x = 0; x < image.Width; x++)
                    {
                        ColorFloatPixel tempPX = SobelX(tempImg, x + 1, y + 1),
                            tempPY = SobelY(tempImg, x + 1, y + 1), tempP;
                        tempP.a = 128 + image[x, y].a;
                        tempP.r = 128 + (float)Math.Sqrt(tempPX.r * tempPX.r + tempPY.r * tempPY.r);
                        tempP.g = 128 + (float)Math.Sqrt(tempPX.g * tempPX.g + tempPY.g * tempPY.g);
                        tempP.b = 128 + (float)Math.Sqrt(tempPX.b * tempPX.b + tempPY.b * tempPY.b);
                        image[x, y] = tempP;
                    }
            }
        }
#endregion

	}
}
