using Raylib_cs;
using System;
using static System.MathF;
using System.Numerics;
using System.Collections.Generic;

namespace radarSim {
    class Program {
        static void Main() {
            const int W = 1350;

            float angle = 0;
            Random rng = new Random();
            List<Vector2> things = new List<Vector2>();

            for (int i = 0; i < 10; i++) {
                float ang = (float)rng.NextDouble()*2*PI;
                things.Add(new Vector2(ang, (float)(rng.NextDouble()*0.4f+0.05f)*W));
            }

            Vector2 centre = Vector2.One*(W/2f);


            Raylib.InitWindow(W,W, "Radar");

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.BLACK);
            Raylib.DrawRectangle(0,0,W,W,new Color(0,0,0,128));
            Raylib.EndDrawing();

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.BLACK);
            Raylib.DrawRectangle(0,0,W,W,new Color(0,0,0,128));
            Raylib.EndDrawing();

            while(!Raylib.WindowShouldClose()) {
                angle += Raylib.GetFrameTime()*0.5f;
                angle = (angle+2*PI)%(2*PI);

                Vector2 lineDir = unitCircle(angle);

                Raylib.BeginDrawing();
                Raylib.DrawRectangle(0,0,W,W, new Color(0,0,0, 1));
                Raylib.ClearBackground(Color.BLACK);

                Raylib.DrawCircleGradient((int)centre.X,(int)centre.Y, W/2f-3, Color.DARKGREEN,Color.BLACK);
                Raylib.DrawCircleLines((int)centre.X, (int)centre.Y, W/2f-3, Color.GREEN);

                Raylib.DrawLineEx(centre, centre+lineDir*(W/2f-5), 4, Color.GREEN);
                foreach (Vector2 v in things) {
                    float angDif = v.X-angle;
                    Vector2 pos = unitCircle(v.X)*v.Y+centre;
                    if ((-2*PI*0.125f < angDif && angDif < 0) || (2*PI*0.875f < angDif && angDif < 2*PI)) {
                        float prop = (angDif+2*PI)%(2*PI*0.125f);
                        Raylib.DrawCircleV(pos, 5, Raylib.ColorAlpha(Color.GREEN, 1-rollOff(prop)) );
                    }
                }

                Raylib.DrawFPS(5,5);
                Raylib.EndDrawing();
            }
            Raylib.CloseWindow();
        }

        static Vector2 unitCircle(float angle) {
            return new Vector2(Sin(angle), Cos(angle));
        }

        static float rollOff(float x) {
            x += 0.2f;
            return Pow(0.25f, x)+1/(10*x)-0.3f;
        }
    }
}
