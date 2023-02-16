using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using Raylib_cs;

namespace raycasting {
    class GameV2 {

        struct line {
            public float X1 { get; private set; }
            public float Y1 { get; private set; }
            public float X2 { get; private set; }
            public float Y2 { get; private set; }
            public float normalAngle { get; private set; }
            public float angle { get; private set; }
            public float CenterX { get; private set; }
            public float CenterY { get; private set; }

            public line(float x1, float y1, float x2, float y2) : this() {
                X1 = x1; Y1 = y1; X2 = x2; Y2 = y2;
                normalAngle = (float)(Math.Atan((X2 - X1) / (Y2 - Y1)) + (Math.PI / 2f));
                angle = normalAngle + 0.5f*(float)Math.PI;
                CenterX = (x1 + x2) / 2f; CenterY = (y1 + y2) / 2f;
            }
        }
        struct ray {
            public Vector2 position;
            public Vector2 dir { get { return new Vector2((float)Math.Sin(Ang), (float)Math.Cos(Ang)); } }
            Vector2 endPoint;
            public float Ang;
            public float distanceSQ { get; private set; }

            public ray(Vector2 pos, float angle) : this() {
                position = pos;
                endPoint = pos;
                Ang = angle;
            }

            public int cast(List<line> walls) {
                int closestindex = -1;
                int current = 0;
                foreach (line l in walls) {
                    current++;
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
                        if (closestindex != -1) {
                            if (distSquared(endPoint.X, endPoint.Y) > distSquared(newEndPoint.X, newEndPoint.Y)) {
                                endPoint = newEndPoint;
                                closestindex = current;
                            }
                        } else { //it doesn't have an end point yet
                            endPoint = newEndPoint;
                            closestindex = current;
                        }

                    } else { continue; }
                }
                distanceSQ = distSquared(endPoint.X, endPoint.Y);
                return closestindex;
            }
            public float distSquared(float x, float y) {
                return (x - position.X) * (x - position.X) + (y - position.Y) * (y - position.Y);
            }

        }

        bool recast = false;
        Vector2 ScreenSize = new Vector2(1600, 1200);
        List<line> walls = new List<line>();
        float[] distSqMap;
        int[] wallMap;
        int numRays;
        Vector2 playerpos;
        float playerAngle = 0;
        float distFromScreen = 1000;
        float ScreenSizeMult = 0.25f;
        bool ShowDebugInfo = false;
        bool ShowMap = false;

        public void Run() {

            //walls
            //lines.Add(new line(x1, y1, x2, y2));
            walls.Add(new line(0, 0, 0, 100));
            walls.Add(new line(0, 0, 100, 0));
            walls.Add(new line(0, 100, 100, 100));
            walls.Add(new line(100, 0, 100, 100));

            //create rays
            numRays = (int)ScreenSize.X;
            recast = true;

            playerpos = new Vector2(50,50);
            distSqMap = new float[numRays];
            wallMap = new int[numRays];

            Raylib.InitWindow((int)ScreenSize.X, (int)ScreenSize.Y, "");
            Raylib.SetExitKey(KeyboardKey.KEY_NULL);

            while (!Raylib.WindowShouldClose()) {
                float deltaT = Raylib.GetFrameTime();

                //cast
                if (recast) { 
                    for (int i = 0; i < numRays; i++) {
                        ray r = new ray(playerpos, playerAngle+MathF.Atan(((i-numRays/2f)*ScreenSizeMult)/distFromScreen));
                        wallMap[i] = r.cast(walls);
                        distSqMap[i] = r.distanceSQ;
                    }
                    recast = false;
                }

                if (Raylib.IsKeyDown(KeyboardKey.KEY_W)) { playerpos.Y += 60 * (float)Math.Cos(playerAngle) * deltaT; playerpos.X += 60 * (float)Math.Sin(playerAngle) * deltaT; recast = true; }
                if (Raylib.IsKeyDown(KeyboardKey.KEY_S)) { playerpos.Y -= 60 * (float)Math.Cos(playerAngle) * deltaT; playerpos.X -= 60 * (float)Math.Sin(playerAngle) * deltaT; recast = true; }
                if (Raylib.IsKeyDown(KeyboardKey.KEY_A)) { playerAngle -= 1 * deltaT; recast = true; }
                if (Raylib.IsKeyDown(KeyboardKey.KEY_D)) { playerAngle += 1 * deltaT; recast = true; }

                if(Raylib.IsKeyDown(KeyboardKey.KEY_UP)) { distFromScreen *= 1+deltaT; recast = true; }
                if(Raylib.IsKeyDown(KeyboardKey.KEY_DOWN)) { distFromScreen *= 1-deltaT; recast = true; }
                if(Raylib.IsKeyDown(KeyboardKey.KEY_LEFT)) { ScreenSizeMult *= 1+deltaT; recast = true; }
                if(Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT)) { ScreenSizeMult *= 1-deltaT; recast = true; }

                if(Raylib.IsKeyReleased(KeyboardKey.KEY_F3)) { ShowDebugInfo = !ShowDebugInfo; }
                if(Raylib.IsKeyReleased(KeyboardKey.KEY_M)) { ShowMap = !ShowMap; }

                //======================drawloop===================//
                Raylib.BeginDrawing();

                Raylib.ClearBackground(Color.BLACK);

                for (int i = 0; i < numRays; i++) {
                    float dist = (float)Math.Sqrt(distSqMap[i]);
                    float wallHeight = 10000/dist;

                    //Raylib.DrawLine(i,(int)(ScreenSize.Y/2f-wallHeight), i,(int)(ScreenSize.Y/2f+wallHeight), Color.BLUE);
                    Raylib.DrawLine(i,(int)(ScreenSize.Y/2f-wallHeight), i,(int)(ScreenSize.Y/2f+wallHeight), Raylib.ColorFromHSV(((wallMap[i]+1)*90)%360, 1,1));
                    Raylib.DrawLine(i,0, i,(int)ScreenSize.Y,new Color((byte)0,(byte)0,(byte)0,(byte)Math.Min(Math.Max(dist,0),255)));
                }

                if (ShowMap) {
                    foreach (line l in walls) {
                        Raylib.DrawLine((int)(ScreenSize.X/2f+l.X1), (int)(ScreenSize.Y/2f+l.Y1), (int)(ScreenSize.X/2f+l.X2), (int)(ScreenSize.Y/2f+l.Y2), Color.WHITE);
                    }
                    Raylib.DrawCircle((int)(ScreenSize.X/2f+playerpos.X),(int)(ScreenSize.Y/2f+playerpos.Y), 10, Color.RED);
                }

                if (ShowDebugInfo) {
                    Raylib.DrawFPS(5,5);
                    Raylib.DrawText("Dist From Screen: "+distFromScreen, 5, 30, 20, Color.LIME);
                    Raylib.DrawText("Screen Size: "+ScreenSizeMult, 5, 50, 20, Color.LIME);
                }

                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();

        }
    }
}
