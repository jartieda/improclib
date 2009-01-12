using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
namespace ImProcLib
{
    public class ImageWriter
    {
        public Bitmap img;
        public Stream strm;
        public Rectangle roi;
        private int cursor;
        public ImageWriter(Bitmap b, Stream s)
        {
            img = b;
            roi = new Rectangle(0, 0, img.Width, img.Height);
            strm = s;
        }
        public ImageWriter(Bitmap b)
        {
            img = b;
            roi = new Rectangle(0, 0, img.Width, img.Height);
        }

        public bool Write(int Lngth)
        {
            // GDI+ still lies to us - the return format is BGR, NOT RGB. 
            // BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
            // ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bmData = img.LockBits(new Rectangle(0, 0, img.Width, img.Height),
                    ImageLockMode.ReadWrite, img.PixelFormat);
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
                    for (int x = roi.Left; x < roi.Right; x++)
                    {
                        for (int y = roi.Top; y < roi.Bottom; y++)
                        {
                            if (cursor < maxlength)
                            {
                                //strm.WriteByte(p[cursor]);
                                p[cursor] = (byte) strm.ReadByte();
                                cursor++;
                                Lngth--;
                                if (Lngth < 1)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                break;
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

        public bool Write(byte[] data)
        {
            // GDI+ still lies to us - the return format is BGR, NOT RGB. 
            // BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
            // ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bmData = img.LockBits(new Rectangle(0, 0, img.Width, img.Height),
                    ImageLockMode.ReadWrite, img.PixelFormat);
            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;
            int nbands = 3;
            if (img.PixelFormat == PixelFormat.Format24bppRgb)
                nbands = 3;
            if (img.PixelFormat == PixelFormat.Format8bppIndexed)
                nbands = 1;
            int maxlength;
            int Lngth = 0;
            maxlength = img.Width * img.Height * nbands;
            try
            {
                unsafe
                {
                    byte* p = (byte*)(void*)Scan0;
                    for (int x = 0 ; x<data.Length ; x++)
                    {
                            if (cursor < maxlength)
                            {
                                //strm.WriteByte(p[cursor]);
                                p[cursor] = data[x ];
                                cursor++;
                            }
                            else
                            {
                                break;
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
