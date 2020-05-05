using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Globalization;

namespace ImageReadCS
{
    class Program
    {
        
        
        static void FlipImage(GrayscaleFloatImage image)
        {
            for (int y = 0; y < image.Height; y++)
                for (int x = 0; x < image.Width / 2; x++)
                {
                    float p = image[x, y];
                    image[x, y] = image[image.Width - 1 - x, y];
                    image[image.Width - 1 - x, y] = p;
                }
        }
      

        static void Main(string[] args)
        {
			if (args.Length < 3)
                return;
            string InputFileName = args[args.Length-2], OutputFileName = args[args.Length-1];
            if (!File.Exists(InputFileName))
                return;

			switch (args[0])
            {
                case ("prewitt"):
                {
                    ColorFloatImage image = ImageIO.FileToColorFloatImage(InputFileName);
                    if (args.Length >= 4)
                    {
                        if (args[1] == "y") ImageProcessing.Prewitt(image,"y");
                        if (args[1] == "x") ImageProcessing.Prewitt(image,"x");

						}  else {
                        ImageProcessing.Prewitt(image);
                    }
                    ImageIO.ImageToFile(image, OutputFileName);
                    break;
                }
                case ("invert"):
                {
                    ColorFloatImage image = ImageIO.FileToColorFloatImage(InputFileName);
                    ImageProcessing.InvertImage(image);
                    ImageIO.ImageToFile(image, OutputFileName);
                    break;
                }
                case ("sobel"):
                {
                    ColorFloatImage image = ImageIO.FileToColorFloatImage(InputFileName);
                    if (args.Length >= 4)
                    {
                        if (args[1] == "y") ImageProcessing.Sobel(image, "y");
                        if (args[1] == "x") ImageProcessing.Sobel(image, "x");
                    }
                    else
                    {
                        ImageProcessing.Sobel(image);
                    }
                    ImageIO.ImageToFile(image, OutputFileName);
                    break;
                }
                case ("mirror"):
					{
                    if (args.Length >= 4)
                        {
                            ColorFloatImage image = ImageIO.FileToColorFloatImage(InputFileName);
                            if (args[1] == "y") ImageProcessing.FlipImageY(image);
                            if (args[1] == "x") ImageProcessing.FlipImageX(image);
                            ImageIO.ImageToFile(image, OutputFileName);
                        }
                    break;
                }
                case ("roberts"):
                    {
                        ColorFloatImage image = ImageIO.FileToColorFloatImage(InputFileName);
                        if (args.Length >= 4)
                        {
                            if (args[1] == "1") ImageProcessing.Roberts(image, 1);
                            if (args[1] == "2") ImageProcessing.Roberts(image, 2);
                        } else ImageProcessing.Roberts(image);
                        ImageIO.ImageToFile(image, OutputFileName);
                        break;
                    }
                case ("rotate"):
                    {
						if (args.Length < 5) return;
						ColorFloatImage image = ImageIO.FileToColorFloatImage(InputFileName);
						ImageProcessing.Rotate(ref image, args[1], float.Parse(args[2], CultureInfo.InvariantCulture));
                        ImageIO.ImageToFile(image, OutputFileName);
                    }
                    break;
                case ("gauss"): {
                        if (args.Length < 4) return;
                        ColorFloatImage image = ImageIO.FileToColorFloatImage(InputFileName);
						if (args.Length < 5)
						{
							image = ImageProcessing.Gauss( image, float.Parse(args[1], CultureInfo.InvariantCulture));
						}
						else
						{
							image = ImageProcessing.Gauss( image, float.Parse(args[1], CultureInfo.InvariantCulture), float.Parse(args[2], CultureInfo.InvariantCulture));
						}
						ImageIO.ImageToFile(image, OutputFileName);
                        break; }
                case ("median"):
                    {
                        if (args.Length < 4) return;
                        ColorFloatImage image = ImageIO.FileToColorFloatImage(InputFileName);
                        ImageProcessing.Median(image, Int32.Parse(args[1]));
                        ImageIO.ImageToFile(image, OutputFileName);
                        break;
                    }
				case ("gabor"):
					{
						GrayscaleFloatImage image = ImageIO.FileToGrayscaleFloatImage(InputFileName);
						image = ImageProcessing.Gabor(image, Single.Parse(args[1], CultureInfo.InvariantCulture), Single.Parse(args[2], CultureInfo.InvariantCulture), Single.Parse(args[3], CultureInfo.InvariantCulture), Single.Parse(args[4], CultureInfo.InvariantCulture), Single.Parse(args[5], CultureInfo.InvariantCulture));
						ImageIO.ImageToFile(image, OutputFileName);
						break;
					}
				case ("gradient"):
                    {
						ColorFloatImage image = ImageIO.FileToColorFloatImage(InputFileName);
                        ImageProcessing.gradient(ref image, Single.Parse(args[1], CultureInfo.InvariantCulture));
                        ImageIO.ImageToFile(ImageIO.BitmapToGrayscaleFloatImage( ImageIO.ImageToBitmap (image)), OutputFileName);
                        break;
                    }
				case ("vessels"):
					{
						ColorFloatImage image = ImageIO.FileToColorFloatImage(InputFileName);
						var result = ImageProcessing.vessels( image, Single.Parse(args[3], CultureInfo.InvariantCulture));
						ImageIO.ImageToFile(result, OutputFileName);
						break;
					}
				case ("canny"):
					{
						ColorFloatImage image = ImageIO.FileToColorFloatImage(InputFileName);
						var result = EdgeDetection.Canny(image, Single.Parse(args[1], CultureInfo.InvariantCulture), Single.Parse(args[2], CultureInfo.InvariantCulture), Single.Parse(args[3], CultureInfo.InvariantCulture));
						ImageIO.ImageToFile(result, OutputFileName);
						break;
					}
				case ("mse"):
					{
						ColorFloatImage image1 = ImageIO.FileToColorFloatImage(args[1]),
							 image2 = ImageIO.FileToColorFloatImage(args[2]);
						Console.WriteLine(ImageInterpolation.MSE(image1, image2));
						break;
					}
				case ("psnr"):
					{
						ColorFloatImage image1 = ImageIO.FileToColorFloatImage(args[1]),
							 image2 = ImageIO.FileToColorFloatImage(args[2]);
						Console.WriteLine(ImageInterpolation.PSNR(image1, image2));
						break;
					}
				case ("ssim"):
					{
						ColorFloatImage image1 = ImageIO.FileToColorFloatImage(args[1]),
							 image2 = ImageIO.FileToColorFloatImage(args[2]);
						Console.WriteLine(ImageInterpolation.SSIM(image1, image2));
						break;
					}
				case ("mssim"):
					{
						ColorFloatImage image1 = ImageIO.FileToColorFloatImage(args[1]),
							 image2 = ImageIO.FileToColorFloatImage(args[2]);
						Console.WriteLine(ImageInterpolation.MSSIM(image1, image2));
						break;
					}
				case ("unsharp"):
					{
						ColorFloatImage image = ImageIO.FileToColorFloatImage(InputFileName);
						var result = ImageProcessing.unsharp(image);
						ImageIO.ImageToFile(result, OutputFileName);
						break;
					}

				default: break;
            }
            //GrayscaleFloatImage image = ImageIO.FileToGrayscaleFloatImage(InputFileName);
            //FlipImage(image); 
        }
    }
}
