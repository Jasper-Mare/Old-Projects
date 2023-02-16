using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Media;
using _3d.Properties;

namespace _3d {
    public partial class Form1 : Form {
        //=================================game engine requirements=========
        new const int Width  = 1080;
        new const int Height = 608;

        System.Timers.Timer UpdateLoop = new System.Timers.Timer(1);
        Image img;
        Random random = new Random();
        Stopwatch updateTimer = new Stopwatch();
        Stopwatch frameTimer = new Stopwatch();
        PixelBox Canvas = new PixelBox();
        InterpolationMode interpMode = InterpolationMode.NearestNeighbor;
        InterpolationMode canvasInterpMode = InterpolationMode.NearestNeighbor;
        //==================================================================

        float speed = 4f;
        Point3 playerPos = new Point3(0,1,0);
        float yVel = 0f;
        float playerPitch = 0;
        float playerYaw = 0;

        bool paused = false;
        bool showFrameTimes = false;
        bool kForwards = false;
        bool kBack = false;
        bool kLeft = false;
        bool kRight = false;
        bool kJump = false;
        Point mouseLastPos;
        float mouseSensitivity = 0.01f;

        Queue<float> frametimes = new Queue<float>();
        Pen penOutline = new Pen(Color.Black, Width/100);

        Wall[] walls;

        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            MinimumSize = new Size(Width, Height);
            KeyPreview = true;
            KeyDown += Form1_KeyDown;
            KeyUp += Form1_KeyUp;

            UpdateLoop.Enabled = true;
            UpdateLoop.Elapsed += new System.Timers.ElapsedEventHandler(Update_Tick);
            UpdateLoop.Start();

            img = new Bitmap(Width,Height);
            Canvas.Image = img;
            Canvas.Paint += Canvas_Paint;
            Canvas.Dock = DockStyle.Fill;
            Canvas.SizeMode = PictureBoxSizeMode.Zoom;
            Canvas.InterpolationMode = canvasInterpMode;
            Controls.Add(Canvas);

            walls = new Wall[] { 
                new Wall(new PointF(-5,  5), new PointF( 5,  5), 0   , 1.1f, new SolidBrush(Color.Red)),
                new Wall(new PointF( 5, -5), new PointF( 5,  5), 0   , 1.5f, new SolidBrush(Color.Blue)),
                new Wall(new PointF(-5, -5), new PointF( 5, -5), 0   , 2   , new SolidBrush(Color.Green)),
                new Wall(new PointF(-5, -5), new PointF(-5,  5), 0   , 1.5f, new SolidBrush(Color.Yellow)),
            };

            Canvas.Invalidate();
        }

        //======================================================================//
        private void Canvas_Paint(object sender, PaintEventArgs e) {
            float deltaT = frameTimer.ElapsedMilliseconds / 1000f;
            float fps = 1/deltaT;
            frameTimer.Restart();
            Graphics gfx = Graphics.FromImage(img);
            gfx.InterpolationMode = interpMode;
            gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            if (!paused) {
                frametimes.Enqueue(deltaT);
                if (frametimes.Count >= Width/3) { frametimes.Dequeue(); }
                //skybox
                gfx.Clear(Color.CornflowerBlue);
                for (int x = 0; x < Width; x++) { float s = (float)Math.Sin(2.5*(playerYaw+(x*(Math.PI/(2*Width))))); s *= s; 
                    gfx.DrawLine(Pens.SandyBrown, x, Height/2, x, (2-s)*Height/4);
                }
                gfx.FillRectangle(Brushes.DarkGreen, 0, Height/2, Width, Height/2+1);

                foreach (Wall w in walls) {
                    Point[] wPoints = projectWall(w);
                    if (wPoints.Length == 0) { continue; }
                    gfx.FillPolygon(w.texture, wPoints);
                    gfx.DrawPolygon(penOutline, wPoints);
                }

                Text = "Res: " + this.Size.ToString() +" FPS: " + fps.ToString("000");
                if (showFrameTimes) {
                    int a = 0;
                    foreach (float f in frametimes) {
                        gfx.FillRectangle(Brushes.Red, a, Height-(f*Height), 3, (f*Height));
                        a += 3;
                    }
                }
            } else {
                //gfx.FillRectangle(new SolidBrush(Color.FromArgb(5, Color.Black)), 0,0,Width,Height);
                gfx.DrawString("Paused", new Font(FontFamily.GenericSansSerif, 45f), Brushes.Red, new Point(0,0));
            }
            //Canvas.Image = img;
            Canvas.Invalidate();
        }
        private void Update_Tick(object sender, EventArgs e) {
            UpdateLoop.Enabled = false;
            float deltaT = updateTimer.ElapsedMilliseconds / 1000f;
            updateTimer.Restart();
            if (!paused) {
                float tmpSpeed = speed * deltaT;

                if (kForwards) { playerPos.X += tmpSpeed * (float)Math.Sin(playerYaw); playerPos.Z += tmpSpeed * (float)Math.Cos(playerYaw); }
                if (kBack)     { playerPos.X -= tmpSpeed * (float)Math.Sin(playerYaw); playerPos.Z -= tmpSpeed * (float)Math.Cos(playerYaw); }
                if (kLeft)     { playerPos.X -= tmpSpeed * (float)Math.Cos(playerYaw); playerPos.Z += tmpSpeed * (float)Math.Sin(playerYaw); }
                if (kRight)    { playerPos.X += tmpSpeed * (float)Math.Cos(playerYaw); playerPos.Z -= tmpSpeed * (float)Math.Sin(playerYaw); }

                if (playerPos.Y <= 1) { playerPos.Y = 1; yVel = 0; }
                if (kJump) { if (yVel == 0) { yVel = 5; } }
                playerPos.Y += yVel * deltaT;
                yVel -= 10f * deltaT;

                playerYaw   -= (mouseLastPos.X - MousePosition.X)*mouseSensitivity;
                playerYaw   %= 2*(float)Math.PI;
                playerPitch += (mouseLastPos.Y - MousePosition.Y)*mouseSensitivity;
                playerPitch %= 2*(float)Math.PI;
            }
            mouseLastPos = MousePosition;
            UpdateLoop.Enabled = true;
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e) {
            switch (e.KeyData) {
                case Keys.W: kForwards = true; break;
                case Keys.A: kLeft     = true; break;
                case Keys.S: kBack     = true; break;
                case Keys.D: kRight    = true; break;
                case Keys.Space: kJump = true; break;
            }
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e) {
            switch (e.KeyData) {
                case Keys.W: kForwards   = false;   break;
                case Keys.A: kLeft       = false;   break;
                case Keys.S: kBack       = false;   break;
                case Keys.D: kRight      = false;   break;
                case Keys.Space:  kJump  = false;   break;
                case Keys.Escape: paused = !paused; break;
                case Keys.F3: showFrameTimes = !showFrameTimes; break;
            }
        }

