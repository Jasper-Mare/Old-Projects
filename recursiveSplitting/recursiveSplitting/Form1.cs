using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace recursiveSplitting {
    public partial class Form1 : Form {
        Random rng = new Random();
        List<(float,bool,bool)> Tree = new List<(float,bool,bool)>();
        Bitmap bmp = new Bitmap(1024,1024);
        Graphics gfx;

        public Form1() {
            InitializeComponent();

            gfx = Graphics.FromImage(bmp);
        }

        private void button1_Click(object sender,EventArgs e) {
            Tree = new List<(float, bool, bool)>();
            gfx.Clear(Color.Black);

            button1.Enabled = false;

            Split(0, 5, 0.75f);
            Rectangle space = new Rectangle(128,128,768,768);
            gfx.DrawRectangle(Pens.White, space);
            Draw(0, space, false);

            button1.Enabled = true;

            pictureBox1.Image = bmp;
        }

        void Split(int depth, int maxDepth, float continueChance) {
            bool left = false, right = false;

            if(depth != maxDepth) { 
                if (rng.NextDouble() < continueChance) {
                    left = true;
                }
                if(rng.NextDouble() < continueChance) {
                    right = true;
                }
            }

            float val = (float)rng.NextDouble()*0.4f+0.3f;
            Tree.Add((val, left,right));

            if(left) {
                Split(depth+1,maxDepth,continueChance);
            }
            if(right) {
                Split(depth+1,maxDepth,continueChance);
            }
        }

        int Draw(int currentIndex, Rectangle space, bool horizontal) {
            //drawNode
            float val  = Tree[currentIndex].Item1;
            bool left  = Tree[currentIndex].Item2;
            bool right = Tree[currentIndex].Item3;

            if (horizontal) {
                gfx.DrawLine(Pens.White, space.X+(space.Width*val),space.Y, space.X+(space.Width*val),space.Y+space.Height);
            } else { 
                gfx.DrawLine(Pens.White, space.X,space.Y+(space.Height*val), space.X+space.Width,space.Y+(space.Height*val));
            }


            //Go to Left if it has one
            if (left) {
                Rectangle rect;
                if (horizontal) { //next cut is vertical
                    rect = new Rectangle(space.X, space.Y, (int)(space.Width*val), space.Height);
                } else { //next cut is horizontal
                    rect = new Rectangle(space.X,space.Y, space.Width, (int)(space.Height*val));
                }
                currentIndex = Draw(currentIndex+1, rect, !horizontal);
            }

            //Go to Right if it has one
            if(right) {
                Rectangle rect;
                if(horizontal) { //next cut is vertical
                    rect = new Rectangle(space.X+(int)(space.Width*val), space.Y, (int)(space.Width*(1-val)), space.Height);
                } else { //next cut is horizontal
                    rect = new Rectangle(space.X, space.Y+(int)(space.Height*val), space.Width, (int)(space.Height*(1-val)));
                }
                currentIndex = Draw(currentIndex+1, rect, !horizontal);
            }

            return currentIndex;
        }
    }
}
