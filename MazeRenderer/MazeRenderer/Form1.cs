using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;

namespace MazeRenderer {
    public partial class Form1 : Form {
        //=================================game engine requirements=========
        new const int Width = 1080;
        new const int Height = 608;

        System.Timers.Timer UpdateLoop = new System.Timers.Timer(1);
        Image img;
        Random random = new Random();
        PixelBox Canvas = new PixelBox();
        Stopwatch frameTimer = new Stopwatch();
        Stopwatch updateTimer = new Stopwatch();
        Dictionary<Keys, bool> KeyOn = new Dictionary<Keys, bool>();
        InterpolationMode interpMode = InterpolationMode.NearestNeighbor;
        InterpolationMode canvasInterpMode = InterpolationMode.NearestNeighbor;
        //==================================================================
        bool paused = false;
        bool reroll = true;
        int mazewidth = 5, mazeheight = 5;
        Cell[] maze;

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

            img = new Bitmap(Width, Height);
            Canvas.Image = img;
            Canvas.Paint += Canvas_Paint;
            Canvas.Dock = DockStyle.Fill;
            Canvas.SizeMode = PictureBoxSizeMode.Zoom;
            Canvas.InterpolationMode = canvasInterpMode;
            Controls.Add(Canvas);
            Canvas.Invalidate();

            maze = new Cell[mazewidth*mazeheight];
            for (int i = 0; i < maze.Length; i++) {
                maze[i] = new Cell();
                maze[i].North = true;
                maze[i].South = true;
                maze[i].East  = true;
                maze[i].West  = true;
            }
        }

        private void Canvas_Paint(object sender, PaintEventArgs e) {
            float deltaT = frameTimer.ElapsedMilliseconds / 1000f;
            float fps = 1 / deltaT;
            frameTimer.Restart();
            Graphics gfx = Graphics.FromImage(img);
            gfx.InterpolationMode = interpMode;
            gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            gfx.Clear(Color.White);

            Pen pNorth = new Pen(Color.Red,   3);
            Pen pSouth = new Pen(Color.Blue,  3);
            Pen pEast = new Pen(Color.Green,  3);
            Pen pWest = new Pen(Color.Yellow, 3);
            gfx.FillRectangle(Brushes.Gray, 10, 10, mazewidth*100, mazeheight*100);
            for (int x, y, i = 0; i < maze.Length; i++) {
                x = i % mazeheight; y = i / mazeheight;
                if (maze[i].North) { gfx.DrawLine(pNorth, (x+0.02f)*100+10, (y+0.02f)*100+10, (x+0.02f)*100+10, (y+0.98f)*100+10); }
                if (maze[i].South) { gfx.DrawLine(pSouth, (x+0.02f)*100+10, (y+0.02f)*100+10, (x+0.98f)*100+10, (y+0.02f)*100+10); }
                if (maze[i].East ) { gfx.DrawLine(pEast , (x+0.98f)*100+10, (y+0.02f)*100+10, (x+0.98f)*100+10, (y+0.98f)*100+10); }
                if (maze[i].West ) { gfx.DrawLine(pWest , (x+0.02f)*100+10, (y+0.98f)*100+10, (x+0.98f)*100+10, (y+0.98f)*100+10); }
            }
            Canvas.Image = img;
        }

        private void Update_Tick(object sender, EventArgs e) {
            UpdateLoop.Enabled = false;
            float deltaT = updateTimer.ElapsedMilliseconds / 1000f;
            updateTimer.Restart();
            if (!paused) {
                if (reroll) {
                    reroll = false;
                    for (int i = 0; i < maze.Length; i++) {
                        maze[i].North = random.Next(0, 2) == 1;
                        maze[i].South = random.Next(0, 2) == 1;
                        maze[i].East  = random.Next(0, 2) == 1;
                        maze[i].West  = random.Next(0, 2) == 1;
                    }
                }
            }
            UpdateLoop.Enabled = true;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e) {
            if (!KeyOn.ContainsKey(e.KeyCode)) { KeyOn.Add(e.KeyCode, true); }
            else { KeyOn[e.KeyCode] = true; }
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e) {
            if (KeyOn.ContainsKey(e.KeyCode)) { KeyOn[e.KeyCode] = false; }
            if (e.KeyCode == Keys.Escape) { paused = !paused; }
            if (e.KeyCode == Keys.Space ) { reroll = !reroll; }
        }
    }
    class Cell {
        public bool North, East, South, West;
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
