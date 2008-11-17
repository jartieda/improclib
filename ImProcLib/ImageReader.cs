using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO ;
namespace ImProcLib
{
    class ImageReader
    {
        public Bitmap img;
        public Stream strm;
        public Rectangle roi;
        private int cursor;
        ImageReader(Bitmap b, Stream s)
        {
            img = b;
            strm = s;
        }

        void Read(int Lngth)
        {
            // GDI+ still lies to us - the return format is BGR, NOT RGB. 
            // BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
            // ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
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
    }
}
