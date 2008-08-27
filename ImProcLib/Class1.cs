
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
       
//        /*Replaces data by its ndim-dimensional discrete Fourier transform, if isign is input as 1.
//nn[1..ndim] is an integer array containing the lengths of each dimension (number of complex
//values), which MUST all be powers of 2. data is a real array of length twice the product of
//these lengths, in which the data are stored as in a multidimensional complex array: real and
//imaginary parts of each element are in consecutive locations, and the rightmost index of the
//array increases most rapidly as one proceeds along data. For a two-dimensional array, this is
//equivalent to storing the array by rows. If isign is input as −1, data is replaced by its inverse
//transform times the product of the lengths of all dimensions.*/
//    public void fourn(float[,] data, UInt64[] nn, int ndim, int isign)
//    {

//        int idim;

//        UInt64  i1,i2,i3,i2rev,i3rev,ip1,ip2,ip3,ifp1,ifp2;
//        UInt64 ibit,k1,k2,n,nprev,nrem,ntot;
//        float tempi,tempr;
//        double theta,wi,wpi,wpr,wr,wtemp; //Double precision for trigonometric recurrences.
//        for (ntot=1,idim=1;idim<=ndim;idim++) //Compute total number of complex values
//                ntot*= nn[idim]; 

//        nprev=1;

//        for (idim=ndim;idim>=1;idim--) {// Main loop over the dimensions.
//            n=nn[idim];//number of cells in dimension = idim
//            nrem=ntot/(n*nprev);//total of vertices div by vertices in this dim by an inc n 
//            ip1=nprev << 1;//2
//            ip2=ip1*n;//
//            ip3=ip2*nrem;
//            i2rev=1;
//            //This is the bit-reversal section of theroutine.
            
//            for (i2 = 0; i2 < (uint ) data.GetUpperBound(1); i2 += 2)
//            {
//                i2rev = 0;
//                for (ibit = 0; ibit < (uint)data.GetUpperBound(1) / 2; ibit++)
//                {
//                    i2rev = i2rev + (i2 >>(int) ibit % 2) * 2 * (8 - ibit);
//                }
//                if (i2rev > i2)
//                {
//                    for (i1 = 0; i1 < (uint)data.GetUpperBound(0); i1++)
//                    {
//                        tempr = data[i1, i2]; data[i1, i2] = data[i1, i2rev]; data[i1, i2rev] = tempr;//SWAP(data[i3], data[i3rev]);
//                        tempr = data[i1, i2 + 1]; data[i1, i2 + 1] = data[i1, i2rev + 1]; data[i1, i2rev + 1] = tempr;// SWAP(data[i3 + 1], data[i3rev + 1]);
//                    }
//                }
//            }
//            //Here begins the Danielson-Lanczos section of the routine.
//            ifp1=ip1; 
//            while(ifp1 < ip2) { 
//                ifp2=ifp1 << 1;
//                theta=isign*6.28318530717959/(ifp2/ip1);// Initialize for the trig. recurrence.
//                wtemp= Math.Sin(0.5*theta); 
//                wpr = -2.0*wtemp*wtemp;
//                wpi=Math.Sin(theta);
//                wr=1.0;
//                wi=0.0;
//                for (i3=1;i3<=ifp1;i3+=ip1) {
//                    for (i1=i3;i1<=i3+ip1-2;i1+=2) {
//                        for (i2=i1;i2<=ip3;i2+=ifp2) {
//                            k1=i2; //Danielson-Lanczos formula:
//                            k2=k1+ifp1;
//                            tempr=(float)wr*data[k2]-(float)wi*data[k2+1];
//                            tempi=(float)wr*data[k2+1]+(float)wi*data[k2];
//                            data[k2]=data[k1]-tempr;
//                            data[k2+1]=data[k1+1]-tempi;
//                            data[k1] += tempr;
//                            data[k1+1] += tempi;
//                        }
//                    }
//                    wr=(wtemp=wr)*wpr-wi*wpi+wr; //Trigonometric recurrence.
//                    wi=wi*wpr+wtemp*wpi+wi;
//                }
//                ifp1=ifp2;
//            }
//            nprev *= n;
//        }
//    }
//        /*Given a three-dimensional real array data[1..nn1][1..nn2][1..nn3] (where nn1 = 1 for
//the case of a logically two-dimensional array), this routine returns (for isign=1) the complex
//fast Fourier transform as two complex arrays: On output, data contains the zero and positive
//frequency values of the third frequency component, while speq[1..nn1][1..2*nn2] contains
//the Nyquist critical frequency values of the third frequency component. First (and second)
//frequency components are stored for zero, positive, and negative frequencies, in standard wraparound
//order. See text for description of how complex values are arranged. For isign=-1, the
//inverse transform (times nn1*nn2*nn3/2 as a constant multiplicative factor) is performed,
//with output data (viewed as a real array) deriving from input data (viewed as complex) and
//speq. For inverse transforms on data not generated 1rst by a forward transform, make sure
//the complex input data array satisfies property (12.5.2). The dimensions nn1, nn2, nn3 must
//always be integer powers of 2.*/
//        void fft(float[,,]data, float[,]speq, UInt64 nn1, UInt64 nn2,
//                UInt64 nn3, int isign)

