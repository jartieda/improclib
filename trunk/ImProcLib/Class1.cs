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
using System.Windows.Forms ;
using System.IO;
using Exocortex.DSP;


namespace ImProcLib
{
    public class ImProc
    {
           
        
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

        public Bitmap ForwardFFT(Bitmap b)
        {
            if (b == null)
            {
                return null;
            }
            Bitmap b_out = new Bitmap(b.Width , b.Height , PixelFormat.Format16bppGrayScale);

            float scale = 1f / (float)Math.Sqrt(b.Width * b.Height);
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
                    ComplexF[] data = new ComplexF[b.Width * b.Height];
                    int i;
                    for (int y = 0; y < b_out.Height; ++y)
                    {
                        for (int x = 0; x < b_out.Width; ++x)
                        {
                            i = x + y * bmData.Stride;
                            if (nbands == 1) data[i ].Re = p[i];
                            else
                            {
                                Color c = Color.FromArgb(p[i]);
                                data[i].Re = ((float)c.R + (float)c.G + (float)c.B) / (3f * 256f);
                            }
                        }
                    }
                    b.UnlockBits(bmData);
                
                    int offset = 0;
                    for (int y = 0; y < b.Height; y++)
                    {
                        for (int x = 0; x < b.Width; x++)
                        {
                            if (((x + y) & 0x1) != 0)
                            {
                                data[offset] *= -1;
                            }
                            offset++;
                        }
                    }
                    
                    Fourier.FFT2(data, b.Width, b.Height, FourierDirection.Forward);

                //  cimage.FrequencySpace = true;

                    for ( i = 0; i < data.Length; i++)
                    {
                        data[i] *= scale;
                    }

                    
                    for (int y = 0; y < b_out.Height; ++y)
                    {
                        for (int x = 0; x < b_out.Width; ++x)
                        {
                            i = x + y *bmData_out.Stride ;
                            p_out[i] = (byte) Math.Sqrt (data[i].Re * data[i].Re + data[i].Im + data[i].Im);
                        }
                    }
                    b_out.UnlockBits(bmData_out);
                }
                return b_out;
            }
            catch (Exception e)
            {
                return null;
            }
        }

    }//endclass

}//end namespace
