using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace firstDirectX
{
    public partial class Form1 : Form {
        private Device device = null;
        private VertexBuffer vb = null;
        CustomVertex.PositionColored[] verts = null;
        Stopwatch frameTimer = new Stopwatch();
        Queue<float> frametimes = new Queue<float>();

        Point mouseLastPos;
        bool paused = false, showFrameTimes = false;
        Dictionary<Keys, bool> KeyOn = new Dictionary<Keys, bool>();
        Obj Player = new Obj(new Vector3(0,0,0), 1.57f, 0, 0);
        Wall[] walls;
        Floor[] floors;
        float mouseSensitivity = 0.005f;
        float speed = 10;

        public Form1() {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);

            walls = new Wall[] {
                new Wall(new PointF( 10, 10), new PointF(-10, 10), -2, 2, Color.Blue),
                new Wall(new PointF( 10, 10), new PointF( 10,-10), -2, 2, Color.Green),
                new Wall(new PointF(-10, 10), new PointF(-10,-10), -2, 2, Color.Red),
                new Wall(new PointF( 10,-10), new PointF(-10,-10), -2, 2, Color.Yellow),
            };
            floors = new Floor[] {
                new Floor(-2, Color.Gray, new PointF(10,10), new PointF(10,-10), new PointF(-10,-10), new PointF(-10,10)),
            };

            InitializeComponent();
            InitializeGraphics();
        }

        private void Form1_Load(object sender, EventArgs e) {
            KeyPreview = true;
            KeyDown += Form1_KeyDown;
            KeyUp += Form1_KeyUp;
            mouseLastPos = MousePosition;
        }
        private void InitializeGraphics() {
            PresentParameters pp = new PresentParameters();
            pp.Windowed = true;
            pp.SwapEffect = SwapEffect.Discard;
            pp.EnableAutoDepthStencil = true;
            pp.AutoDepthStencilFormat = DepthFormat.D16;

            device = new Device(0, DeviceType.Hardware, this, CreateFlags.HardwareVertexProcessing, pp);
            vb = new VertexBuffer(typeof(CustomVertex.PositionColored), 300, device, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionColored.Format, Pool.Default);
            vb.Created += new EventHandler(OnVertexBufferCreate);
            OnVertexBufferCreate(vb, null);

            vb.SetData(verts, 0, LockFlags.None);

        }
        private void OnVertexBufferCreate(object sender, EventArgs e) {
            VertexBuffer buffer = (VertexBuffer)sender;
            List<CustomVertex.PositionColored> vertList = new List<CustomVertex.PositionColored>();
            List<Tri> triList = new List<Tri>();

            foreach (Wall w in walls) {
                int colour = w.color.ToArgb();
                float ax = w.a.X, az = w.a.Y, bx = w.b.X, bz = w.b.Y, top = w.top, bot = w.bottom;
                triList.Add(new Tri(new Vector3(ax, bot, az), new Vector3(ax, top, az), new Vector3(bx, top, bz), colour));
                triList.Add(new Tri(new Vector3(ax, bot, az), new Vector3(bx, top, bz), new Vector3(bx, bot, bz), colour));
            }
            foreach (Floor f in floors) {
                List<utilities.Triangle> triangles = utilities.TriangulateConvexPolygon(f.points);
                foreach (utilities.Triangle tri in triangles) {
                    float ax = tri.A.X, az = tri.A.Y, bx = tri.B.X, bz = tri.B.Y, cx = tri.C.X, cz = tri.C.Y, y = f.height;
                    triList.Add(new Tri(new Vector3(ax, y, az), new Vector3(bx, y, bz), new Vector3(cx, y, cz), f.color.ToArgb()));
                }
            }

            foreach (Tri t in triList) {
                vertList.Add(new CustomVertex.PositionColored(t.vertices[0], t.color));
                vertList.Add(new CustomVertex.PositionColored(t.vertices[1], t.color));
                vertList.Add(new CustomVertex.PositionColored(t.vertices[2], t.color));
            }

            verts = vertList.ToArray();
            buffer.SetData(verts, 0, LockFlags.None);
        }
        private void setupCamera() {
            device.Transform.Projection= Matrix.PerspectiveFovLH((float)Math.PI/4, Width/Height, 1.0f, 1000f);

            Vector3 camTarget = new Vector3((float)Math.Cos(Player.Yaw)*(float)Math.Sin(Player.Pitch), (float)Math.Cos(Player.Pitch), (float)Math.Sin(Player.Yaw)*(float)Math.Sin(Player.Pitch));
            device.Transform.View = Matrix.LookAtLH(Player.Pos, Player.Pos+camTarget, new Vector3(0,1,0));

            device.RenderState.Lighting = false;
            device.RenderState.CullMode = Cull.None;
            
            device.RenderState.ZBufferWriteEnable = true;
        }
        private void _Paint(object sender, PaintEventArgs e) {
            float deltaT = frameTimer.ElapsedMilliseconds / 1000f;
            float fps = 1 / deltaT;
            frameTimer.Reset(); frameTimer.Start();

            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.CornflowerBlue, 1, 0);
            
            setupCamera();
            device.BeginScene();

            device.VertexFormat = CustomVertex.PositionColored.Format;
            device.SetStreamSource(0, vb, 0);
            device.DrawPrimitives(PrimitiveType.TriangleList, 0, 100);

            device.EndScene();
            device.Present();

            //overlays
            {
                frametimes.Enqueue(deltaT);
                if (frametimes.Count >= Width/3) { 
                    frametimes.Dequeue(); 
                }
                Graphics gfx = e.Graphics;
                if (paused) {
                    gfx.DrawString("paused", new System.Drawing.Font(FontFamily.GenericMonospace, 70), Brushes.Black, new Point(0, 0));
                }
                if (showFrameTimes) {
                    gfx.DrawString("      " + fps.ToString("000"), new System.Drawing.Font(FontFamily.GenericMonospace, 70), Brushes.Red, new Point(0, 0));
                    int a = 0;
                    foreach (float f in frametimes) {
                        gfx.FillRectangle(Brushes.Red, a, 100, 3, f*(Height-100));
                        a += 3;
                    }
                }

            }
            
            //===========================================================================================
            //=                                   =GAME=LOGIC=                                          =
            //===========================================================================================

            //=========== Input ===========
            if (!paused) {
                //Forwards
                if (KeyOn.ContainsKey(Keys.W)) { if (KeyOn[Keys.W]) { Player.Pos.X += speed*deltaT*(float)Math.Cos(Player.Yaw); Player.Pos.Z += speed*deltaT*(float)Math.Sin(Player.Yaw); } }
                //Backwards
                if (KeyOn.ContainsKey(Keys.S)) { if (KeyOn[Keys.S]) { Player.Pos.X -= speed*deltaT*(float)Math.Cos(Player.Yaw); Player.Pos.Z -= speed*deltaT*(float)Math.Sin(Player.Yaw); } }
                //StrafeLeft
                if (KeyOn.ContainsKey(Keys.A)) { if (KeyOn[Keys.A]) { Player.Pos.X -= speed*deltaT*(float)Math.Sin(Player.Yaw); Player.Pos.Z += speed*deltaT*(float)Math.Cos(Player.Yaw); } }
                //StrafeRight
                if (KeyOn.ContainsKey(Keys.D)) { if (KeyOn[Keys.D]) { Player.Pos.X += speed*deltaT*(float)Math.Sin(Player.Yaw); Player.Pos.Z -= speed*deltaT*(float)Math.Cos(Player.Yaw); } }


                //mouse
                Player.Yaw += (mouseLastPos.X - MousePosition.X) * mouseSensitivity;
                Player.Yaw %= 2*(float)Math.PI;
                Player.Pitch -= (mouseLastPos.Y-MousePosition.Y) * mouseSensitivity;
                Player.Pitch = (float)Math.Max(0, Math.Min(Math.PI-0.001f,Player.Pitch));
            }
            mouseLastPos = MousePosition;

            Invalidate();
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e) {
            if (!KeyOn.ContainsKey(e.KeyCode)) { KeyOn.Add(e.KeyCode, true); }
            else { KeyOn[e.KeyCode] = true; }
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e) {
            if (KeyOn.ContainsKey(e.KeyCode)) { KeyOn[e.KeyCode] = false; }
            if (e.KeyCode == Keys.Escape) { paused = !paused; }
            if (e.KeyCode == Keys.F3) { showFrameTimes = !showFrameTimes; }
        }

    }

    static class utilities {
        public struct Triangle { public PointF A, B, C; public Triangle(PointF a, PointF b, PointF c) { A = a; B = b; C = c; } }
        public static List<Triangle> TriangulateConvexPolygon(PointF[] convexHullpoints) {
            List<Triangle> triangles = new List<Triangle>();

            for (int i = 2; i < convexHullpoints.Length; i++) {
                PointF a = convexHullpoints[0];
                PointF b = convexHullpoints[i - 1];
                PointF c = convexHullpoints[i];

                triangles.Add(new Triangle(a, b, c));
            }

            return triangles;
        }
    }

    class Tri {
        public Vector3[] vertices = new Vector3[3];
        public int color = 0;
        public Tri(Vector3 v1, Vector3 v2, Vector3 v3, int color) {
            vertices = new Vector3[] { v1, v2, v3 }; this.color = color;
        }
    }
    class Obj {
        public Vector3 Pos = new Vector3();

        public float Pitch = 0;
        public float Roll = 0;
        public float Yaw = 0;

        public Obj(Vector3 pos, float pitch, float roll, float yaw) {
            Pos = pos; Pitch = pitch; Roll = roll; Yaw = yaw;
        }
    }
    class Wall {
        public PointF a, b;
        public float bottom, top;
        public Color color;
        public Wall(PointF a, PointF b, float bottom, float top, Color color) {
            this.a = a; this.b = b; this.bottom = bottom; this.top = top; this.color = color;
        }
    }
    class Floor { 
        public PointF[] points;
        public float height;
        public Color color;

        public Floor(PointF[] points, float height, Color color) {
            this.points = points; this.height = height; this.color = color;
        }
        public Floor(float height, Color color, params PointF[] points) {
            this.points = points; this.height = height; this.color = color;
        }
    }
}
