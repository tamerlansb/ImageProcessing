using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace task2
{
    class ImageProcessing
    {
        private static float
           GaborConversion1(GrayscaleFloatImage image, int x, int y, float[,] matr, float normValue)
        {
            float tempPixel = 0;
           

            int n = matr.GetLength(0);

            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                {
                    tempPixel += matr[i, j] * image[((x + i - (n - 1) / 2) < 0 || (x + i - (n - 1) / 2) >= image.Width ? x : (x + i - (n - 1) / 2)), ((y + j - (n - 1) / 2) < 0 || (y + j - (n - 1) / 2) >= image.Height ? y : (y + j - (n - 1) / 2))];
                    
                }
            

            return tempPixel;
        }
        public static void gabor(ref GrayscaleFloatImage image, float sigma, float gamma, float theta, float lambda, float psi)
        {
            GrayscaleFloatImage tempImg = new GrayscaleFloatImage(image.Width, image.Height);
            int n = (int)(6 * Math.Abs(sigma) + 1);
            n = n == 1 || n % 2 == 0 ? 3 : n;
            float[,] matr = new float[n, n];
            float normValue = 0;
            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                {
                    double x = i * Math.Cos(theta) + j * Math.Sin(theta);
                    double y = -i * Math.Sin(theta) + j * Math.Cos(theta);
                    matr[i, j] = (float)(Math.Exp(-(x * x + y * y * gamma * gamma) / (2 * sigma * sigma)) * Math.Cos(2 * x * Math.PI / lambda + psi));
                    normValue += matr[i, j];
                }

          
            for (int y = 0; y < image.Height; y++)
                for (int x = 0; x < image.Width; x++)
                {
                    tempImg[x, y] = GaborConversion1(image, x, y, matr, normValue);
                }

            float max = image[0, 0], min = image[0,0];
            for (int y = 0; y < image.Height; y++)
                for (int x = 0; x < image.Width; x++)
                {
                    if (image[x, y] > max) max = image[x, y];
                    if (image[x, y] < min) min = image[x, y];
                }
           
            for (int y = 0; y < image.Height; y++)
                for (int x = 0; x < image.Width; x++)
                {
                    tempImg[x, y] = tempImg[x,y]/ ((max * min) /(max + min));
                }
            image = tempImg;
        }
        
        /*************************************/
        private static ColorFloatPixel 
            GaborConversion(ColorFloatImage image, int x, int y, float[,] matr, float normValue)
        {
            ColorFloatPixel tempPixel = image[x, y];
            
            tempPixel.r = 0;
            tempPixel.g = 0;
            tempPixel.b = 0;

            int n = matr.GetLength(0);

            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                {
                    tempPixel.r += matr[i, j] * image[((x + i - (n - 1) / 2) < 0 || (x + i - (n - 1) / 2) >= image.Width ? x : (x + i - (n - 1) / 2)), ((y + j - (n - 1) / 2) < 0 || (y + j - (n - 1) / 2) >= image.Height ? y : (y + j - (n - 1) / 2))].r;
                    tempPixel.g += matr[i, j] * image[((x + i - (n - 1) / 2) < 0 || (x + i - (n - 1) / 2) >= image.Width ? x : (x + i - (n - 1) / 2)), ((y + j - (n - 1) / 2) < 0 || (y + j - (n - 1) / 2) >= image.Height ? y : (y + j - (n - 1) / 2))].g;
                    tempPixel.b += matr[i, j] * image[((x + i - (n - 1) / 2) < 0 || (x + i - (n - 1) / 2) >= image.Width ? x : (x + i - (n - 1) / 2)), ((y + j - (n - 1) / 2) < 0 || (y + j - (n - 1) / 2) >= image.Height ? y : (y + j - (n - 1) / 2))].b;

                }
            tempPixel.r = tempPixel.r / normValue ;
            tempPixel.g = tempPixel.g / normValue ;
            tempPixel.b = tempPixel.b / normValue ;

            return tempPixel;
        }
        public static void gabor(ref ColorFloatImage image,float sigma,float gamma,float theta,float lambda, float psi)
        {
            ColorFloatImage tempImg = new ColorFloatImage(image.Width, image.Height);
            int n = (int)(6 * Math.Abs(sigma) + 1);
            n = n == 1 || n % 2 == 0 ? 3 : n;
            float[,] matr = new float[n, n];
            float normValue = 0;
            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                {
                    double x = i * Math.Cos(theta) + j * Math.Sin(theta);
                    double y = -i * Math.Sin(theta) + j * Math.Cos(theta);
                    matr[i, j] = (float)(Math.Exp(-(x*x + y*y*gamma*gamma)/(2*sigma*sigma)) *Math.Cos(2*x*Math.PI / lambda + psi));
                    normValue += matr[i, j];
                }


            for (int y = 0; y < image.Height; y++)
                for (int x = 0; x < image.Width; x++)
                {
                    tempImg[x, y] = GaborConversion(image, x, y, matr, normValue);
                }
            image = tempImg;
        } 
        private static ColorFloatPixel 
         GaussConversion(ColorFloatImage image,int x, int y,float[,] matr,float normValue)
        {
            ColorFloatPixel tempPixel = image[x,y];
            tempPixel.r = 0;
            tempPixel.g = 0;
            tempPixel.b = 0;
            int n = matr.GetLength(0);

            for (int i = 0; i < n ; ++i )
                for (int j = 0; j < n; ++j)
                {
                    tempPixel.r += matr[j,i]* image[((x + i - (n - 1) / 2) < 0 || (x + i - (n - 1) / 2) >= image.Width ? x : (x + i - (n - 1) / 2)), ((y + j - (n - 1) / 2) < 0 || (y + j - (n - 1) / 2) >= image.Height ? y : (y + j - (n - 1) / 2))].r;
                    tempPixel.g += matr[j,i]* image[((x + i - (n - 1) / 2) < 0 || (x + i - (n - 1) / 2) >= image.Width ? x : (x + i - (n - 1) / 2)), ((y + j - (n - 1) / 2) < 0 || (y + j - (n - 1) / 2) >= image.Height ? y : (y + j - (n - 1) / 2))].g;
                    tempPixel.b += matr[j,i]* image[((x + i - (n - 1) / 2) < 0 || (x + i - (n - 1) / 2) >= image.Width ? x : (x + i - (n - 1) / 2)), ((y + j - (n - 1) / 2) < 0 || (y + j - (n - 1) / 2) >= image.Height ? y : (y + j - (n - 1) / 2))].b;
                    
                }
            tempPixel.r = tempPixel.r / normValue;
            tempPixel.g = tempPixel.g / normValue;
            tempPixel.b = tempPixel.b / normValue;
            
            return tempPixel;
        }
        public static void Gauss(ref ColorFloatImage image, float sigma)
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
                    tempImg[x, y] = GaussConversion(image,x,y,matr,normValue);
                }

            image = tempImg;
        }
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
            tempPixel.r = (image[x, y].r - image[x + 1, y + 1].r)/2;
            tempPixel.g = (image[x, y].g - image[x + 1, y + 1].g)/2;
            tempPixel.b = (image[x, y].b - image[x + 1, y + 1].b)/2;
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
            tempPixel.r = ((image[x, y + 1].r - image[x, y - 1].r) / 2 + (image[x - 1, y + 1].r - image[x - 1, y - 1].r) / 2 + (image[x + 1, y + 1].r - image[x + 1, y - 1].r) / 2) / 3;
            tempPixel.g = ((image[x, y + 1].g - image[x, y - 1].g) / 2 + (image[x - 1, y + 1].g - image[x - 1, y - 1].g) / 2 + (image[x + 1, y + 1].g - image[x + 1, y - 1].g) / 2) / 3;
            tempPixel.b = ((image[x, y + 1].b - image[x, y - 1].b) / 2 + (image[x - 1, y + 1].b - image[x - 1, y - 1].b) / 2 + (image[x + 1, y + 1].b - image[x + 1, y - 1].b) / 2) / 3;
            return tempPixel;
        }
        private static ColorFloatPixel PrewittX(ColorFloatImage image,int x,int y)
        {
            ColorFloatPixel tempPixel;
            tempPixel.a = image[x, y].a;
            tempPixel.r = ((image[x + 1, y].r - image[x - 1, y].r) / 2 + (image[x + 1, y - 1].r - image[x - 1, y - 1].r) / 2 + (image[x + 1, y + 1].r - image[x - 1, y + 1].r) / 2) / 3;
            tempPixel.g = ((image[x + 1, y].g - image[x - 1, y].g) / 2 + (image[x + 1, y - 1].g - image[x - 1, y - 1].g) / 2 + (image[x + 1, y + 1].g - image[x - 1, y + 1].g) / 2) / 3;
            tempPixel.b = ((image[x + 1, y].b - image[x - 1, y].b) / 2 + (image[x + 1, y - 1].b - image[x - 1, y - 1].b) / 2 + (image[x + 1, y + 1].b - image[x - 1, y + 1].b) / 2) / 3;
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
            tempPixel.r = ((image[x, y + 1].r - image[x, y - 1].r) + (image[x - 1, y + 1].r - image[x - 1, y - 1].r) / 2 + (image[x + 1, y + 1].r - image[x + 1, y - 1].r) / 2) / 3;
            tempPixel.g = ((image[x, y + 1].g - image[x, y - 1].g) + (image[x - 1, y + 1].g - image[x - 1, y - 1].g) / 2 + (image[x + 1, y + 1].g - image[x + 1, y - 1].g) / 2) / 3;
            tempPixel.b = ((image[x, y + 1].b - image[x, y - 1].b) + (image[x - 1, y + 1].b - image[x - 1, y - 1].b) / 2 + (image[x + 1, y + 1].b - image[x + 1, y - 1].b) / 2) / 3;
            return tempPixel;
        }
        private static ColorFloatPixel SobelX(ColorFloatImage image, int x, int y)
        {
            ColorFloatPixel tempPixel;
            tempPixel.a = image[x, y].a;
            tempPixel.r = ((image[x + 1, y].r - image[x - 1, y].r) + (image[x + 1, y - 1].r - image[x - 1, y - 1].r) / 2 + (image[x + 1, y + 1].r - image[x - 1, y + 1].r) / 2) / 3;
            tempPixel.g = ((image[x + 1, y].g - image[x - 1, y].g) + (image[x + 1, y - 1].g - image[x - 1, y - 1].g) / 2 + (image[x + 1, y + 1].g - image[x - 1, y + 1].g) / 2) / 3;
            tempPixel.b = ((image[x + 1, y].b - image[x - 1, y].b) + (image[x + 1, y - 1].b - image[x - 1, y - 1].b) / 2 + (image[x + 1, y + 1].b - image[x - 1, y + 1].b) / 2) / 3;
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


    }
}
