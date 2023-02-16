namespace OSmapGenerator
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.bttn_generateHeights = new System.Windows.Forms.Button();
            this.pnl_buttons = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.trkbr_seaLevel = new System.Windows.Forms.TrackBar();
            this.trkbr_contours = new System.Windows.Forms.TrackBar();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.canvas = new OSmapGenerator.PixelBox();
            this.pnl_buttons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkbr_seaLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkbr_contours)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).BeginInit();
            this.SuspendLayout();
            // 
            // bttn_generateHeights
            // 
            this.bttn_generateHeights.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bttn_generateHeights.Location = new System.Drawing.Point(3, 3);
            this.bttn_generateHeights.Name = "bttn_generateHeights";
            this.bttn_generateHeights.Size = new System.Drawing.Size(407, 71);
            this.bttn_generateHeights.TabIndex = 0;
            this.bttn_generateHeights.Text = "Generate Contour Lines";
            this.bttn_generateHeights.UseVisualStyleBackColor = true;
            this.bttn_generateHeights.Click += new System.EventHandler(this.bttn_generateHeights_Click);
            // 
            // pnl_buttons
            // 
            this.pnl_buttons.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.pnl_buttons.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnl_buttons.Controls.Add(this.label2);
            this.pnl_buttons.Controls.Add(this.label1);
            this.pnl_buttons.Controls.Add(this.trkbr_seaLevel);
            this.pnl_buttons.Controls.Add(this.trkbr_contours);
            this.pnl_buttons.Controls.Add(this.bttn_generateHeights);
            this.pnl_buttons.Location = new System.Drawing.Point(12, 43);
            this.pnl_buttons.Name = "pnl_buttons";
            this.pnl_buttons.Size = new System.Drawing.Size(415, 1273);
            this.pnl_buttons.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 153);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(108, 25);
            this.label2.TabIndex = 5;
            this.label2.Text = "Sea Level";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 77);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(134, 25);
            this.label1.TabIndex = 4;
            this.label1.Text = "Contour Gap";
            // 
            // trkbr_seaLevel
            // 
            this.trkbr_seaLevel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trkbr_seaLevel.Location = new System.Drawing.Point(3, 181);
            this.trkbr_seaLevel.Maximum = 100;
            this.trkbr_seaLevel.Minimum = -1;
            this.trkbr_seaLevel.Name = "trkbr_seaLevel";
            this.trkbr_seaLevel.Size = new System.Drawing.Size(407, 90);
            this.trkbr_seaLevel.TabIndex = 3;
            this.trkbr_seaLevel.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trkbr_seaLevel.Value = 1;
            // 
            // trkbr_contours
            // 
            this.trkbr_contours.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trkbr_contours.Location = new System.Drawing.Point(3, 105);
            this.trkbr_contours.Maximum = 100;
            this.trkbr_contours.Minimum = 1;
            this.trkbr_contours.Name = "trkbr_contours";
            this.trkbr_contours.Size = new System.Drawing.Size(407, 90);
            this.trkbr_contours.TabIndex = 1;
            this.trkbr_contours.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trkbr_contours.Value = 1;
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1724, 40);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFileToolStripMenuItem,
            this.saveToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(72, 36);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openFileToolStripMenuItem
            // 
            this.openFileToolStripMenuItem.Name = "openFileToolStripMenuItem";
            this.openFileToolStripMenuItem.Size = new System.Drawing.Size(341, 44);
            this.openFileToolStripMenuItem.Text = "Open Height map";
            this.openFileToolStripMenuItem.Click += new System.EventHandler(this.openFileToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(341, 44);
            this.saveToolStripMenuItem.Text = "Save OS map";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "All Files|*.*|PNGs (*.png)|*.png|Bitmaps (*.bmp)|*.bmp|JPEGs (*.jpg)|*.jpg";
            // 
            // canvas
            // 
            this.canvas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.canvas.BackColor = System.Drawing.SystemColors.ControlDark;
            this.canvas.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.canvas.Location = new System.Drawing.Point(433, 43);
            this.canvas.Name = "canvas";
            this.canvas.Size = new System.Drawing.Size(1273, 1273);
            this.canvas.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.canvas.TabIndex = 1;
            this.canvas.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1724, 1329);
            this.Controls.Add(this.canvas);
            this.Controls.Add(this.pnl_buttons);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(1750, 1400);
            this.Name = "Form1";
            this.Text = "OS Map Generator";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.pnl_buttons.ResumeLayout(false);
            this.pnl_buttons.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkbr_seaLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkbr_contours)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bttn_generateHeights;
        private PixelBox canvas;
        private System.Windows.Forms.Panel pnl_buttons;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openFileToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.TrackBar trkbr_contours;
        private System.Windows.Forms.TrackBar trkbr_seaLevel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}

