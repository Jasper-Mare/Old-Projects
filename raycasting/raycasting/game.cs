using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using Raylib_cs;

namespace raycasting {
    class Game {

        struct line {
            public float X1 { get; private set; }
            public float Y1 { get; private set; }
            public float X2 { get; private set; }
            public float Y2 { get; private set; }
            public float normalAngle { get; private set; }
            public float angle { get; private set; }
            public float CenterX { get; private set; }
            public float CenterY { get; private set; }
            public bool reflective;

            public line(float x1, float y1, float x2, float y2, bool mirror) : this() {
                X1 = x1; Y1 = y1; X2 = x2; Y2 = y2;
                normalAngle = (float)(Math.Atan((X2 - X1) / (Y2 - Y1)) + (Math.PI / 2f));
                angle = normalAngle + 0.5f * (float)Math.PI;
                CenterX = (x1 + x2) / 2f; CenterY = (y1 + y2) / 2f;
                reflective = mirror;
            }
        }
        struct ray {
            public Vector2 position;
            public Vector2 dir { get { return new Vector2((float)Math.Sin(ang), (float)Math.Cos(ang)); } }
            public Vector2 endPoint { get; private set; }
            public float ang;
            public float distanceSQ { get; private set; }

            public ray(Vector2 pos, float angle) : this() {
                position = pos;
                ang = angle;
                endPoint = pos;
            }

            public line cast(List<line> walls) {
                line closestL = walls[0];
                foreach (line l in walls) {
                    float x1 = l.X1;
                    float y1 = l.Y1;
                    float x2 = l.X2;
                    float y2 = l.Y2;

                    float x3 = position.X;
                    float y3 = position.Y;
                    float x4 = position.X + dir.X;
                    float y4 = position.Y + dir.Y;

                    float den = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
                    if (den == 0) {
                        continue;
                    }

                    float t = ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / den;
                    float u = -((x1 - x2) * (y1 - y3) - (y1 - y2) * (x1 - x3)) / den;

                    if (t > 0 && t < 1 && u > 0) {
                        Vector2 newEndPoint = new Vector2(x1 + t * (x2 - x1), y1 + t * (y2 - y1));
                        if (endPoint != position) {
                            if (distSquared(endPoint.X, endPoint.Y) > distSquared(newEndPoint.X, newEndPoint.Y)) {
                                endPoint = newEndPoint;
                                closestL = l;
                            }
                        } else { //it doesn't have an end point yet
                            endPoint = newEndPoint;
                            closestL = l;
                        }

                    } else { continue; }
                }
                distanceSQ = distSquared(endPoint.X, endPoint.Y);
                return closestL;
            }
            public float distSquared(float x, float y) {
                return (x - position.X) * (x - position.X) + (y - position.Y) * (y - position.Y);
            }

        }

        bool recast = false;
        List<line> lines = new List<line>();
        List<Stack<ray>> rayPaths = new List<Stack<ray>>();
        float numRays;
        bool raydrawing = false;
        float angleSpread = (float)(Math.PI / 180f) * 70;
        Vector2 playerpos;
        float playerAngle = 0;
        System.Drawing.Size Canvas = new System.Drawing.Size(1600, 1200);

