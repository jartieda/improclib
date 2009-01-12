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
            ImProcLib.ImProc.ComplexImage cimg = ImProcLib.ImProc.ForwardFFT(c);
            double factor;
            Bitmap gaus = ImProcLib.ImProc.Gausian(16, 2, out factor);
            Bitmap biggaus = new Bitmap(c.Width, c.Height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
            ImProcLib.ImProc.Copy(gaus, biggaus, (int)(c.Width / 2.0 - (gaus.Width / 2.0)), (int)(c.Height / 2.0 - (gaus.Height / 2.0)));
            ImProcLib.ImProc.ComplexImage cimaggaus = ImProcLib.ImProc.ForwardFFT(biggaus);
            ImProcLib.ImProc.ComplexImage multiplicado = new ImProcLib.ImProc.ComplexImage(cimaggaus.Width, cimaggaus.Height);
            for (int i = 0; i < cimaggaus.Width * cimaggaus.Height; i++)
            {
                multiplicado.data[i] = cimaggaus.data[i] * cimg.data[i];
            }
            Bitmap filtrad = ImProcLib.ImProc.InverseFFT(multiplicado);
            // check palette
            System.Drawing.Imaging. ColorPalette cp = biggaus.Palette;
            // init palette
            for (int i = 0; i < 256; i++)
            {
                cp.Entries[i] = Color.FromArgb(i, i, i);
            }
            filtrad.Palette = cp;
            Bitmap bmult = ImProcLib.ImProc.Complex2Bitmap(multiplicado);
            bmult.Palette = cp;
            pictureBox1.Image =bmult;
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog f = new System.Windows.Forms.OpenFileDialog();
            f.ShowDialog();
            Bitmap b = ImProcLib.ImProc.LoadImage(f.FileName);
           
            // check palette
            System.Drawing.Imaging.ColorPalette cp = b.Palette;
            // init palette
            for (int i = 0; i < 256; i++)
            {
                cp.Entries[i] = Color.FromArgb(255-i, i, i);
            }
            b.Palette = cp;
            b.Save("c:\\kk.bmp"  );
            pictureBox1.Image = b;
        }
    }
}
