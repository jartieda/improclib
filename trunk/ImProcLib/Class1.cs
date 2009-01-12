/*
 * BSD Licence:
 * Copyright (c) 2001, 2002 Jorge Artieda [ jartieda@yahoo.com ]
 * Diphernet [ www.diphernet.com ]
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without 
 * modification, are permitted provided that the following conditions are met:
 *
 * 1. Redistributions of source code must retain the above copyright notice, 
 * this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright 
 * notice, this list of conditions and the following disclaimer in the 
 * documentation and/or other materials provided with the distribution.
 * 3. Neither the name of the <ORGANIZATION> nor the names of its contributors
 * may be used to endorse or promote products derived from this software
 * without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE REGENTS OR CONTRIBUTORS BE LIABLE FOR
 * ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
 * DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
 * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
 * LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY
 * OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH
 * DAMAGE.
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.IO;
using Exocortex.DSP;
using System.Collections;


namespace ImProcLib
{
    public class ImProc
    {
        public class ComplexImage
        {
            public int Width;
            public int Height;
            public ComplexF[] data;
            public ComplexImage(int w, int h)
            {
                Width = w;
                Height = h; 
                data = new ComplexF[Width * Height];
            }
            public ComplexImage()
            {

            }

        }
        public class ConvMatrix
        {
            public int TopLeft;
            public int TopMid;
            public int TopRight;
            public int MidLeft;
            public int Pixel;
            public int MidRight;
            public int BottomLeft;
            public int BottomMid;
            public int BottomRight;
            public int Factor;
            public int Offset;
        }
        static public Bitmap LoadImage(string filename)
        {
            Bitmap bp;
            bp = (Bitmap)Bitmap.FromFile(filename);
            return bp;
        }
        public static void SaveImage(Bitmap b, string filename)
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
        public static bool Reclass(Bitmap b, int classnum)
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
            int c;
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
                            c = p[0] / classnum;
                            p[0] = (byte)(c * (255 / classnum));
                            //p[0] = (byte)(255 - p[0]);
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
        public static bool Threshold(Bitmap b, int limit)
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
                            if (p[0] > limit) p[0] = 255;
                            else p[0] = 0;
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
        public static bool Add(Bitmap b, Bitmap a)
        {
            // GDI+ still lies to us - the return format is BGR, NOT RGB. 

            //BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width,b.Height),
            //   ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            if ((a.Width != b.Width) || (a.Height != b.Height))
            {
                return false;
            }
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
                    ImageLockMode.ReadWrite, b.PixelFormat);
            BitmapData amData = a.LockBits(new Rectangle(0, 0, a.Width, a.Height),
                    ImageLockMode.ReadWrite, a.PixelFormat);
            int astride = bmData.Stride;
            int bstride = amData.Stride;
            System.IntPtr aScan0 = amData.Scan0;
            System.IntPtr bScan0 = bmData.Scan0;
            int nbands = 3;
            if (b.PixelFormat == PixelFormat.Format24bppRgb)
                nbands = 3;
            if (b.PixelFormat == PixelFormat.Format8bppIndexed)
                nbands = 1;
            int c;
            try
            {
                unsafe
                {
                    byte* ap = (byte*)(void*)aScan0;
                    byte* bp = (byte*)(void*)bScan0;
                    int bnOffset = bstride - b.Width * nbands;
                    int bnWidth = b.Width * nbands;
                    int anOffset = astride - a.Width * nbands;
                    int anWidth = a.Width * nbands;

                    for (int y = 0; y < b.Height; ++y)
                    {
                        for (int x = 0; x < bnWidth; ++x)
                        {
                            c = bp[0] + ap[0];
                            if (c > 255) bp[0] = 255;
                            else bp[0] = (byte)c;
                            ++bp;
                            ++ap;
                        }
                        bp += bnOffset;
                        ap += anOffset;
                    }
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("error escribiendo" + e.Message + " bandas " + nbands);
                return false;
            }

            b.UnlockBits(bmData);
            a.UnlockBits(amData);

            return true;
        }
        public static bool Conv3x3(Bitmap b, ConvMatrix m)
        {
            if (0 == m.Factor)
                return false;
            Bitmap bSrc = (Bitmap)b.Clone();
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
                ImageLockMode.ReadWrite, b.PixelFormat);
            BitmapData bmSrc = bSrc.LockBits(new Rectangle(0, 0, b.Width, b.Height),
                ImageLockMode.ReadWrite, b.PixelFormat);

            int stride = bmData.Stride;
            int stride2 = stride * 2;
            System.IntPtr Scan0 = bmData.Scan0;
            System.IntPtr SrcScan0 = bmSrc.Scan0;
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
                    byte* pSrc = (byte*)(void*)SrcScan0;
                    int nOffset = stride - b.Width * nbands;
                    int nWidth = b.Width - 2;
                    int nHeight = b.Height - 2;
                    int nPixel;
                    for (int y = 0; y < nHeight; ++y)
                    {
                        for (int x = 0; x < nWidth; ++x)
                        {
                            for (int band = 0; band < nbands; band++)
                            {
                                nPixel = (((
                                    (pSrc[band] * m.TopLeft) +
                                    (pSrc[1 * nbands + band] * m.TopMid) +
                                    (pSrc[2 * nbands + band] * m.TopRight) +
                                    (pSrc[band + stride] * m.MidLeft) +
                                    (pSrc[1 * nbands + band + stride] * m.Pixel) +
                                   (pSrc[2 * nbands + band + stride] * m.MidRight) +
                                    (pSrc[band + stride2] * m.BottomLeft) +
                                    (pSrc[1 * nbands + band + stride2] * m.BottomMid) +
                                    (pSrc[2 * nbands + band + stride2] * m.BottomRight)) / m.Factor) + m.Offset);
                                if (nPixel < 0) nPixel = 0;
                                if (nPixel > 255) nPixel = 255;
                                p[1 * nbands + band + stride] = (byte)nPixel;
                            }
                            p += nbands;
                            pSrc += nbands;
                        }
                        p += nOffset;
                        pSrc += nOffset;
                    }
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("error escribiendo" + e.Message + " bandas " + nbands);
                return false;
            }

            b.UnlockBits(bmData);
            bSrc.UnlockBits(bmSrc);
            return true;
        }
        public static bool Dilate(Bitmap b)
        {

            Bitmap bSrc = (Bitmap)b.Clone();
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
                ImageLockMode.ReadWrite, b.PixelFormat);
            BitmapData bmSrc = bSrc.LockBits(new Rectangle(0, 0, b.Width, b.Height),
                ImageLockMode.ReadWrite, b.PixelFormat);

            int stride = bmData.Stride;
            int stride2 = stride * 2;
            System.IntPtr Scan0 = bmData.Scan0;
            System.IntPtr SrcScan0 = bmSrc.Scan0;
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
                    byte* pSrc = (byte*)(void*)SrcScan0;
                    int nOffset = stride - b.Width * nbands;
                    int nWidth = b.Width - 2;
                    int nHeight = b.Height - 2;
                    int nPixel;
                    for (int y = 0; y < nHeight; ++y)
                    {
                        for (int x = 0; x < nWidth; ++x)
                        {
                            for (int band = 0; band < nbands; band++)
                            {
                                if ((pSrc[band] > 0 || (pSrc[1 * nbands + band] > 0) || (pSrc[2 * nbands + band] > 0) ||
                                    (pSrc[band + stride] > 0) || (pSrc[1 * nbands + band + stride] > 0) || (pSrc[2 * nbands + band + stride] > 0) ||
                                    (pSrc[band + stride2] > 0) || (pSrc[1 * nbands + band + stride2] > 0) || (pSrc[2 * nbands + band + stride2] > 0)))
                                {
                                    nPixel = 255;
                                }
                                else
                                {
                                    nPixel = 0;
                                }
                                p[1 * nbands + band + stride] = (byte)nPixel;
                            }
                            p += nbands;
                            pSrc += nbands;
                        }
                        p += nOffset;
                        pSrc += nOffset;
                    }
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("error escribiendo" + e.Message + " bandas " + nbands);
                return false;
            }

            b.UnlockBits(bmData);
            bSrc.UnlockBits(bmSrc);
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
        public static bool Statistics(Bitmap b, ref int min, ref int max, int excludefrom)
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
                            if (p[0] < min && p[0] > excludefrom) min = p[0];
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
        public static Bitmap ColorMap(Bitmap b, int min, int max, string CMapFilename)
        {
            StreamReader re;
            try
            {
                re = File.OpenText(CMapFilename);
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Error al abrir el fichero de color: " + CMapFilename + " " + e.Message);
                return b;
            }
            string input = null;
            int[,] CMap = new int[256, 3];
            char[] separator = new char[1];
            separator[0] = ' ';

            while ((input = re.ReadLine()) != null)
            {
                try
                {
                    string[] cadenas = input.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    CMap[int.Parse(cadenas[0]), 0] = int.Parse(cadenas[1]);
                    CMap[int.Parse(cadenas[0]), 1] = int.Parse(cadenas[2]);
                    CMap[int.Parse(cadenas[0]), 2] = int.Parse(cadenas[3]);
                }
                catch (Exception e)
                {
                    System.Windows.Forms.MessageBox.Show(e.Message);
                }
            }


            Bitmap b_out = new Bitmap(b.Width, b.Height, PixelFormat.Format24bppRgb);


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

                    for (int y = 0; y < b.Height; ++y)
                    {
                        for (int x = 0; x < b.Width; ++x)
                        {
                            if (p[0] > 3)
                            {
                                val = (int)((p[0] - min) * 255.0 / (max - min));
                            }
                            else val = p[0];

                            p_out[0] = (byte)(CMap[val, 2]);
                            p_out[1] = (byte)(CMap[val, 1]);
                            p_out[2] = (byte)(CMap[val, 0]);
                            p_out += 3;
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
            
            return b_out;
        }
        public static Bitmap ColorMapIndexed(Bitmap b, int min, int max, string CMapFilename)
        {
            StreamReader re;
            try
            {
                re = File.OpenText(CMapFilename);
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Error al abrir el fichero de color: " + CMapFilename + " " + e.Message);
                return b;
            }
            string input = null;
            int[,] CMap = new int[256, 3];
            char[] separator = new char[1];
            separator[0] = ' ';

            while ((input = re.ReadLine()) != null)
            {
                try
                {
                    string[] cadenas = input.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    CMap[int.Parse(cadenas[0]), 0] = int.Parse(cadenas[1]);
                    CMap[int.Parse(cadenas[0]), 1] = int.Parse(cadenas[2]);
                    CMap[int.Parse(cadenas[0]), 2] = int.Parse(cadenas[3]);
                }
                catch (Exception e)
                {
                    System.Windows.Forms.MessageBox.Show(e.Message);
                }
            }
            // check palette
            System.Drawing.Imaging.ColorPalette cp = b.Palette;
            // init palette
            int val;
            for (int i = 0; i < 256; i++)
            {
                val = (int)((i - min) * 255.0 / (max - min));
                cp.Entries[i] = Color.FromArgb(CMap[val, 0], CMap[val, 1], CMap[val, 2]);
            }
            b.Palette = cp;
            
            return b;
        }
        public static Bitmap subset(Bitmap b, int top, int left, int height, int width)
        {
            Bitmap b_out = new Bitmap(width, height, PixelFormat.Format24bppRgb);

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
            if (b.PixelFormat == PixelFormat.Format32bppRgb)
                nbands = 4;
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
                            p_out[0] = p[(x + left) * nbands + (y + top) * bmData.Stride];// (byte)(CMap[val, 0]);
                            p_out[1] = p[(x + left) * nbands + (y + top) * bmData.Stride + 1];// (byte)(CMap[val, 1]);
                            p_out[2] = p[(x + left) * nbands + (y + top) * bmData.Stride + 2];// (byte)(CMap[val, 2]);
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
        public static Bitmap padToNext2Power(Bitmap b)
        {
            if (b == null)
            {
                return null;
            }
            int t = b.Height;
            t = t - 1;
            t = t | (t >> 1);
            t = t | (t >> 2);
            t = t | (t >> 4);
            t = t | (t >> 8);
            t = t | (t >> 16);
            int height2 = t + 1;
            t = b.Width;
            t = t - 1;
            t = t | (t >> 1);
            t = t | (t >> 2);
            t = t | (t >> 4);
            t = t | (t >> 8);
            t = t | (t >> 16);
            int width2 = t + 1;

            if ((height2 != b.Height) || (width2 != b.Width))
            {
                Bitmap bout = new Bitmap(width2, height2, b.PixelFormat);
                BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
                                                ImageLockMode.ReadWrite, b.PixelFormat);
                int stride = bmData.Stride;
                System.IntPtr Scan0 = bmData.Scan0;
                int nbands = 3;
                if (b.PixelFormat == PixelFormat.Format24bppRgb)
                    nbands = 3;
                if (b.PixelFormat == PixelFormat.Format8bppIndexed)
                    nbands = 1;

                BitmapData bmDataOut = bout.LockBits(new Rectangle(0, 0, bout.Width, bout.Height),
                                                    ImageLockMode.ReadWrite, bout.PixelFormat);
                int strideOut = bmDataOut.Stride;
                System.IntPtr OutScan0 = bmDataOut.Scan0;
                int nbandsOut = 3;
                if (bout.PixelFormat == PixelFormat.Format24bppRgb)
                    nbandsOut = 3;
                if (bout.PixelFormat == PixelFormat.Format8bppIndexed)
                    nbandsOut = 1;
                try
                {
                    unsafe
                    {
                        byte* p = (byte*)(void*)Scan0;
                        byte* pout = (byte*)(void*)OutScan0;
                        int nOffset = stride - b.Width * nbands;
                        int nWidth = b.Width * nbands;
                        int nOffsetOut = strideOut - bout.Width;
                        for (int y = 0; y < b.Height; ++y)
                        {
                            for (int x = 0; x < b.Width; ++x)
                            {
                                int apos = y * strideOut + x * nbandsOut;
                                int pos = y * stride + x * nbands;
                                for (int band = 0; band < nbands; band++)
                                {
                                    pout[apos + band] = p[pos + band];
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    System.Windows.Forms.MessageBox.Show("error escribiendo" + e.Message + " bandas " + nbands);
                    return null;
                }
                b.UnlockBits(bmData);
                bout.UnlockBits(bmDataOut);
                return bout;
            }
            else
            {
                return b;
            }
        }
        public static Bitmap Mult(Bitmap a, Bitmap b)
        {
            if (b == null)
            {
                return null;
            }
            if (a == null)
            {
                return null;
            }

            if ((a.Height != b.Height) || (a.Width != b.Width))
            {
                return null;
            }
            Bitmap bout = new Bitmap(b.Width, b.Width, b.PixelFormat);

            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
                                            ImageLockMode.ReadWrite, b.PixelFormat);
            int bstride = bmData.Stride;
            System.IntPtr bScan0 = bmData.Scan0;
            int bnbands = 3;
            if (b.PixelFormat == PixelFormat.Format24bppRgb)
                bnbands = 3;
            if (b.PixelFormat == PixelFormat.Format8bppIndexed)
                bnbands = 1;

            BitmapData amData = a.LockBits(new Rectangle(0, 0, a.Width, a.Height),
                                                ImageLockMode.ReadWrite, a.PixelFormat);
            int astride = amData.Stride;
            System.IntPtr aScan0 = amData.Scan0;
            int anbands = 3;
            if (bout.PixelFormat == PixelFormat.Format24bppRgb)
                anbands = 3;
            if (bout.PixelFormat == PixelFormat.Format8bppIndexed)
                anbands = 1;

            BitmapData bmDataOut = bout.LockBits(new Rectangle(0, 0, bout.Width, bout.Height),
                                               ImageLockMode.ReadWrite, bout.PixelFormat);
            int strideOut = bmDataOut.Stride;
            System.IntPtr OutScan0 = bmDataOut.Scan0;
            int nbandsOut = 3;
            if (bout.PixelFormat == PixelFormat.Format24bppRgb)
                nbandsOut = 3;
            if (bout.PixelFormat == PixelFormat.Format8bppIndexed)
                nbandsOut = 1;
            try
            {
                unsafe
                {
                    byte* bp = (byte*)(void*)bScan0;
                    byte* ap = (byte*)(void*)aScan0;
                    byte* pout = (byte*)(void*)OutScan0;
                    for (int y = 0; y < b.Height; ++y)
                    {
                        for (int x = 0; x < b.Width; ++x)
                        {
                            int apos = y * astride + x * anbands;
                            for (int band = 0; band < anbands; band++)
                            {
                                pout[apos + band] = (byte)(ap[apos + band] * bp[apos + band]);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("error escribiendo" + e.Message);
                return null;
            }
            b.UnlockBits(bmData);
            bout.UnlockBits(bmDataOut);
            return bout;
        }
        public static Bitmap Gausian(int size, double sigma, out double factor)
        {
            Bitmap bout = new Bitmap(size, size, PixelFormat.Format8bppIndexed);
            BitmapData bmDataOut = bout.LockBits(new Rectangle(0, 0, bout.Width, bout.Height),
                                                   ImageLockMode.ReadWrite, bout.PixelFormat);
            int strideOut = bmDataOut.Stride;
            System.IntPtr OutScan0 = bmDataOut.Scan0;
            int nbandsOut = 1;
            factor = 1;
            try
            {
                unsafe
                {
                    byte* pout = (byte*)(void*)OutScan0;
                    for (int y = 0; y < bout.Height; ++y)
                    {
                        for (int x = 0; x < bout.Width; ++x)
                        {
                            double xx = x - bout.Width / 2.0;
                            double yy = y - bout.Height / 2.0;
                            if ((y == 0) && (x == 0))
                            {
                                factor = 1 / ((1 / (2 * Math.PI * sigma * sigma)) * Math.Exp(-(xx * xx + yy * yy) / (2 * sigma * sigma)));
                            }
                            int apos = y * strideOut + x * nbandsOut;
                            for (int band = 0; band < nbandsOut; band++)
                            {
                                pout[apos + band] = (byte)(factor * ((1 / (2 * Math.PI * sigma * sigma)) * Math.Exp(-(xx * xx + yy * yy) / (2 * sigma * sigma))));
                            }
                        }
                    }
                }
                bout.UnlockBits(bmDataOut);
                return bout;

            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("error escribiendo" + e.Message);
                bout.UnlockBits(bmDataOut);
                return null;
            }

        }
        public static ComplexImage ForwardFFT(Bitmap b)
        {
            if (b == null)
            {
                return null;
            }

            float scale = 1f / (float)Math.Sqrt(b.Width * b.Height);
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
                                           ImageLockMode.ReadWrite, b.PixelFormat);
            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;

            int nbands = 3;
            if (b.PixelFormat == PixelFormat.Format32bppRgb)
                nbands = 4;
            if (b.PixelFormat == PixelFormat.Format24bppRgb)
                nbands = 3;
            if (b.PixelFormat == PixelFormat.Format8bppIndexed)
                nbands = 1;
            int val = 0;
            ComplexImage cimg;
            try
            {
                unsafe
                {
                    byte* p = (byte*)(void*)Scan0;
                    
                    int nOffset = stride - b.Width * nbands;
                    
                    int nWidth = b.Width * nbands;
                    cimg = new ComplexImage(b.Width ,b.Height );

                    ComplexF[] data = cimg.data;
                    
                    int i;
                    for (int y = 0; y < b.Height; ++y)
                    {
                        for (int x = 0; x < b.Width; ++x)
                        {
                            i = x + y * bmData.Stride;
                            if (nbands == 1) data[i].Re = p[i];
                            else
                            {
                                Color c = Color.FromArgb(p[i]);
                                data[i].Re = ((float)c.R + (float)c.G + (float)c.B) / (3f * 256f);
                            }
                        }
                    }
                    b.UnlockBits(bmData);

                    //int offset = 0;
                    //for (int y = 0; y < b.Height; y++)
                    //{
                    //    for (int x = 0; x < b.Width; x++)
                    //    {
                    //        if (((x + y) & 0x1) != 0)
                    //        {
                    //            data[offset] *= -1;
                    //        }
                    //        offset++;
                    //    }
                    //}

                    Fourier.FFT2(data, b.Width, b.Height, FourierDirection.Forward);

                    //cimage.FrequencySpace = true;
                    //for (i = 0; i < data.Length; i++)
                    //{
                    //    data[i] *= scale;
                    //}

                }
                return cimg;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                return null;
            }

        }
        public static Bitmap InverseFFT(ComplexImage cimg)
        {
            if (cimg == null)
            {
                return null;
            }
            Bitmap b = new Bitmap(cimg.Width, cimg.Height , PixelFormat.Format8bppIndexed);
            float scale = 1f / (float)Math.Sqrt(b.Width * b.Height);
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
                                           ImageLockMode.ReadWrite, b.PixelFormat);
            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;

            int nbands = 3;
            if (b.PixelFormat == PixelFormat.Format32bppRgb)
                nbands = 4;
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

                    int nOffset = stride - b.Width * nbands;

                    int nWidth = b.Width * nbands;
                    
                    Fourier.FFT2(cimg.data , cimg.Width, cimg.Height, FourierDirection.Backward);
                    int i = 0;
                    for (int y = 0; y < b.Height; ++y)
                    {
                        for (int x = 0; x < b.Width; ++x)
                        {
                            i = x + y * bmData.Stride;
                            //if (Math.Sqrt(cimg.data[i].Re * cimg.data[i].Re + cimg.data[i].Im * cimg.data[i].Im) > 255)
                            //    p[i] = 255;
                            //else
                            //    p[i] = (byte)Math.Sqrt(cimg.data[i].Re * cimg.data[i].Re + cimg.data[i].Im * cimg.data[i].Im);
                            p[i] = (byte) cimg.data[i].Re;

                        }
                    }
                    //  cimage.FrequencySpace = true;

                    //int offset = 0;
                    //for (int y = 0; y < b.Height; y++)
                    //{
                    //    for (int x = 0; x < b.Width; x++)
                    //    {
                    //        if (((x + y) & 0x1) != 0)
                    //        {
                    //            data[offset] *= -1;
                    //        }
                    //        offset++;
                    //    }
                    //}

                    for (i = 0; i < cimg.data.Length; i++)
                    {
                        cimg.data[i] *= scale;
                    }
                    b.UnlockBits(bmData);

                }
                return b;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                return null;
            }

        }
        public static Bitmap Complex2Bitmap(ComplexImage  cimg)
        {
            Bitmap b_out = new Bitmap(cimg.Width, cimg.Height, PixelFormat.Format8bppIndexed);
            BitmapData bmData_out = b_out.LockBits(new Rectangle(0, 0, b_out.Width, b_out.Height),
                    ImageLockMode.ReadWrite, b_out.PixelFormat);
            int stride_out = bmData_out.Stride;
            System.IntPtr ScanOut = bmData_out.Scan0;
            //ComplexImage cimg = ForwardFFT(b);
            try
            {
                unsafe
                {
                    byte* p_out = (byte*)(void*)ScanOut;

                    int nOffset_out = stride_out - b_out.Width * 3;

                    int nWidth_out = b_out.Width * 3;
                    int i;
                    for (int y = 0; y < b_out.Height; ++y)
                    {
                        for (int x = 0; x < b_out.Width; ++x)
                        {
                            i = x + y * bmData_out.Stride;
                            if (Math.Sqrt(cimg.data[i].Re * cimg.data[i].Re + cimg.data[i].Im * cimg.data[i].Im) > 255)
                                p_out[i] = 255;
                            else
                                p_out[i] = (byte)Math.Sqrt(cimg.data[i].Re * cimg.data[i].Re + cimg.data[i].Im * cimg.data[i].Im);

                        }
                    }
                }
                b_out.UnlockBits(bmData_out);
                return b_out;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                return null;
            }
        }
        public static bool Smooth(Bitmap b)
        {
            ConvMatrix m = new ConvMatrix();
            m.BottomLeft = 1;
            m.BottomMid = 1;
            m.BottomRight = 1;
            m.MidLeft = 1;
            m.Pixel = 1;
            m.MidRight = 1;
            m.TopLeft = 1;
            m.TopMid = 1;
            m.TopRight = 1;
            m.Factor = 1 / 9;
            m.Offset = 0;
            Conv3x3(b, m);
            return true;
        }
        public static bool HSobel(Bitmap b)
        {
            ConvMatrix m = new ConvMatrix();
            m.BottomLeft = -1;
            m.BottomMid = -2;
            m.BottomRight = -1;
            m.MidLeft = 0;
            m.Pixel = 0;
            m.MidRight = 0;
            m.TopLeft = 1;
            m.TopMid = 2;
            m.TopLeft = 1;
            m.Factor = 1;
            m.Offset = 0;
            Conv3x3(b, m);
            return true;
        }
        public static bool GausianBlur3x3(Bitmap b)
        {
            ConvMatrix blur = new ConvMatrix();
            blur.BottomLeft = 1;
            blur.BottomMid = 2;
            blur.BottomRight = 1;
            blur.MidLeft = 2;
            blur.Pixel = 4;
            blur.MidRight = 2;
            blur.TopLeft = 1;
            blur.TopMid = 2;
            blur.TopRight = 1;
            blur.Factor = 16;
            blur.Offset = 0;
            Conv3x3(b, blur);
            return true;
        }
        public static Bitmap borders(Bitmap b)
        {
            // GDI+ still lies to us - the return format is BGR, NOT RGB. 
            //BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
            //   ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            Bitmap outb = new Bitmap(b.Width, b.Height, PixelFormat.Format8bppIndexed);

            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
                    ImageLockMode.ReadWrite, b.PixelFormat);
            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;
            int nbands = 3;
            if (b.PixelFormat == PixelFormat.Format24bppRgb)
                nbands = 3;
            if (b.PixelFormat == PixelFormat.Format8bppIndexed)
                nbands = 1;

            BitmapData bmDataOut = outb.LockBits(new Rectangle(0, 0, outb.Width, outb.Height),
                    ImageLockMode.ReadWrite, outb.PixelFormat);
            int strideOut = bmDataOut.Stride;
            System.IntPtr OutScan0 = bmDataOut.Scan0;


            int c;
            try
            {
                unsafe
                {
                    byte* p = (byte*)(void*)Scan0;
                    byte* pout = (byte*)(void*)OutScan0;
                    int nOffset = stride - b.Width * nbands;
                    int nWidth = b.Width * nbands;
                    int nOffsetOut = strideOut - outb.Width;
                    for (int y = 1; y < b.Height - 1; ++y)
                    {
                        for (int x = 1; x < nWidth - 1; ++x)
                        {
                            int apos = y * stride + x;
                            if ((p[apos - 1] > p[apos]) ||
                                (p[apos + 1] > p[apos]) ||
                                (p[apos - stride - 1] > p[apos]) ||
                                (p[apos - stride + 1] > p[apos]) ||
                                (p[apos - stride] > p[apos]) ||
                                (p[apos + stride - 1] > p[apos]) ||
                                (p[apos + stride] > p[apos]) ||
                                (p[apos + stride + 1] > p[apos]))
                                c = 255;
                            else
                                c = 0;
                            pout[apos] = (byte)c;
                            //p[0] = (byte)(255 - p[0]);
                            //++p;
                            //++pout;
                        }
                        //p += nOffset;
                        //pout += nOffsetOut;
                    }
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("error escribiendo" + e.Message + " bandas " + nbands);
                return null;
            }
            b.UnlockBits(bmData);
            outb.UnlockBits(bmDataOut);
            return outb;
        }
        public static ArrayList FindContours(Bitmap b_out)
        {
            if (b_out == null)
            {

                return null;
            }
            //Bitmap b_out = new Bitmap(b.Width, b.Height, PixelFormat.Format8bppIndexed);
            BitmapData bmData_out = b_out.LockBits(new Rectangle(0, 0, b_out.Width, b_out.Height),
                    ImageLockMode.ReadWrite, b_out.PixelFormat);
            int stride_out = bmData_out.Stride;

            System.IntPtr ScanOut = bmData_out.Scan0;

            try
            {

                ArrayList list = new ArrayList();
                unsafe
                {
                    byte* p_out = (byte*)(void*)ScanOut;
                    int nOffset_out = stride_out - b_out.Width;
                    int nWidth_out = b_out.Width * 3;
                    int pos = 0;
                    int i, top, bot;
                    for (int y = 1; y < b_out.Height - 1; y++)
                    {
                        for (int x = 1; x < b_out.Width - 1; x++)
                        {
                            i = x + y * bmData_out.Stride;
                            top = x + (y - 1) * bmData_out.Stride;
                            bot = x + (y + 1) * bmData_out.Stride;

                            if (p_out[i] == 255)
                            {
                                pos = i;
                                ArrayList tramo = new ArrayList();
                                Point orig = new Point();
                                orig.X = x;
                                orig.Y = y;
                                tramo.Add(orig);
                                while (p_out[pos] == 255)
                                {
                                    Point pto = new Point();

                                    p_out[pos] = 0;
                                    if (p_out[pos + 1] == 255)
                                    {
                                        pos = pos + 1;
                                        orig.X += 1;
                                        pto = orig;
                                        tramo.Add(pto);
                                    }
                                    else if (p_out[pos + 1 + bmData_out.Stride] == 255)
                                    {
                                        pos = pos + 1 + bmData_out.Stride;
                                        orig.X += 1;
                                        orig.Y += 1;
                                        pto = orig;
                                        tramo.Add(pto);
                                    }
                                    else if (p_out[pos + bmData_out.Stride] == 255)
                                    {
                                        pos = pos + bmData_out.Stride;
                                        //orig.X -= 0;
                                        orig.Y += 1;
                                        pto = orig;
                                        tramo.Add(pto);
                                    }
                                    else if (p_out[pos - 1 + bmData_out.Stride] == 255)
                                    {
                                        pos = pos - 1 + bmData_out.Stride;
                                        orig.X -= 1;
                                        orig.Y += 1;
                                        pto = orig;
                                        tramo.Add(pto);
                                    }
                                    else if (p_out[pos - 1] == 255)
                                    {
                                        pos = pos - 1;
                                        orig.X -= 1;
                                        //orig.Y -= 0;
                                        pto = orig;
                                        tramo.Add(pto);
                                    }
                                    else if (p_out[pos - 1 - bmData_out.Stride] == 255)
                                    {
                                        pos = pos - 1 - bmData_out.Stride;
                                        orig.X -= 1;
                                        orig.Y -= 1;
                                        pto = orig;
                                        tramo.Add(pto);
                                    }
                                    else if (p_out[pos - bmData_out.Stride] == 255)
                                    {
                                        pos = pos - bmData_out.Stride;
                                        //orig.X -= 0;
                                        orig.Y -= 1;
                                        pto = orig;
                                        tramo.Add(pto);
                                    }
                                    else if (p_out[pos + 1 - bmData_out.Stride] == 255)
                                    {
                                        pos = pos + 1 - bmData_out.Stride;
                                        orig.X += 1;
                                        orig.Y -= 1;
                                        pto = orig;
                                        tramo.Add(pto);
                                    }
                                }
                                if (tramo.Count > 1)
                                {
                                    list.Add(tramo);
                                }
                            }

                        }//for x
                    }//for y

                }//unsafe
                //int oldn = 0;
                //ArrayList contourlist = new ArrayList(); 
                //foreach (ArrayList t in list)
                //{
                //    Point p = new Point();
                //    ArrayList contour = new ArrayList();
                //    foreach (int n in t)
                //    {

                //        switch(n){
                //            case 0:
                //                p.X+=1;
                //                break;
                //            case 1:
                //                p.X+=1;
                //                p.Y+=1;
                //                break;
                //            case 2:
                //                p.X+=0;
                //                p.Y+=1;
                //                break;
                //            case 3:
                //                p.X-=1;
                //                p.Y+=1;
                //                break;
                //            case 4:
                //                p.X-=1;
                //                p.Y+=0;
                //                break;
                //            case 5:
                //                p.X-=1;
                //                p.Y-=1;
                //                break;
                //            case 6:
                //                p.X+=0;
                //                p.Y-=1;
                //                break;
                //            case 7:
                //                p.X+=1;
                //                p.Y-=1;
                //                break;
                //        }
                //        if (n != oldn)
                //        {
                //            contour.Add(p);
                //            oldn = n;
                //            p= new Point() ;
                //        }
                //    }
                //    contourlist.Add(contour);

                //}
                b_out.UnlockBits(bmData_out);
                return list;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Error: " + e.Message);
                return null;
            }
        }
        public static Bitmap Scale(Bitmap b, int newheight, int newwidth)
        {
            // GDI+ still lies to us - the return format is BGR, NOT RGB. 
            //BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
            //   ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            Bitmap outb = new Bitmap(newwidth, newheight, b.PixelFormat);

            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
                    ImageLockMode.ReadWrite, b.PixelFormat);
            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;
            int nbands = 3;
            if (b.PixelFormat == PixelFormat.Format24bppRgb)
                nbands = 3;
            if (b.PixelFormat == PixelFormat.Format8bppIndexed)
                nbands = 1;

            BitmapData bmDataOut = outb.LockBits(new Rectangle(0, 0, outb.Width, outb.Height),
                    ImageLockMode.ReadWrite, outb.PixelFormat);
            int strideOut = bmDataOut.Stride;
            System.IntPtr OutScan0 = bmDataOut.Scan0;
            int nbandsOut = 3;
            if (outb.PixelFormat == PixelFormat.Format24bppRgb)
                nbandsOut = 3;
            if (outb.PixelFormat == PixelFormat.Format8bppIndexed)
                nbandsOut = 1;

            int c;
            try
            {
                unsafe
                {
                    byte* p = (byte*)(void*)Scan0;
                    byte* pout = (byte*)(void*)OutScan0;
                    int nOffset = stride - b.Width * nbands;
                    int nWidth = b.Width * nbands;
                    int nOffsetOut = strideOut - outb.Width;
                    for (int y = 1; y < outb.Height - 1; ++y)
                    {
                        for (int x = 1; x < outb.Width - 1; ++x)
                        {
                            int apos = y * strideOut + x * nbandsOut;
                            int oldpos = ((y * b.Height / outb.Height) * stride + (x * b.Width / outb.Width) * nbands);
                            for (int band = 0; band < nbandsOut; band++)
                            {
                                pout[apos + band] = p[oldpos + band];
                            }
                            //p[0] = (byte)(255 - p[0]);
                            //++p;
                            //++pout;
                        }
                        //p += nOffset;
                        //pout += nOffsetOut;
                    }
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("error escribiendo" + e.Message + " bandas " + nbands);
                return null;
            }
            b.UnlockBits(bmData);
            outb.UnlockBits(bmDataOut);
            return outb;
        }
        public static Bitmap Mosaic4(Bitmap q, Bitmap r, Bitmap s, Bitmap t)
        {
            // GDI+ still lies to us - the return format is BGR, NOT RGB. 
            //BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
            //   ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            Bitmap outb = new Bitmap(  q.Width +r.Width ,q.Height +t.Height , q.PixelFormat);
            BitmapData qmData = q.LockBits(new Rectangle(0, 0, q.Width, q.Height),
                    ImageLockMode.ReadWrite, q.PixelFormat);
            int strideq = qmData.Stride;
            System.IntPtr Scanq = qmData.Scan0;
            
            BitmapData rmData = r.LockBits(new Rectangle(0, 0,r.Width, r.Height),
                    ImageLockMode.ReadWrite, r.PixelFormat);
            int strider = rmData.Stride;
            System.IntPtr Scanr = rmData.Scan0;
            
            BitmapData smData = s.LockBits(new Rectangle(0, 0, s.Width, s.Height),
                    ImageLockMode.ReadWrite, s.PixelFormat);
            int strides = smData.Stride;
            System.IntPtr Scans = smData.Scan0;
            
            BitmapData tmData = t.LockBits(new Rectangle(0, 0, t.Width, t.Height),
                    ImageLockMode.ReadWrite, t.PixelFormat);
            int stridet = tmData.Stride;
            System.IntPtr Scant = tmData.Scan0;

            int qbands = 3;
            if (q.PixelFormat == PixelFormat.Format24bppRgb)
                qbands = 3;
            if (q.PixelFormat == PixelFormat.Format32bppArgb)
                qbands = 4;
            if (q.PixelFormat == PixelFormat.Format8bppIndexed)
                qbands = 1;
            int rbands = 3;
            if (r.PixelFormat == PixelFormat.Format24bppRgb)
                rbands = 3;
            if (r.PixelFormat == PixelFormat.Format32bppArgb)
                rbands = 4;
            if (r.PixelFormat == PixelFormat.Format8bppIndexed)
                rbands = 1;
            int sbands = 3;
            if (s.PixelFormat == PixelFormat.Format24bppRgb)
                sbands = 3;
            if (s.PixelFormat == PixelFormat.Format32bppArgb)
                sbands = 4;
            if (s.PixelFormat == PixelFormat.Format8bppIndexed)
                sbands = 1;
            int tbands = 3;
            if (t.PixelFormat == PixelFormat.Format24bppRgb)
                tbands = 3;
            if (t.PixelFormat == PixelFormat.Format32bppArgb)
                tbands = 4;
            if (t.PixelFormat == PixelFormat.Format8bppIndexed)
                tbands = 1;

            BitmapData bmDataOut = outb.LockBits(new Rectangle(0, 0, outb.Width, outb.Height),
                    ImageLockMode.ReadWrite, outb.PixelFormat);
            int strideOut = bmDataOut.Stride;
            System.IntPtr OutScan0 = bmDataOut.Scan0;
            int nbandsOut = 3;
            if (outb.PixelFormat == PixelFormat.Format24bppRgb)
                nbandsOut = 3;
            if (outb.PixelFormat == PixelFormat.Format32bppArgb )
                nbandsOut = 4;
            if (outb.PixelFormat == PixelFormat.Format8bppIndexed)
                nbandsOut = 1;

            int c;
            int nx, ny;
            try
            {
                unsafe
                {
                    byte* pq = (byte*)(void*)Scanq;
                    byte* pr = (byte*)(void*)Scanr;
                    byte* ps = (byte*)(void*)Scans;
                    byte* pt = (byte*)(void*)Scant;
                    byte* pout = (byte*)(void*)OutScan0;
                    //int nOffset = strideq - q.Width * qbands;
                    //int nWidth = q.Width * qbands;
                    //int nOffsetOut = strideOut - outb.Width;
                    for (int y = 0; y < q.Height ; ++y)

                    {
                        for (int x = 0; x < q.Width; ++x)
                        {

                            int _qpos = y * strideq + x * qbands;
                            int _rpos = y * strider + x * rbands;
                            int _spos = y * strides + x * sbands;
                            int _tpos = y * stridet + x * tbands;

                            int qpos = y * strideOut + x * nbandsOut;
                            int rpos = y * strideOut + (x+q.Width ) * nbandsOut;
                            int spos = (y+q.Height ) * strideOut + (x+q.Width ) * nbandsOut;
                            int tpos = ((y+q.Height-1  ) * strideOut) + (x * nbandsOut);
                            for (int band = 0; band < nbandsOut; band++)
                            {
                                if (band < qbands)
                                    pout[qpos + band] = pq[_qpos + band];
                                else
                                    pout[qpos + band] = 255;

                                if (band < rbands ) 
                                    pout[rpos + band] = pr[_rpos + band];
                                else
                                    pout[rpos + band] = 255;

                                if (band < sbands) 
                                    pout[spos + band] = ps[_spos + band];
                                else
                                    pout[spos + band] = 255;

                                if (band < tbands) 
                                    pout[tpos + band] = pt[_tpos + band];
                                else
                                    pout[tpos + band] = 255;

                                
                            }
                            //p[0] = (byte)(255 - p[0]);
                            //++p;
                            //++pout;
                        }
                        //p += nOffset;
                        //pout += nOffsetOut;
                    }
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("error escribiendo" + e.Message + " bandas " + qbands + " bandas " + rbands + " bandas " + sbands + " bandas " + tbands);
                return null;
            }
            q.UnlockBits(qmData);
            r.UnlockBits(rmData);
            s.UnlockBits(smData);
            t.UnlockBits(tmData);
            outb.UnlockBits(bmDataOut);
            return outb;
        }
        public static bool Copy(Bitmap orig, Bitmap dest, int xx, int yy)
        {
            if (orig == null)
                return false;
            if (dest == null)
                return false;
            //must check if orig fits in dest at postion x y
            BitmapData origmData = orig.LockBits(new Rectangle(0, 0, orig.Width, orig.Height),
                                               ImageLockMode.ReadWrite, orig.PixelFormat);
            int origstride = origmData.Stride;
            System.IntPtr origScan0 = origmData.Scan0;
            int orignbands = 3;
            if (orig.PixelFormat == PixelFormat.Format24bppRgb)
                orignbands = 3;
            if (orig.PixelFormat == PixelFormat.Format8bppIndexed)
                orignbands = 1;

            BitmapData amData = dest.LockBits(new Rectangle(0, 0, dest.Width, dest.Height),
                                                ImageLockMode.ReadWrite, dest.PixelFormat);
            int astride = amData.Stride;
            System.IntPtr aScan0 = amData.Scan0;
            int anbands = 3;
            if (dest.PixelFormat == PixelFormat.Format24bppRgb)
                anbands = 3;
            if (dest.PixelFormat == PixelFormat.Format8bppIndexed)
                anbands = 1;
            try
            {
                unsafe
                {
                    byte* origp = (byte*)(void*)origScan0;
                    byte* ap = (byte*)(void*)aScan0;
                    for (int y = 0; y < orig.Height; ++y)
                    {
                        for (int x = 0; x < orig.Width; ++x)
                        {
                            int apos = (y + yy) * astride + (x + xx) * anbands;
                            int origpos = y * origstride + x * orignbands;
                            for (int band = 0; band < orignbands; band++)
                            {
                                ap[apos + band] = (byte)(origp[origpos + band]);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("error escribiendo" + e.Message);
                return false;
            }
            orig.UnlockBits(origmData);
            dest.UnlockBits(amData);
            return true;
        }
    }//endclass

}//end namespace
