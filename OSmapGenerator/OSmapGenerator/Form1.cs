using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace OSmapGenerator {
    public partial class Form1 : Form {
        Bitmap img = new Bitmap(100,100);
        int minHeight = 256, maxHeight = 0;
        Color colHeightLine = Color.Orange, colWood = Color.LimeGreen, colIndustry = Color.Gray, colEmpty = Color.White, colWater = Color.Blue;
        int[,] heights = new int[0,0];
        Graphics gfx;

        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            gfx = Graphics.FromImage(img);
            gfx.InterpolationMode = canvas.InterpolationMode;
            gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            gfx.Clear(Color.Black);
            gfx.DrawString("Open a file.", SystemFonts.DefaultFont, Brushes.Red, 5,5);
            canvas.Image = img;
        }

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e) {
            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                //Get the path of specified file
                string filePath = openFileDialog1.FileName;

                //Read the contents of the file
                Bitmap heightmap = new Bitmap(filePath);

                //process the image
                img = new Bitmap(heightmap);
                gfx = Graphics.FromImage(img);
                canvas.Image = img;
                heights = new int[heightmap.Width, heightmap.Height];
                for (int x = 0; x < heightmap.Width; x++) { 
                    for (int y = 0; y < heightmap.Height; y++) {
                        int val = (heightmap.GetPixel(x,y).R+heightmap.GetPixel(x,y).G+heightmap.GetPixel(x,y).B)/3;
                        img.SetPixel(x,y, Color.FromArgb(val,val,val));
                        heights[x, y] = val;
                        if (val < minHeight) { minHeight = val; }
                        if (val > maxHeight) { maxHeight = val; }
                    }
                }

            }

        }

        private void bttn_generateHeights_Click(object sender, EventArgs e) {
            bttn_generateHeights.Enabled = false;
            int gap = (int)((maxHeight-minHeight)*(trkbr_contours.Value/100f));
            int seaLevel = (int)(256*(trkbr_seaLevel.Value/100f));

            if (heights.Length != 0) {
                for (int x = 1; x < img.Width-1; x++) {
                    for (int y = 1; y < img.Height-1; y++) {
                        if (heights[x,y] <= seaLevel) { img.SetPixel(x, y, colWater); } 
                        else { 
                            float avg = ((heights[x, y]/gap)*gap + (heights[x-1, y]/gap)*gap + (heights[x, y-1]/gap)*gap + (heights[x+1, y]/gap)*gap + (heights[x, y+1]/gap)*gap)/5.0f;
                            if ((heights[x, y]/gap)*gap != avg) {
                                img.SetPixel(x, y, colHeightLine);
                            } else {
                                img.SetPixel(x, y, colEmpty);
                                //int val = heights[x, y];
                                //img.SetPixel(x, y, Color.FromArgb(val, val, val));
                            }
                        }
                    }
                }

                canvas.Image = img;
            }

            bttn_generateHeights.Enabled = true;
        }
    }
}