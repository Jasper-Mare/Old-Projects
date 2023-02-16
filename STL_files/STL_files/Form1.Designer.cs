namespace STL_files
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.canvas = new STL_files.PixelBox();
            this.bttn_newPolygon = new System.Windows.Forms.Button();
            this.bttn_export = new System.Windows.Forms.Button();
            this.trkBar_SnapScale = new System.Windows.Forms.TrackBar();
            this.lbl_snap = new System.Windows.Forms.Label();
            this.lbl_zoom = new System.Windows.Forms.Label();
            this.trkbr_zoom = new System.Windows.Forms.TrackBar();
            this.drpdwn_polygons = new System.Windows.Forms.ComboBox();
            this.lbl_polygons = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkBar_SnapScale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkbr_zoom)).BeginInit();
            this.SuspendLayout();
            // 
            // canvas
            // 
            this.canvas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.canvas.Location = new System.Drawing.Point(22, 26);
            this.canvas.Margin = new System.Windows.Forms.Padding(6);
            this.canvas.Name = "canvas";
            this.canvas.Size = new System.Drawing.Size(1245, 909);
            this.canvas.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.canvas.TabIndex = 0;
            this.canvas.TabStop = false;
            // 
            // bttn_newPolygon
            // 
            this.bttn_newPolygon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bttn_newPolygon.Location = new System.Drawing.Point(1276, 26);
            this.bttn_newPolygon.Name = "bttn_newPolygon";
            this.bttn_newPolygon.Size = new System.Drawing.Size(198, 112);
            this.bttn_newPolygon.TabIndex = 1;
            this.bttn_newPolygon.Text = "New Polygon";
            this.bttn_newPolygon.UseVisualStyleBackColor = true;
            this.bttn_newPolygon.Click += new System.EventHandler(this.bttn_newPolygon_Click);
            // 
            // bttn_export
            // 
            this.bttn_export.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bttn_export.Location = new System.Drawing.Point(1276, 823);
            this.bttn_export.Name = "bttn_export";
            this.bttn_export.Size = new System.Drawing.Size(198, 112);
            this.bttn_export.TabIndex = 2;
            this.bttn_export.Text = "Export STL File";
            this.bttn_export.UseVisualStyleBackColor = true;
            this.bttn_export.Click += new System.EventHandler(this.bttn_export_Click);
            // 
            // trkBar_SnapScale
            // 
            this.trkBar_SnapScale.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.trkBar_SnapScale.Location = new System.Drawing.Point(1276, 144);
            this.trkBar_SnapScale.Minimum = 1;
            this.trkBar_SnapScale.Name = "trkBar_SnapScale";
            this.trkBar_SnapScale.Size = new System.Drawing.Size(198, 90);
            this.trkBar_SnapScale.TabIndex = 3;
            this.trkBar_SnapScale.Value = 1;
            this.trkBar_SnapScale.Scroll += new System.EventHandler(this.trkBar_SnapScale_Scroll);
            // 
            // lbl_snap
            // 
            this.lbl_snap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_snap.AutoSize = true;
            this.lbl_snap.Location = new System.Drawing.Point(1282, 189);
            this.lbl_snap.Name = "lbl_snap";
            this.lbl_snap.Size = new System.Drawing.Size(72, 32);
            this.lbl_snap.TabIndex = 4;
            this.lbl_snap.Text = "Snap:";
            // 
            // lbl_zoom
            // 
            this.lbl_zoom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_zoom.AutoSize = true;
            this.lbl_zoom.Location = new System.Drawing.Point(1282, 285);
            this.lbl_zoom.Name = "lbl_zoom";
            this.lbl_zoom.Size = new System.Drawing.Size(82, 32);
            this.lbl_zoom.TabIndex = 6;
            this.lbl_zoom.Text = "Zoom:";
            // 
            // trkbr_zoom
            // 
            this.trkbr_zoom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.trkbr_zoom.Location = new System.Drawing.Point(1276, 240);
            this.trkbr_zoom.Maximum = 3;
            this.trkbr_zoom.Minimum = -3;
            this.trkbr_zoom.Name = "trkbr_zoom";
            this.trkbr_zoom.Size = new System.Drawing.Size(198, 90);
            this.trkbr_zoom.TabIndex = 5;
            this.trkbr_zoom.Scroll += new System.EventHandler(this.trkbr_zoom_Scroll);
            // 
            // drpdwn_polygons
            // 
            this.drpdwn_polygons.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.drpdwn_polygons.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpdwn_polygons.FormattingEnabled = true;
            this.drpdwn_polygons.Location = new System.Drawing.Point(1276, 352);
            this.drpdwn_polygons.Name = "drpdwn_polygons";
            this.drpdwn_polygons.Size = new System.Drawing.Size(198, 40);
            this.drpdwn_polygons.TabIndex = 7;
            // 
            // lbl_polygons
            // 
            this.lbl_polygons.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_polygons.AutoSize = true;
            this.lbl_polygons.Location = new System.Drawing.Point(1282, 317);
            this.lbl_polygons.Name = "lbl_polygons";
            this.lbl_polygons.Size = new System.Drawing.Size(115, 32);
            this.lbl_polygons.TabIndex = 8;
            this.lbl_polygons.Text = "Polygons:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1486, 960);
            this.Controls.Add(this.lbl_polygons);
            this.Controls.Add(this.drpdwn_polygons);
            this.Controls.Add(this.lbl_zoom);
            this.Controls.Add(this.trkbr_zoom);
            this.Controls.Add(this.lbl_snap);
            this.Controls.Add(this.trkBar_SnapScale);
            this.Controls.Add(this.bttn_export);
            this.Controls.Add(this.bttn_newPolygon);
            this.Controls.Add(this.canvas);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkBar_SnapScale)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkbr_zoom)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PixelBox canvas;
        private System.Windows.Forms.Button bttn_newPolygon;
        private System.Windows.Forms.Button bttn_export;
        private System.Windows.Forms.TrackBar trkBar_SnapScale;
        private System.Windows.Forms.Label lbl_snap;
        private System.Windows.Forms.Label lbl_zoom;
        private System.Windows.Forms.TrackBar trkbr_zoom;
        private System.Windows.Forms.ComboBox drpdwn_polygons;
        private System.Windows.Forms.Label lbl_polygons;
    }
}

