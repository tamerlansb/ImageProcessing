using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace task2
{
    class Program
    {
        
        static void Main(string[] args)
        {
            for (int i = 1; i < 8;++i)
            {
                string InputFileName = "C:\\Users\\Tamerlan\\Desktop\\123\\Безымянный" + i.ToString() + ".png",
                    OutputFile = "C:\\Users\\Tamerlan\\Desktop\\123\\Безымянный" + i.ToString() + ".png";
                ColorFloatImage image = ImageIO.FileToColorFloatImage(InputFileName);
                ImageInterpolation.downsample(ref image, 1.5f);
                ImageIO.ImageToFile(image, OutputFile);
            }
            //for (int i = 10; i < 41; ++i)
            //{
            //    string InputFileName = "C:\\Users\\Admin\\Desktop\\Преддиплом. практика\\cell\\cell00" + i.ToString() + ".tif",
            //        OutputFile = "C:\\Users\\Admin\\Desktop\\Преддиплом. практика\\new_cell\\cell00" + i.ToString() + ".tif";
            //    ColorFloatImage image = ImageIO.FileToColorFloatImage(InputFileName);
            //    ImageInterpolation.downsample(ref image, 2.7f);
            //    ImageIO.ImageToFile(image, OutputFile);
            //}
        }
    }
}
