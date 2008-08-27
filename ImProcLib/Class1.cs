using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms ;
using System.IO;
namespace ImProcLib
{
    public class ImProc
    {
        static public Bitmap LoadImage(string filename)
        {
            Bitmap bp;
            bp = (Bitmap) Bitmap.FromFile(filename);
            return bp;
        }
        public static void SaveImage(Bitmap b,string filename)
        {
            b.Save(filename);
        }
        public static bool Invert(Bitmap b)
        {
            // GDI+ still lies to us - the return format is BGR, NOT RGB. 

            //BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
            //   ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
                    ImageLockMode.ReadWrite, b.PixelFormat);
            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;
            int nbands = 3;
            if (b.PixelFormat == PixelFormat.Format24bppRgb)
                nbands = 3;
            if (b.PixelFormat == PixelFormat.Format8bppIndexed)
                nbands = 1;
            try
            {
                unsafe
                {
                    byte* p = (byte*)(void*)Scan0;
                    int nOffset = stride - b.Width * nbands;
                    int nWidth = b.Width * nbands;

                    for (int y = 0; y < b.Height; ++y)
                    {
                        for (int x = 0; x < nWidth; ++x)
                        {
                            p[0] = (byte)(255 - p[0]);
                            ++p;
                        }
                        p += nOffset;
                    }
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("error escribiendo" + e.Message + " bandas " + nbands);
                return false;
            }

            b.UnlockBits(bmData);

            return true;
        }
        public static bool greyscale(Bitmap b)
        {
            // GDI+ still lies to us - the return format is BGR, NOT RGB. 

            //BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
            //   ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
                    ImageLockMode.ReadWrite, b.PixelFormat);
            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;
            int nbands = 3;
            if (b.PixelFormat == PixelFormat.Format24bppRgb)
                nbands = 3;
            if (b.PixelFormat == PixelFormat.Format8bppIndexed)
                nbands = 1;
            try
            {
                unsafe
                {
                    byte* p = (byte*)(void*)Scan0;

                    int nOffset = stride - b.Width * 3;

                    byte red, green, blue;

                    for (int y = 0; y < b.Height; ++y)
                    {
                        for (int x = 0; x < b.Width; ++x)
                        {
                            blue = p[0];
                            green = p[1];
                            red = p[2];

                            p[0] = p[1] = p[2] = (byte)(.299 * red
                                + .587 * green
                                + .114 * blue);

                            p += 3;
                        }
                        p += nOffset;
                    }
                }

            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("error escribiendo" + e.Message + " bandas " + nbands);
                return false;
            }

            b.UnlockBits(bmData);

            return true;
        }
        public static bool brightness(Bitmap b, int nBrightness)
        {
            // GDI+ still lies to us - the return format is BGR, NOT RGB. 

            //BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
            //   ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
                    ImageLockMode.ReadWrite, b.PixelFormat);
            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;
            int nbands = 3;
            if (b.PixelFormat == PixelFormat.Format24bppRgb)
                nbands = 3;
            if (b.PixelFormat == PixelFormat.Format8bppIndexed)
                nbands = 1;
            try
            {
                unsafe
                {
                    byte* p = (byte*)(void*)Scan0;

                    int nOffset = stride - b.Width * 3;

                    int nVal;

                    for (int y = 0; y < b.Height; ++y)
                    {
                        for (int x = 0; x < b.Width; ++x)
                        {
                            nVal = (int)(p[0] + nBrightness);

                            if (nVal < 0) nVal = 0;
                            if (nVal > 255) nVal = 255;

                            p[0] = (byte)nVal;

                            ++p;

                        }
                        p += nOffset;
                    }
                }

            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("error escribiendo" + e.Message + " bandas " + nbands);
                return false;
            }

            b.UnlockBits(bmData);