        public void Run() {

            //walls
            //lines.Add(new line(x1, y1, x2, y2, mirror));
            lines.Add(new line(0, 0, 1, 1, false));

            playerpos = new Vector2(Canvas.Width / 4f, Canvas.Height / 2f);

            numRays = Canvas.Width / 2;
            for (int i = 0; i < numRays; i++) {
                rayPaths.Add(new Stack<ray>());
            }
            recast = true;

            Raylib.InitWindow(Canvas.Width, Canvas.Height, "");
            Raylib.SetExitKey(KeyboardKey.KEY_NULL);

            while (!Raylib.WindowShouldClose()) {
                float deltaT = Raylib.GetFrameTime();

                if (recast) {
                    for (int a = 0; a < rayPaths.Count; a++) { //a is the screen column
                        Stack<ray> tmpStack = new Stack<ray>();
                        tmpStack.Clear();
                        float Ang = (float)(a - rayPaths.Count / 2f) * (angleSpread / numRays);
                        tmpStack.Push(new ray(playerpos, (float)(Ang + playerAngle)));

                        while (true) {
                            while (raydrawing) { System.Threading.Thread.Sleep(1); }
                            ray r = tmpStack.Pop();
                            line collidedLine = r.cast(lines);
                            tmpStack.Push(r);
                            if (!collidedLine.reflective || tmpStack.Count > 20) { break; }
                            tmpStack.Push(new ray(r.endPoint, 2 * collidedLine.angle - r.ang));
                        }
                        rayPaths[a] = tmpStack;
                    }
                    recast = false;
                }

                if (Raylib.IsKeyDown(KeyboardKey.KEY_W)) { playerpos.Y += 60 * (float)Math.Cos(playerAngle) * deltaT; playerpos.X += 60 * (float)Math.Sin(playerAngle) * deltaT; recast = true; }
                if (Raylib.IsKeyDown(KeyboardKey.KEY_S)) { playerpos.Y -= 60 * (float)Math.Cos(playerAngle) * deltaT; playerpos.X -= 60 * (float)Math.Sin(playerAngle) * deltaT; recast = true; }
                if (Raylib.IsKeyDown(KeyboardKey.KEY_A)) { playerAngle += 1 * deltaT; recast = true; }
                if (Raylib.IsKeyDown(KeyboardKey.KEY_D)) { playerAngle -= 1 * deltaT; recast = true; }

                //======================drawloop===================//
                Raylib.BeginDrawing();

                Raylib.ClearBackground(Color.BLACK);

                foreach (line l in lines) {
                    if (!l.reflective) {
                        Raylib.DrawLine((int)l.X1, (int)l.Y1, (int)l.X2, (int)l.Y2, Color.GRAY);
                    } else {
                        Raylib.DrawLine((int)l.X1, (int)l.Y1, (int)l.X2, (int)l.Y2, Color.WHITE);
                    }
                }

                float Scale = (Canvas.Width / 2f) / numRays;
                for (int a = 0; a < rayPaths.Count; a++) {
                    float cumulativeDistSQ = 0;
                    for (int i = 0; i < rayPaths[a].Count; i++) {
                        cumulativeDistSQ += rayPaths[a].ElementAt(i).distanceSQ;
                    }
                    for (int i = 0; i < rayPaths[a].Count; i++) {
                        raydrawing = true;
                        ray r = rayPaths[a].ElementAt(rayPaths[a].Count - i - 1);
                        Raylib.DrawLine((int)r.position.X, (int)r.position.Y, (int)r.endPoint.X, (int)r.endPoint.Y, Color.YELLOW);
                        raydrawing = false;
                    }
                    raydrawing = true;
                    float Ang = (float)(a-rayPaths.Count/2f)*(angleSpread/numRays);
                    float dist = (float)(Math.Sqrt(cumulativeDistSQ) * Math.Cos(Ang));
                    float wallHeight = 10000 / dist;
                    
                    Raylib.DrawRectangle((int)(Canvas.Width/2f+(rayPaths.Count-a-1)*Scale), (int)(Canvas.Height/2f-wallHeight), (int)Scale, (int)(2*wallHeight), Color.BLUE);
                    //Raylib.DrawRectangle((int)(Canvas.Width/2f+(rayPaths.Count-a-1)*Scale), (int)(Canvas.Height/2f-wallHeight), (int)Scale, (int)(2*wallHeight), Raylib.ColorFromHSV((rayPaths[a].Count/20f)*360, 1, 1));
                    Raylib.DrawRectangle((int)(Canvas.Width/2f+(rayPaths.Count-a-1)*Scale), (int)(Canvas.Height/2f-wallHeight), (int)Scale, (int)(2*wallHeight), new Color((byte)0, (byte)0, (byte)0, (byte)Math.Min(Math.Max(dist,0)/6,255)));
                    raydrawing = false;
                }
                Raylib.DrawCircle((int)playerpos.X, (int)playerpos.Y, 20, Color.RED);

                Raylib.DrawFPS(5,5);
                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();

        }
    }
}
