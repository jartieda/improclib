using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO ;
namespace ImProcLib
{
    public class ImageReader
    {
        public Bitmap img;
        public Stream strm;
        public Rectangle roi;
        private int cursor;
        public ImageReader(Bitmap b, Stream s)
        {
            img = b;
            strm = s;
        }

        public bool  Read(int Lngth)
        {
            // GDI+ still lies to us - the return format is BGR, NOT RGB. 
            // BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
            // ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bmData = img.LockBits(new Rectangle(0, 0, img.Width, img.Height),
                    ImageLockMode.ReadOnly, img.PixelFormat);
            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;
            int nbands = 3;
            if (img.PixelFormat == PixelFormat.Format24bppRgb)
                nbands = 3;
            if (img.PixelFormat == PixelFormat.Format8bppIndexed)
                nbands = 1;
            int maxlength;
            maxlength = img.Width * img.Height * nbands;
            try
            {
                unsafe
                {
                    byte* p = (byte*)(void*)Scan0;
                    
                        for (int y = roi.Top; y < roi.Bottom; y++)
                        {
                          for (int x = roi.Left; x < roi.Right; x++)
                           {
                               for (int bnd = 0; bnd < nbands; bnd++)
                               {
                                   cursor = y * stride + x*nbands + bnd ;
                                   strm.WriteByte(p[cursor]);
                                   Lngth--;
                                   if (Lngth < 1)
                                   {
                                       break;
                                   }
                               }
                           }
                        }
                }

            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("error escribiendo" + e.Message + " bandas " + nbands);
                return false;
            }

            img.UnlockBits(bmData);

            return true;
        }
    }
}