            return true;
        }
        public static bool Statistics(Bitmap b,ref int min ,ref int max, int excludefrom)
        {
            // GDI+ still lies to us - the return format is BGR, NOT RGB. 

            //BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
            //   ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
                    ImageLockMode.ReadWrite, b.PixelFormat);
            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;
            int nbands = 3;
            if (b.PixelFormat == PixelFormat.Format24bppRgb)
                nbands = 3;
            if (b.PixelFormat == PixelFormat.Format8bppIndexed)
                nbands = 1;
            min = 255;
            max = 0;
            try
            {
                unsafe
                {
                    byte* p = (byte*)(void*)Scan0;
                    int nOffset = stride - b.Width * nbands;
                    int nWidth = b.Width * nbands;

                    for (int y = 0; y < b.Height; ++y)
                    {
                        for (int x = 0; x < nWidth; ++x)
                        {
                            if (p[0] < min && p[0]>excludefrom ) min = p[0];
                            if (p[0] > max) max = p[0];
                            ++p;
                        }
                        p += nOffset;
                    }
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("error escribiendo" + e.Message + " bandas " + nbands);
                return false;
            }

            b.UnlockBits(bmData);

            return true;
        }

        public static Bitmap  ColorMap(Bitmap b,int min , int max,string CMapFilename)
        {
            StreamReader re;
            try
            {
                 re = File.OpenText(CMapFilename);
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Error al abrir el fichero de color: " + CMapFilename+ " " + e.Message );
                return b;
            }
            string input = null;
            int[,] CMap=new int[256,3];
            char[] separator=new char [1];
            separator[0]= ' ';

            while ((input = re.ReadLine()) != null)
            {
                try
                {
                    string[] cadenas = input.Split(separator,StringSplitOptions.RemoveEmptyEntries);
                    CMap[int.Parse(cadenas[0]), 0] = int.Parse(cadenas[1]);
                    CMap[int.Parse(cadenas[0]), 1] = int.Parse(cadenas[2]);
                    CMap[int.Parse(cadenas[0]), 2] = int.Parse(cadenas[3]);
                }
                catch (Exception e)
                {
                    System.Windows.Forms.MessageBox.Show(e.Message);
                }
            }


            Bitmap b_out=new Bitmap (b.Width,b.Height,PixelFormat.Format24bppRgb );
            

            //BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
            //   ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
                    ImageLockMode.ReadWrite, b.PixelFormat);
            BitmapData bmData_out = b_out.LockBits(new Rectangle(0, 0, b_out.Width, b_out.Height),
                    ImageLockMode.ReadWrite, b_out.PixelFormat);
            int stride = bmData.Stride;
            int stride_out = bmData_out.Stride;

            System.IntPtr Scan0 = bmData.Scan0;
            System.IntPtr ScanOut = bmData_out.Scan0;

            int nbands = 3;
            if (b.PixelFormat == PixelFormat.Format24bppRgb)
                nbands = 3;
            if (b.PixelFormat == PixelFormat.Format8bppIndexed)
                nbands = 1;
            int val=0;
            try
            {
                unsafe
                {
                    byte* p = (byte*)(void*)Scan0;
                    byte* p_out = (byte*)(void*)ScanOut;

                    int nOffset = stride - b.Width * nbands;
                    int nOffset_out = stride_out - b_out.Width * 3;

                    int nWidth = b.Width * nbands;
                    int nWidth_out = b_out.Width * 3;
                    
                    for (int y = 0; y < b.Height; ++y)
                    {
                        for (int x = 0; x < b.Width ; ++x)
                        {
                            if (p[0] > 3)
                            {
                                val = (int)((p[0] - min) * 255.0 / (max - min));
                            }
                            else val = p[0];

                            p_out[0] = (byte)(CMap[val,0]);
                            p_out[1] = (byte)(CMap[val,1]);
                            p_out[2] = (byte)(CMap[val,2]);
                            p_out+=3;
                            p += nbands;
                        }
                        p += nOffset;
                        p_out += nOffset_out;
                    }
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("error escribiendo" + e.Message + " val " + val);
            }

            b.UnlockBits(bmData);

            return b_out ;
        }

        public static Bitmap subset(Bitmap b,int top, int left, int height, int width)
        {
            Bitmap b_out = new Bitmap(width , height , PixelFormat.Format24bppRgb);

            //BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
            //   ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
                    ImageLockMode.ReadWrite, b.PixelFormat);
            BitmapData bmData_out = b_out.LockBits(new Rectangle(0, 0,b_out.Width  ,b_out.Height ),
                    ImageLockMode.ReadWrite, b_out.PixelFormat);
            int stride = bmData.Stride;
            int stride_out = bmData_out.Stride;

            System.IntPtr Scan0 = bmData.Scan0;
            System.IntPtr ScanOut = bmData_out.Scan0;

            int nbands = 3;
            if (b.PixelFormat == PixelFormat .Format32bppRgb)
                nbands=4;
            if (b.PixelFormat == PixelFormat.Format24bppRgb)
                nbands = 3;
            if (b.PixelFormat == PixelFormat.Format8bppIndexed)
                nbands = 1;
            int val = 0;
            try
            {
                unsafe
                {
                    byte* p = (byte*)(void*)Scan0;
                    byte* p_out = (byte*)(void*)ScanOut;

                    int nOffset = stride - b.Width * nbands;
                    int nOffset_out = stride_out - b_out.Width * 3;

                    int nWidth = b.Width * nbands;
                    int nWidth_out = b_out.Width * 3;

                    for (int y = 0; y < b_out.Height; ++y)
                    {
                        for (int x = 0; x < b_out.Width; ++x)
                        {
                            p_out[0] = p[(x+left)*nbands+(y+top)*bmData.Stride];// (byte)(CMap[val, 0]);
                            p_out[1] = p[(x+left)*nbands+(y+top)*bmData.Stride+1];// (byte)(CMap[val, 1]);
                            p_out[2] = p[(x+left)*nbands+(y+top)*bmData.Stride+2];// (byte)(CMap[val, 2]);
                            p_out += 3;
                         // p += nbands;
                        }
                        //p += nOffset;
                        p_out += nOffset_out;
                    }
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("error escribiendo" + e.Message + " val " + val);
            }

            b.UnlockBits(bmData);
            b_out.UnlockBits(bmData_out);
            return b_out;
        }
    }

}
