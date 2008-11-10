using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog f = new System.Windows.Forms.OpenFileDialog();
            f.ShowDialog ();
            Bitmap b = ImProcLib.ImProc .LoadImage (f.FileName );
            ImProcLib.ImProc.Smooth(b);
            ImProcLib.ImProc.Reclass(b,10);
            Bitmap b_out =ImProcLib.ImProc.borders(b);
            Bitmap c = (Bitmap)b_out.Clone();
            ArrayList l=ImProcLib.ImProc .FindContours (c);
           
            pictureBox1.Image = b_out ;
            
        }

        private void button2_Click(object sender, EventArgs e)
        {

            System.Windows.Forms.OpenFileDialog f = new System.Windows.Forms.OpenFileDialog();
            f.ShowDialog();
            Bitmap b = ImProcLib.ImProc.LoadImage(f.FileName);
            Bitmap c = ImProcLib.ImProc.padToNext2Power(b);
            //Bitmap c = ImProcLib.ImProc.Scale(b, 1024, 1024);
            Bitmap b_out = ImProcLib.ImProc.ForwardFFT(c);
            double factor;
            Bitmap gaus = ImProcLib.ImProc.Gausian(16, 2, out factor);
            Bitmap biggaus = new Bitmap(b_out.Width, b_out.Height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
            ImProcLib.ImProc.Copy(gaus, biggaus, (int)(b_out.Width / 2.0 - (gaus.Width / 2.0)), (int)(b_out.Height / 2.0 - (gaus.Height / 2.0)));
            
            Bitmap fftgaus = ImProcLib.ImProc.ForwardFFT(biggaus);
            // check palette
            System.Drawing.Imaging. ColorPalette cp = fftgaus.Palette;
            System.Drawing.Color  cc;
            // init palette
            for (int i = 0; i < 256; i++)
            {
                cp.Entries[i] = Color.FromArgb(i, i, i);
            }

            fftgaus.Palette = cp;

            pictureBox1.Image = fftgaus;
            
            
        }
    }
}
