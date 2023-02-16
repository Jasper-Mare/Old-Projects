using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GraphicsMessingAbout {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            Random random = new Random();
            canvas.SizeMode = PictureBoxSizeMode.Zoom;
            int numbPoints = 5;
            for (int i = 0; i < numbPoints; i++) {
                pointPolarCoords.Add((random.Next(6283)/1000f, random.Next(40, 360))); //6283 is 1000*2*pi
            }
        }

        Stopwatch frametimer = new Stopwatch();
        float position = 0;
        float speed = 0.5f;
        List<(float,float)> pointPolarCoords = new List<(float,float)>(); //float1 is angle, float2 is distance from center

        private void canvas_Paint(object sender, PaintEventArgs e) {
            frametimer.Stop(); float deltaT = frametimer.ElapsedMilliseconds / 1000f; frametimer.Restart();
            Text = ((int)(1 / deltaT)).ToString();
            Bitmap frame = new Bitmap(800, 800);
            Graphics gfx = Graphics.FromImage(frame);
            gfx.Clear(Color.Black);
            gfx.DrawLine   (new Pen(Color.FromArgb(0, 255, 0), 2), 10,400, 790,400);
            gfx.DrawLine   (new Pen(Color.FromArgb(0, 255, 0), 2), 400,10, 400,790);
            gfx.DrawEllipse(new Pen(Color.FromArgb(0, 255, 0), 4),  10,10, 780,780);
            position += speed*deltaT;

            foreach ((float, float) point in pointPolarCoords) {
                float aDist = point.Item1 - (position%(2f*(float)Math.PI)); if (aDist < 0) { aDist += (2f * (float)Math.PI); }
                Brush b = new SolidBrush(Color.FromArgb(((byte)(aDist*40.5f)+145)*2%255, 0, 255, 0));
                gfx.FillEllipse(b, (float)(point.Item2*Math.Cos(point.Item1))+390, (float)(point.Item2 * Math.Sin(point.Item1))+390, 20, 20);

            }

            for (int a = 0; a < 128; a++) {
                float aAng = a*(float)(Math.PI/180)/2;
                gfx.DrawLine(new Pen(Color.FromArgb(a,0, 255, 0), 3), 400,400, 400+390*(float)Math.Sin(-(position+aAng)),400+390*(float)Math.Cos(-(position+aAng)));
            }
            canvas.Image = frame;
        }
    }
}