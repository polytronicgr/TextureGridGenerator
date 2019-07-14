using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace TextureGridGenerator
{
    public partial class Form1 : Form
    {
        public int resolution = 512;
        public Bitmap grid = new Bitmap(256, 256);
        Pen blackPen = new Pen(Color.Black, 1);
        Pen greyPen = new Pen(Color.LightGray, 0.25f);
        SolidBrush alphaBright = new SolidBrush(Color.FromArgb(64, 200, 200, 200));
        SolidBrush alphaDark = new SolidBrush(Color.FromArgb(64, 50, 50, 50));

        public Form1()
        {
            InitializeComponent();
        }

        public List<PointF> getCorners(RectangleF r)
        {
            return new List<PointF>() { r.Location, new PointF(r.Right, r.Top),
            new PointF(r.Right, r.Bottom), new PointF(r.Left, r.Bottom)};
        }

        public static Color medianColor(List<Color> cols)
        {
            int c = cols.Count;
            return Color.FromArgb(cols.Sum(x => x.A) / c, cols.Sum(x => x.R) / c,
                cols.Sum(x => x.G) / c, cols.Sum(x => x.B) / c);
        }

        public void generate(int resolution, bool color, bool smallGrid, bool checker)
        {
            grid.Dispose();
            grid = new Bitmap(resolution, resolution);
            Graphics gfx = Graphics.FromImage(grid);
            SolidBrush white = new SolidBrush(Color.White);
            SolidBrush black = new SolidBrush(Color.Black);
            if (color)
            {
                Rectangle r = new Rectangle(0, 0, resolution, resolution);
                List<Color> colors = new List<Color> { Color.FromArgb(255, 255, 0, 0), Color.FromArgb(255, 0, 255, 0), Color.FromArgb(255, 0, 255, 255), Color.FromArgb(255, 255, 0, 255) };
                for (int y = 0; y < r.Height; y++)
                {

                    using (PathGradientBrush pgb = new PathGradientBrush(getCorners(r).ToArray()))
                    {
                        pgb.CenterColor = medianColor(colors);
                        pgb.SurroundColors = colors.ToArray();
                        gfx.FillRectangle(pgb, 0, y, r.Width, 1);
                    }
                }
            }
            else
            {
                gfx.FillRectangle(white, 0, 0, resolution, resolution);
            }

            if (checker)
            {
                int part = resolution / 10;
                Size size = new Size(part, part);
                for(int i = 0; i <= 9; i++)
                {
                    for(int j = 0; j <= 9; j++)
                    {
                        Point start = new Point(i * part, j*part);
                        Rectangle rec = new Rectangle(start, size);
                        if(j % 2 == 0)
                        {
                            if (i % 2 == 0)
                            {
                                gfx.FillRectangle(alphaBright, rec);
                            }
                            else
                            {
                                gfx.FillRectangle(alphaDark, rec);
                            }
                        }
                        else
                        {
                            if (i % 2 == 1)
                            {
                                gfx.FillRectangle(alphaBright, rec);
                            }
                            else
                            {
                                gfx.FillRectangle(alphaDark, rec);
                            }
                        }
                    }
                }
            }

            if (smallGrid)
            {
                for (int j = 0; j <= 9; j++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        gfx.DrawLine(greyPen, ((resolution / 50) + (i * (resolution / 50))) + j * (resolution / 10), 0, ((resolution / 50) + (i * (resolution / 50))) + j * (resolution / 10), resolution);
                        gfx.DrawLine(greyPen, 0, ((resolution / 50) + (i * (resolution / 50))) + j * (resolution / 10), resolution, ((resolution / 50) + (i * (resolution / 50))) + j * (resolution / 10));
                    }
                }
            }

            for (int i = 0; i < 9; i++)
            {
                gfx.DrawLine(blackPen, (resolution / 10) + (i * (resolution / 10)), 0, (resolution / 10) + (i * (resolution / 10)), resolution);
                gfx.DrawLine(blackPen, 0, (resolution / 10) + (i * (resolution / 10)), resolution, (resolution / 10) + (i * (resolution / 10)));
            }

            gfx.DrawString("0,0", new Font(FontFamily.GenericSansSerif, resolution / 30), black, resolution / 80, resolution / 50);
            gfx.DrawString("1,0", new Font(FontFamily.GenericSansSerif, resolution / 30), black, resolution - (resolution / 11), resolution / 50);
            gfx.DrawString("0,1", new Font(FontFamily.GenericSansSerif, resolution / 30), black, resolution / 80, resolution - (resolution / 12));
            gfx.DrawString("1,1", new Font(FontFamily.GenericSansSerif, resolution / 30), black, resolution - (resolution / 11), resolution - (resolution / 12));

            for (int i = 0; i <= 9; i++)
            {
                for (int j = 0; j <= 9; j++)
                {
                    gfx.DrawString("0." + i, new Font(FontFamily.GenericSansSerif, resolution / 50), black, i * (resolution / 10), j * (resolution / 10));
                }
            }

            for (int i = 0; i <= 9; i++)
            {
                for (int j = 0; j <= 9; j++)
                {
                    gfx.DrawString("0." + i, new Font(FontFamily.GenericSansSerif, resolution / 50), black, j * (resolution / 10) + (resolution / 18), i * (resolution / 10) + (resolution / 15));
                }
            }
            gfx.Dispose();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            generate(resolution, colorCheckBox.Checked, smallGridCheckbox.Checked, checkerCheckBox.Checked);

            pictureBox1.BackgroundImage = (Image)grid;
        }

        private void TrackBar1_Scroll(object sender, EventArgs e)
        {
            switch (trackBar1.Value)
            {
                case 1:
                    resolutionLabel.Text = "512x512";
                    resolution = 512;
                    blackPen.Width = 1;
                    greyPen.Width = 0.25f;
                    break;
                case 2:
                    resolutionLabel.Text = "1024x1024";
                    resolution = 1024;
                    blackPen.Width = 2;
                    greyPen.Width = 0.5f;
                    break;
                case 3:
                    resolutionLabel.Text = "2048x2048";
                    resolution = 2048;
                    blackPen.Width = 4;
                    greyPen.Width = 1;
                    break;
                case 4:
                    resolutionLabel.Text = "4096x4096";
                    resolution = 4096;
                    blackPen.Width = 8;
                    greyPen.Width = 2;
                    break;
                case 5:
                    resolutionLabel.Text = "8192x8192";
                    resolution = 8192;
                    blackPen.Width = 16;
                    greyPen.Width = 4;
                    break;
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PNG image|*.png|JPG image|*.jpg";
            if(sfd.ShowDialog() == DialogResult.OK)
            {
                if (sfd.FileName.EndsWith(".png"))
                {
                    grid.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Png);
                }  else if (sfd.FileName.EndsWith(".jpg"))
                {
                    grid.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();  
            if(fbd.ShowDialog() == DialogResult.OK)
            {
                string basePath = fbd.SelectedPath;

                float bl = blackPen.Width;
                float gr = greyPen.Width;
                
                for(int i = 512; i <= 8192; i *= 2)
                {
                    blackPen.Width = i / 512.0f;
                    greyPen.Width = i / 2048.0f;
                    for(int c = 0; c <= 1; c++)
                    {
                        for(int g = 0; g <= 1; g++)
                        {
                            for(int ch = 0; ch <= 1; ch++)
                            {
                                generate(i, c == 1, g == 1, ch == 1);
                                string name = "\\";
                                if (c == 0)
                                    name += "bw_";
                                else
                                    name += "color_";

                                if (g == 1)
                                    name += "grid_";

                                if (ch == 1)
                                    name += "checker_";

                                name += i;
                                name += ".png";
                                grid.Save(basePath + name, System.Drawing.Imaging.ImageFormat.Png);
                            }
                        }
                    }
                }

                blackPen.Width = bl;
                greyPen.Width = gr;
            }
        }
    }
}
