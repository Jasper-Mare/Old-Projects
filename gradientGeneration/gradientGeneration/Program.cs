using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using PixelEngine;

namespace gradientGeneration
{
    class Program : Game {
        //vars
        const float PI = (float)Math.PI;
        const float step = 4;
        const float aUp = 0, aLeft = 1.5f*PI, aDown = PI, aRight = 0.5f*PI;
        List<PointF> pathofline = new List<PointF>();
        List<float>[] WorldPoints;
        int[,] world;
        int s = 1;

        //functions
        static void Main(string[] args) {
            Program prog = new Program();
            prog.Construct(400,300,4,4);
            prog.Start();
        }

        public override void OnCreate() {
            WorldPoints = new List<float>[ScreenWidth];
            world = new int[ScreenWidth,ScreenHeight];
            generate(s, 0.3f);
            drawMap();
        }

        public override void OnUpdate(float delta) {
            if (GetKey(Key.Space).Pressed) {
                s = Random(int.MaxValue);
                WorldPoints = new List<float>[ScreenWidth];
                world = new int[ScreenWidth, ScreenHeight];
                generate(s, 0.3f);
                drawMap();
            }
        }

        public void generate(int seed, float variance = 0.1f) {
            world = new int[ScreenWidth, ScreenHeight];
            WorldPoints = new List<float>[ScreenWidth];
            pathofline = new List<PointF>();
            Random random = new Random(seed);
            float angle = aRight; //radians
            float lastX = 0, lastY = ScreenHeight/2;

            for (int a = 0; a < ScreenWidth; a++) { WorldPoints[a] = new List<float>(); }

            int length = 0;
            while (lastX < ScreenWidth && length < 5000) {
                pathofline.Add(new PointF(lastX, lastY));
                float x = lastX;
                float y = lastY;

                x += (int)(Math.Sin(angle) * step);
                y += (int)(Math.Cos(angle) * step);
                angle += (float)(random.NextDouble()-0.5f)*2*variance;
                angle %= 2f*PI; //keep between 0 and 2pi

                //is the point near an edge?
                if (x < 20) { angle = aRight+(float)((random.NextDouble()-0.5f)*PI*0.2f); }//left edge
                if (y < 20) { angle = aRight-(0.1f*PI); }//top edge
                if (y > ScreenHeight - 20) { angle = aRight+(0.1f*PI); }//bottom edge

                lastX = x;
                lastY = y;
                length++;
            }

            //get a y pos for every x
            for (int p = 1; p < pathofline.Count; p++) {
                float x1 = pathofline[p-1].X, y1 = pathofline[p-1].Y, x2 = pathofline[p].X, y2 = pathofline[p].Y;
                int dir = Math.Sign((int)x2-(int)x1);
                if (dir == 1) {
                    for (int x = (int)x1; x < (int)x2; x++) {
                        WorldPoints[x].Add(Map(x, x1, x2, y1, y2));
                    }
                } else if (dir == -1) { 
                    for (int x = (int)x1; x > (int)x2; x--) {
                        WorldPoints[x].Add(Map(x, x1, x2, y1, y2));
                    }
                }
            }

            //turn into map
            for (int x = 0; x < ScreenWidth; x++) {
                WorldPoints[x].Add(0);
                WorldPoints[x].Add(ScreenHeight);
                WorldPoints[x].RemoveAll(c => c > ScreenHeight || c < 0);
                WorldPoints[x].Sort();
                for (int i = 1; i < WorldPoints[x].Count-1; i++) { 
                    if (WorldPoints[x][i]-WorldPoints[x][i-1] <= step && WorldPoints[x][i+1]-WorldPoints[x][i] <= step) { 
                        WorldPoints[x][i] = float.NaN; 
                    }
                }
                WorldPoints[x].RemoveAll(c => float.IsNaN(c));

                for (int p = 0; p < WorldPoints[x].Count - 1; p++) {
                    int fill = p%2;
                    for (int y = (int)WorldPoints[x][p]; y < WorldPoints[x][p+1]; y++) {
                        world[x,ScreenHeight-y-1] = fill;
                    }
                    world[x, ScreenHeight-(int)WorldPoints[x][p]-1] = 2;
                }
            }

            foreach (PointF p in pathofline) {
                if (p.X >= 0 && p.X < ScreenWidth && p.Y >= 0 && p.Y < ScreenHeight) { 
                    //world[(int)p.X, (int)(ScreenHeight - p.Y - 1)] = 2; 
                }
            }
        }

        void drawMap() {
            for (int x = 0; x < ScreenWidth; x++) {
                for (int y = 0; y < ScreenHeight; y++) {
                    Pixel p;
                    if (world[x, y] == 0) { p = Pixel.Presets.Cyan; }
                    else if (world[x, y] == 1) { p = Pixel.Presets.Green; }
                    else { p = Pixel.Presets.Red; }
                    Draw(x,ScreenHeight-y-1, p);
                }
            }
            //DrawPath(pathofline.ToArray(), Pixel.Presets.Red);
        }
    }
}