//            {
//            UInt64 i1,i2,i3,j1,j2,j3,ii3;
//            UInt64[] nn = new UInt64[4];
//            double theta,wi,wpi,wpr,wr,wtemp;
//            double c1,c2,h1r,h1i,h2r,h2i;
//            //FIXME
//            //if (1+&data[nn1,nn2,nn3]-&data[1,1,1] != nn1*nn2*nn3)
//            //        Console.WriteLine ("rlft3: problem with dimensions or contiguity of data array\n");
//            c1=0.5;
//            c2 = -0.5*isign;
//            theta=isign*(6.28318530717959/nn3);
//            wtemp=Math.Sin(0.5*theta);
//            wpr = -2.0*wtemp*wtemp;
//            wpi=Math.Sin(theta);
//            nn[1]=nn1;
//            nn[2]=nn2;
//            nn[3]=nn3 >> 1;
//            if (isign == 1) { //Case of forward transform.
//                fourn(data,nn,3,isign); //Here is where most all of the compute time is spent.
//                for(i1=1;i1<=nn1;i1++) //
//                    for (i2=1,j2=0;i2<=nn2;i2++) { //Extend data periodically into speq.
//                        speq[i1,++j2]=data[i1,i2,1];
//                        speq[i1,++j2]=data[i1,i2,2];
//                    }
//            }
//            for (i1=1;i1<=nn1;i1++) {
//                j1=(i1 != 1 ? nn1-i1+2 : 1);
//                //Zero frequency is its own reflection, otherwise locate corresponding negative frequency
//                //in wrap-around order.
//                wr=1.0;// Initialize trigonometric recurrence.
//                wi=0.0;
//                for (ii3=1,i3=1;i3<=(nn3>>2)+1;i3++,ii3+=2) {
//                    for (i2=1;i2<=nn2;i2++) {
//                        if (i3 == 1) { //Equation (12.3.5).
//                            j2=(i2 != 1 ? ((nn2-i2)<<1)+3 : 1);
//                            h1r=c1*(data[i1,i2,1]+speq[j1,j2]);
//                            h1i=c1*(data[i1,i2,2]-speq[j1,j2+1]);
//                            h2i=c2*(data[i1,i2,1]-speq[j1,j2]);
//                            h2r= -c2*(data[i1,i2,2]+speq[j1,j2+1]);
//                            data[i1,i2,1]=(float)(h1r+h2r);
//                            data[i1,i2,2]=(float)(h1i+h2i);
//                            speq[j1,j2]=(float)(h1r-h2r);
//                            speq[j1,j2+1]=(float)(h2i-h1i);
//                        } else {
//                            j2=(i2 != 1 ? nn2-i2+2 : 1);
//                            j3=nn3+3-(i3<<1);
//                            h1r=c1*(data[i1,i2,ii3]+data[j1,j2,j3]);
//                            h1i=c1*(data[i1,i2,ii3+1]-data[j1,j2,j3+1]);
//                            h2i=c2*(data[i1,i2,ii3]-data[j1,j2,j3]);
//                            h2r= -c2*(data[i1,i2,ii3+1]+data[j1,j2,j3+1]);
//                            data[i1,i2,ii3]=(float)(h1r+wr*h2r-wi*h2i);
//                            data[i1,i2,ii3+1]=(float)(h1i+wr*h2i+wi*h2r);
//                            data[j1,j2,j3]=(float)(h1r-wr*h2r+wi*h2i);
//                            data[j1,j2,j3+1]=(float)( -h1i+wr*h2i+wi*h2r);
//                        }
//                    }
//                    wr=(wtemp=wr)*wpr-wi*wpi+wr; //Do the recurrence.
//                    wi=wi*wpr+wtemp*wpi+wi;
//                }
//            }
//            if (isign == -1) //Case of reverse transform.
//                fourn(data,nn,3,isign);
//        }
    }//endclass

}//end namespace
