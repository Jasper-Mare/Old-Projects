using System;
using System.Numerics;
using System.Collections.Generic;
using Raylib_cs;

namespace bezierCurves {
    class Program {
        static void Main(string[] args) {
            int width = 800, height = 600;
            Vector2 midpoint = new Vector2(width/2, height/2);
            Random rng = new Random();
            List<Vector2> points = new List<Vector2>();
            int AnchorPoints = 4;

            for(int i = 0; i < AnchorPoints; i++) {
                //points.Add(new Vector2(rng.Next(width/6,5*width/6), rng.Next(height/6, 5*height/6) ));
                points.Add(new Vector2(MathF.Cos(i/(float)AnchorPoints*2*MathF.PI), MathF.Sin(i/(float)AnchorPoints*2*MathF.PI))*rng.Next(20,180)+midpoint);
            }
            
            Raylib.InitWindow(width, height, "Beziers");

            while (!Raylib.WindowShouldClose()) {

                int indexOfSelected = -1;
                if (Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT)) {
                    float closestToMouseDist = MathF.Max(10, Raylib.GetMouseDelta().Length()+30);
                    for (int i = 0; i < points.Count; i++) {
                        float leng = (points[i]-Raylib.GetMousePosition()).Length();
                        if (leng < closestToMouseDist) {
                            indexOfSelected = i;
                            closestToMouseDist = leng;
                        }
                    }

                    if (indexOfSelected != -1) {
                        int mx = Raylib.GetMouseX()+5, my = Raylib.GetMouseY()+5;
                        points[indexOfSelected] = new Vector2((int)(mx/10)*10, (int)(my/10)*10);
                    }

                }

                if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE)) {
                    Raylib.DrawText("Reseting", 5,5, 10, Color.LIME);
                    points.Clear();
                    for(int i = 0; i < AnchorPoints; i++) {
                        points.Add(new Vector2(MathF.Cos(i/(float)AnchorPoints*2*MathF.PI),MathF.Sin(i/(float)AnchorPoints*2*MathF.PI))*100+midpoint);
                    }
                }

                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.BLACK);

                for (float x = 5; x < width; x += 10) { 
                    for (float y = 5; y < height; y += 10) {
                        Raylib.DrawPixelV(new Vector2(x,y), Color.GRAY);
                    }
                }

                for (int i = 1; i < points.Count; i++) {
                    Raylib.DrawLineV(points[i-1], points[i], Color.DARKGRAY);
                }

                int index = 0;
                foreach (Vector2 p in points) {
                    if (index == indexOfSelected) {
                        Raylib.DrawCircleV(p, 6, Color.RED);
                    } else {
                        Raylib.DrawCircleV(p, 3, Color.LIME);
                    }
                    index++;
                }

                Vector2 last = GetBezierPoint(points.ToArray(), 0, 0);
                float gap = 0.01f;
                for (float p = gap; p <= 1.001f; p += gap) {
                    Vector2 point = GetBezierPoint(points.ToArray(), p, 0);
                    Raylib.DrawLineV(point, last, Color.RAYWHITE);
                    last = point;
                }

                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        }

        static Vector2 GetBezierPoint(Vector2[] points, float p, int depth) {
            if (points.Length == 1) {
                return points[0];
            } else if (points.Length == 2) {
                float x = Raymath.Lerp(points[0].X, points[1].X, p),
                      y = Raymath.Lerp(points[0].Y, points[1].Y, p);
                //Raylib.DrawLineV(points[0], points[1], Color.LIME);
                return new Vector2(x,y);
            } else { //quadratic
                Vector2 a = GetBezierPoint(points[0..(points.Length-1)], p, depth+1),
                        b = GetBezierPoint(points[1.. points.Length   ], p, depth+1),
                        c = GetBezierPoint(new Vector2[] {a,b}, p, depth+1);
                return c;
            } 
                
        }

    }
}
