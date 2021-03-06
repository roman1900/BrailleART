﻿using System;
using System.Text;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;

namespace BrailleART
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;
            string photo = "me.jpg";
            Bitmap b = new Bitmap(photo);

            int width = (int)(Math.Ceiling((float)b.Width / 2.0));
            int height = (int)(Math.Ceiling((float)b.Height / 4.0));
            int[] intensityhist = new int[256];
            int count = 0;
            float intensity;
            float ii = 0;
            for (int x = 0; x < b.Width; x++)
            {
                for (int y = 0; y < b.Height; y++)
                {
                    var i = b.GetPixel(x, y).GetBrightness();
                    intensityhist[i >= 1.0 ? (int)255 : (int)(i * 256.0)]++;
                    ii = ii + i;
                    count++;
                }

            }
            intensity = intensityhist.ToList().IndexOf(intensityhist.Max()) / (float)256.0;
            ii = ii / count;
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"output.html", false))
            {
                file.WriteLine($"<html><body style='font-size:5;font-spacing:0'><img src='{photo}'><pre>");
                for (int row = 0; row < height; row++)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int charno = 0; charno < width; charno++)
                    {
                        int braille = 0x2800;
                        float darkest = 1;
                        int darkestpixel = 0x0;
                        for (int x = charno * 2; x < (charno * 2) + 2; x++)
                        {
                            for (int y = row * 4; y < (row * 4) + 4; y++)
                            {
                                if (!(x >= b.Width || y >= b.Height))
                                {
                                    if (b.GetPixel(x, y).GetBrightness() < ii)// && b.GetPixel(x, y).GetBrightness()<ii)
                                    {
                                        braille = braille + BrailleBit(y - (row * 4), x - (charno * 2));
                                    }
                                    else if (b.GetPixel(x, y).GetBrightness() <= darkest)
                                    {
                                        darkest = b.GetPixel(x, y).GetBrightness();
                                        darkestpixel = BrailleBit(y - (row * 4), x - (charno * 2));
                                    }
                                }
                            }

                        }
                        braille = braille == 0x2800 ? braille + darkestpixel : braille;
                        sb.Append((char)braille);
                    }
                    file.WriteLine(sb.ToString());
                    Graphics g
                }
                file.WriteLine(@"</pre></body></html>");
            }
        }

        static int BrailleBit(int row, int col)
        {
            int val = 0;
            switch (row)
            {
                case 0:
                    val = 0x1 << (col * 3);
                    break;
                case 1:
                    val = 0x2 << (col * 3);
                    break;
                case 2:
                    val = 0x4 << (col * 3);
                    break;
                case 3:
                    val = 0x40 << (col * 1);
                    break;
            }
            return val;
        }
    }
}


