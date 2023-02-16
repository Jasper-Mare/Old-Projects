using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Drawing;
using FastBitmapLib;

namespace TryingFastBitmap {
    public partial class Form1 : Form {
        const int imgW = 128, imgH = 128;


        enum PredefinedColours : uint {
            Red   = 0xFF_FF_00_00,
            Green = 0x00_00_FF_00,
            Blue  = 0x00_00_00_FF,
        }

        uint getColour(byte R, byte G, byte B) {
            return 0xFF_00_00_00 | (((uint)R) << 16) | (((uint)G) << 8) | ((uint)B);
        }

        public Form1() {
            InitializeComponent();
            Shown += Form1_Shown;
        }

        private void Form1_Shown(object sender, EventArgs e) {
            pbox.InterpolationMode = InterpolationMode.NearestNeighbor;
            pbox.SizeMode = PictureBoxSizeMode.Zoom;
            pbox.Show();

            Bitmap bmp = new Bitmap(imgW, imgH);
            FastBitmap fbmp = new FastBitmap(bmp);
            fbmp.Lock(FastBitmapLockFormat.Format32bppArgb);
            
            for (int x = 0; x < imgW; x++) { 
                for (int y = 0; y < imgH; y++) {
                    if ((x+1)%(y+1) == 0) {
                        fbmp.SetPixel(x, y, getColour(255,255,0));
                    } else { 
                        fbmp.SetPixel(x,y, (uint)PredefinedColours.Red);
                    }
                }
            }

            fbmp.Unlock();
            pbox.BackColor = SystemColors.ControlDark;
            pbox.Image = bmp;
        }
    }

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