using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IxMilia.Stl;

namespace STL_files {
    public partial class Form1 : Form {
        //====================================================================================//
        //=== engine vars === engine vars === engine vars === engine vars === engine vars === //
        //====================================================================================//
        #region Engine Variables
        new const int Width = 750;
        new const int Height = 750;

        Bitmap image = new Bitmap(Width, Height);
        Graphics gfx;
        System.Timers.Timer Updater = new System.Timers.Timer(1);
        Stopwatch updateTimer = new Stopwatch();
        Stopwatch frameTimer = new Stopwatch();
        float updateElapsedTime = 0;
        float gfxElapsedTime = 0;

        PointF CameraPos = new PointF();
        const float cameraScale = 0.1f;

        InterpolationMode interpMode = InterpolationMode.NearestNeighbor;
        Dictionary<Keys, bool> PressedKeys = new Dictionary<Keys, bool>();
        #endregion

        List<Polygon> polygons = new List<Polygon>();

        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            canvas.InterpolationMode = interpMode;
            canvas.Image = image;

            gfx = Graphics.FromImage(image);
            gfx.InterpolationMode = interpMode;
            gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

            KeyPreview = true;
            KeyDown += KeyPressed;
            KeyUp += KeyUnpressed;

            PressedKeys.Add(Keys.Up, false);
            PressedKeys.Add(Keys.Down, false);
            PressedKeys.Add(Keys.Left, false);
            PressedKeys.Add(Keys.Right, false);

            //start the timer
            Updater.Elapsed += UpdateLoop;
            Updater.Enabled = true;
            canvas.Invalidate();
            canvas.Paint += Canvas_Paint;
        }

        private void Canvas_Paint(object sender, PaintEventArgs e) {
            float deltaT = frameTimer.ElapsedMilliseconds / 1000f;
            gfxElapsedTime += deltaT;
            frameTimer.Restart();

            gfx.Clear(SystemColors.ControlDarkDark);

            gfx.ResetClip();
            gfx.TranslateTransform(Width/2,Height/2);
            gfx.TranslateTransform(-CameraPos.X,-CameraPos.Y);
            gfx.SetClip(new Rectangle((int)CameraPos.X-Width/2,(int)CameraPos.Y-Height/2,Width,Height));
            gfx.ScaleTransform(cameraScale, cameraScale);
            //https://www.vbforums.com/showthread.php?624596-RESOLVED-Offsetting-a-HatchBrush
            gfx.RenderingOrigin = new Point((int)CameraPos.X,(int)CameraPos.Y);

            //draw grid
            if (gfx.ClipBounds.Top < 0 && 0 < gfx.ClipBounds.Bottom) {
                gfx.DrawLine(Pens.LimeGreen, gfx.ClipBounds.Left,0, gfx.ClipBounds.Right,0);
            }
            if(gfx.ClipBounds.Left < 0 && 0 < gfx.ClipBounds.Right) {
                gfx.DrawLine(Pens.Red, 0,gfx.ClipBounds.Top, 0,gfx.ClipBounds.Bottom);
            }

            int gridGap = 100*trkBar_SnapScale.Value;
            for (int x = gridGap*((int)(gfx.ClipBounds.Left)/gridGap); x < gfx.ClipBounds.Right; x += gridGap) {
                if(x == 0) { continue; }
                gfx.DrawLine(Pens.Gray, x,gfx.ClipBounds.Top, x,gfx.ClipBounds.Bottom);
            }
            for(int y = gridGap*((int)(gfx.ClipBounds.Top)/gridGap); y < gfx.ClipBounds.Bottom; y += gridGap) {
                if(y == 0) { continue; }
                gfx.DrawLine(Pens.Gray, gfx.ClipBounds.Left,y, gfx.ClipBounds.Right,y);
            }



            gfx.ResetTransform();
               
            canvas.Invalidate();
        }

        private void UpdateLoop(object sender, EventArgs e) {
            Updater.Enabled = false;
            float deltaT = updateTimer.ElapsedMilliseconds / 1000f;
            updateTimer.Restart();
            updateElapsedTime += deltaT;

            if (PressedKeys[Keys.Up   ]) { CameraPos.Y -= 100*deltaT; }
            if (PressedKeys[Keys.Down ]) { CameraPos.Y += 100*deltaT; }
            if (PressedKeys[Keys.Right]) { CameraPos.X += 100*deltaT; }
            if (PressedKeys[Keys.Left ]) { CameraPos.X -= 100*deltaT; }

            Updater.Enabled = true;
        }

        private void KeyPressed(object sender, KeyEventArgs e) {
            if (!PressedKeys.ContainsKey(e.KeyCode)) { PressedKeys.Add(e.KeyCode, true); }
            else { PressedKeys[e.KeyCode] = true; }

            e.SuppressKeyPress = true;
        }
        private void KeyUnpressed(object sender, KeyEventArgs e) {
            if (!PressedKeys.ContainsKey(e.KeyCode)) { PressedKeys.Add(e.KeyCode, false); }
            else { PressedKeys[e.KeyCode] = false; }
        }

        private void bttn_newPolygon_Click(object sender,EventArgs e) {
            Polygon p = new Polygon();
        }

        private void bttn_export_Click(object sender,EventArgs e) {

        }

        private void trkBar_SnapScale_Scroll(object sender,EventArgs e) {
            lbl_snap.Text = "Snap: " + trkBar_SnapScale.Value;
        }
        private void trkbr_zoom_Scroll(object sender,EventArgs e) {
            lbl_zoom.Text = "Zoom: "+ Math.Pow(10,trkbr_zoom.Value).ToString()+"m";
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
        protected override void OnPaint (PaintEventArgs pe)  {
            pe.Graphics.InterpolationMode = InterpolationMode;
            base.OnPaint(pe);
        }
        #endregion
    }
}