        //======================================================================//
        private PointF GetScreenPoint(Point3 p3d, out bool show) {
            float X = p3d.X - playerPos.X; float Y = p3d.Y - playerPos.Y; float Z = p3d.Z - playerPos.Z;
            
            PointF tmp = RotatePoint(new PointF(X,Z), playerYaw); X = tmp.X; Z = tmp.Y; //rotate due to player's Yaw (get angle and direction, then change angle) (x and z)

            if (Z <= 0) { show = false; } else { show = true; }
            PointF val = new PointF(Height* (X/Math.Abs(Z))+0.5f*Width, Height*(0.5f-Y/Math.Abs(Z)));
            return val;
        }
        static PointF RotatePoint(PointF pointToRotate, double angleInRadians) {
            double cosTheta = Math.Cos(angleInRadians);
            double sinTheta = Math.Sin(angleInRadians);
            return new PointF {
                X = (float)(cosTheta * (pointToRotate.X) - sinTheta * (pointToRotate.Y)),
                Y = (float)(sinTheta * (pointToRotate.X) + cosTheta * (pointToRotate.Y))
            };
        }

        /// <summary>
        /// takes a wall and gets the points of it's edges on screen
        /// </summary>
        /// <param name="wall">The wall to project</param>
        /// <returns>Returns an array of 4 points or if the wall is completly offscreen 0 points that describe the corners of the wall.</returns>
        Point[] projectWall(Wall wall) {
            PointF a2 = new PointF(), b2 = new PointF(); // a.y == wall a collumn z pos
            //1) transform the points around the player
            a2.X = wall.a.X - playerPos.X; b2.X = wall.b.X - playerPos.X;
            a2.Y = wall.a.Y - playerPos.Z; b2.Y = wall.b.Y - playerPos.Z;
            //2) rotate the points around the player
            a2 = RotatePoint(a2, playerYaw);
            b2 = RotatePoint(b2, playerYaw);
            //3) project
            if (a2.Y > 0 || b2.Y > 0 ) { //either 1 or both points are in view
                //If the line crosses the player's viewplane, clip it.
                PointF i1 = intersect(a2, b2, new PointF(-0.0001f,0.0001f), new PointF(-20, 1));
                PointF i2 = intersect(a2, b2, new PointF( 0.0001f,0.0001f), new PointF( 20, 1));

                if (a2.Y <= 0) { if (i1.Y > 0) { a2.X = i1.X; a2.Y = i1.Y; } else { a2.X = i2.X; a2.Y = i2.Y; } }
                if (b2.Y <= 0) { if (i1.Y > 0) { b2.X = i1.X; b2.Y = i1.Y; } else { b2.X = i2.X; b2.Y = i2.Y; } }

                float top = wall.top - playerPos.Y;
                float bottom = wall.bottom - playerPos.Y;

                //project the points
                return new Point[4] { 
                    new Point((int)(Height*(a2.X/a2.Y)+0.5f*Width), (int)(Height*(0.5f-top/a2.Y))), //a top 
                    new Point((int)(Height*(b2.X/b2.Y)+0.5f*Width), (int)(Height*(0.5f-top/b2.Y))), //b top
                    new Point((int)(Height*(b2.X/b2.Y)+0.5f*Width), (int)(Height*(0.5f-bottom/b2.Y))), //b bottom
                    new Point((int)(Height*(a2.X/a2.Y)+0.5f*Width), (int)(Height*(0.5f-bottom/a2.Y))), //a bottom
                };
            }
            else { //neither of the points are in view
                return new Point[0];
            }
        }
        PointF intersect(PointF p1, PointF p2, PointF p3, PointF p4) {
            float x, y, denominator;
            x = crossProd(p1, p2);
            y = crossProd(p3, p4);
            denominator = crossProd(p1.X-p2.X, p1.Y-p2.Y, p3.X-p4.X, p3.Y-p4.Y);
            x = crossProd(x, p1.X-p2.X, y, p3.X-p4.X) / denominator;
            y = crossProd(x, p1.Y-p2.Y, y, p3.Y-p4.Y) / denominator;
            return new PointF(x,y);
        }
        float crossProd(PointF p1, PointF p2) { return p1.X*p2.Y - p1.Y*p2.X; }
        float crossProd(float x1, float y1, float x2, float y2) { return x1*y2 - y1*x2; }

    }
    class Tri {
        public Point3[] verticies;
        public Color color = Color.Black;
        public Tri(Point3 p0, Point3 p1, Point3 p2) {
            verticies = new Point3[] { p0, p1, p2 };
        }
        public Tri(Point3 p0, Point3 p1, Point3 p2, Color color) {
            verticies = new Point3[] { p0, p1, p2 }; this.color = color;
        }
        public Point3 normalVector {
            get { //credit: https://math.stackexchange.com/a/305914
                Point3 V = new Point3(verticies[1].X - verticies[0].X, verticies[1].Y - verticies[0].Y, verticies[1].Z - verticies[0].Z);
                Point3 W = new Point3(verticies[2].X - verticies[0].X, verticies[2].Y - verticies[0].Y, verticies[2].Z - verticies[0].Z);
                Point3 N = new Point3(0,0,0); N.X = (V.Y * W.Z) - (V.Z * W.Y); N.Y = (V.Z * W.X) - (V.X * W.Z); N.Z = (V.X * W.Y) - (V.Y * W.X);
                Point3 A = new Point3(0,0,0);
                A.X = (N.X * N.X) / ((N.X * N.X) + (N.Y * N.Y) + (N.Z * N.Z));
                A.Y = (N.Y * N.Y) / ((N.X * N.X) + (N.Y * N.Y) + (N.Z * N.Z));
                A.Z = (N.Z * N.Z) / ((N.X * N.X) + (N.Y * N.Y) + (N.Z * N.Z));
                return A;
            }
        }
    }
    class Wall {
        public PointF a, b;
        public float bottom, top;
        public Brush texture;
        public Wall(PointF a, PointF b, float bottom, float top, Brush texture) {
            this.a = a; this.b = b; this.bottom = bottom; this.top = top; this.texture = texture;
        }
    }

    class Point3 {
        public float X;
        public float Y;
        public float Z;
        public Point3(float x, float y, float z) {
            X = x; Y = y; Z = z;
        }
        public override string ToString() {
            return X+"/"+Y+"/"+Z;
        }
    }

    /// <summary>
    /// A PictureBox with configurable interpolation mode.
    /// https://www.codeproject.com/Articles/717312/PixelBox-A-PictureBox-with-configurable-Interpolat
    /// </summary>
    public class PixelBox : PictureBox {
        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="PixelBox"> class.
        /// </see></summary>
        public PixelBox() {
            // Set default.
            InterpolationMode = InterpolationMode.Default;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the interpolation mode.
        /// </summary>
        /// <value>The interpolation mode.</value>
        [Category("Behavior")]
        [DefaultValue(InterpolationMode.Default)]
        public InterpolationMode InterpolationMode { get; set; }
        #endregion

        #region Overrides of PictureBox
        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.Paint"> event.
        /// </see></summary>
        /// <param name="pe" />A <see cref="T:System.Windows.Forms.PaintEventArgs"> that contains the event data. 
        protected override void OnPaint(PaintEventArgs pe) {
            pe.Graphics.InterpolationMode = InterpolationMode;
            base.OnPaint(pe);
        }
        #endregion
    }
}