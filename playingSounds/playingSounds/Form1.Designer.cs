
namespace playingSounds {
    partial class Form1 {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.bttn_load = new System.Windows.Forms.Button();
            this.pnl_buttons = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // bttn_load
            // 
            this.bttn_load.Location = new System.Drawing.Point(427, 12);
            this.bttn_load.Name = "bttn_load";
            this.bttn_load.Size = new System.Drawing.Size(75, 23);
            this.bttn_load.TabIndex = 0;
            this.bttn_load.Text = "Load";
            this.bttn_load.UseVisualStyleBackColor = true;
            this.bttn_load.Click += new System.EventHandler(this.bttn_load_Click);
            // 
            // pnl_buttons
            // 
            this.pnl_buttons.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.pnl_buttons.AutoScroll = true;
            this.pnl_buttons.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnl_buttons.Location = new System.Drawing.Point(12, 12);
            this.pnl_buttons.Name = "pnl_buttons";
            this.pnl_buttons.Size = new System.Drawing.Size(409, 426);
            this.pnl_buttons.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.pnl_buttons);
            this.Controls.Add(this.bttn_load);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bttn_load;
        private System.Windows.Forms.Panel pnl_buttons;
    }
}

