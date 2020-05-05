using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

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
            string InputFileName = args[0], OutputFileName = args[1];
            if (!File.Exists(InputFileName))
                return;
          
            switch (args[2])
            {
                case ("harris"):
                {
                        GrayscaleFloatImage image = ImageIO.FileToGrayscaleFloatImage(InputFileName);
                        GrayscaleFloatImage Res = EdgeDetection.harris(image, float.Parse(args[3]));
                        ImageIO.ImageToFile(Res, OutputFileName);
                        break;
                }
                case ("canny"):
                    {
                        ColorFloatImage image = ImageIO.FileToColorFloatImage(InputFileName);
                        GrayscaleFloatImage Reslut =  EdgeDetection.Canny(image, float.Parse(args[3]), float.Parse(args[4]), float.Parse(args[5]));
                        ImageIO.ImageToFile(Reslut, OutputFileName);
                        break;
                    }
                case ("nothing"):
                    {
                        ColorFloatImage image = ImageIO.FileToColorFloatImage(InputFileName);
                        ImageIO.ImageToFile(image, OutputFileName);
                        break;
                    }
                case ("prewitt"):
                {
                    ColorFloatImage image = ImageIO.FileToColorFloatImage(InputFileName);
                    if (args.Length >= 4)
                    {
                        if (args[3] == "y") ImageProcessing.Prewitt(image,"y");
                        if (args[3] == "x") ImageProcessing.Prewitt(image,"x");
                        
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
                        if (args[3] == "y") ImageProcessing.Sobel(image, "y");
                        if (args[3] == "x") ImageProcessing.Sobel(image, "x");
                    }
                    else
                    {
                        ImageProcessing.Sobel(image);
                    }
                    ImageIO.ImageToFile(image, OutputFileName);
                    break;
                }
                case ("mirror"): {
                    if (args.Length >= 4)
                        {
                            ColorFloatImage image = ImageIO.FileToColorFloatImage(InputFileName);
                            if (args[3] == "y") ImageProcessing.FlipImageY(image);
                            if (args[3] == "x") ImageProcessing.FlipImageX(image);
                            ImageIO.ImageToFile(image, OutputFileName);
                        }
                    break;
                }
                case ("roberts"):
                    {
                        ColorFloatImage image = ImageIO.FileToColorFloatImage(InputFileName);
                        if (args.Length >= 4)
                        {
                            if (args[3] == "1") ImageProcessing.Roberts(image, 1);
                            if (args[3] == "2") ImageProcessing.Roberts(image, 2);
                        } else ImageProcessing.Roberts(image);
                        ImageIO.ImageToFile(image, OutputFileName);
                        break;
                    }
                case ("rotate"):
                    {
                        if (args.Length < 5) return;
                        ColorFloatImage image = ImageIO.FileToColorFloatImage(InputFileName);
                        if (args[3] == "cw")
                        {
                            if (args[4] == "90") ImageProcessing.RotateCw90(ref image);
                            if (args[4] == "180") { ImageProcessing.RotateCw90(ref image);
                                ImageProcessing.RotateCw90(ref image);
                            }
                            if (args[4] == "270") {
                                ImageProcessing.RotateCw90(ref image);
                                ImageProcessing.RotateCw90(ref image);
                                ImageProcessing.RotateCw90(ref image);
                            }
                        } else if (args[3] == "ccw")
                        {
                            if (args[4] == "90")
                            {
                                ImageProcessing.RotateCw90(ref image);
                                ImageProcessing.RotateCw90(ref image);
                                ImageProcessing.RotateCw90(ref image);
                            }
                            if (args[4] == "180")
                            {
                                ImageProcessing.RotateCw90(ref image);
                                ImageProcessing.RotateCw90(ref image);
                            }
                            if (args[4] == "270") ImageProcessing.RotateCw90(ref image);
                        }
                        ImageIO.ImageToFile(image, OutputFileName);
                    }
                    break;
                case ("gauss"): {
                        if (args.Length < 4) return;
                        ColorFloatImage image = ImageIO.FileToColorFloatImage(InputFileName);
                        ImageProcessing.Gauss(ref image, float.Parse(args[3]));
                        ImageIO.ImageToFile(image, OutputFileName);
                        break; }
                case ("median"):
                    {
                        if (args.Length < 4) return;
                        ColorFloatImage image = ImageIO.FileToColorFloatImage(InputFileName);
                        ImageProcessing.Median(image, Int32.Parse(args[3]));
                        ImageIO.ImageToFile(image, OutputFileName);
                        break;
                    }
                case ("gabor"):
                    {
                        ColorFloatImage image = ImageIO.FileToColorFloatImage(InputFileName);
                        ImageProcessing.gabor(ref image, Single.Parse(args[3]), Single.Parse(args[4]), Single.Parse(args[5]), Single.Parse(args[6]), Single.Parse(args[7]));
                        ImageIO.ImageToFile(image, OutputFileName);
                        break;
                    }
                default: break;
            }
            //GrayscaleFloatImage image = ImageIO.FileToGrayscaleFloatImage(InputFileName);
            //FlipImage(image); 
        }
    }
}
