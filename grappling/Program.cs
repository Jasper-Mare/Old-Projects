using Raylib_cs;
using static Raylib_cs.Raylib;
using System.Numerics;
using Math = System.MathF;

namespace grappling {
    
    internal class Program {
        
        static void Main(string[] args) {
            Vector2 windowSize = new Vector2(1200,675);

            Vector2 grappleHeadPos = windowSize/2;
            Vector2 grappleHeadVel = Vector2.Zero;
            Vector2 grappleHeadAcc = Vector2.Zero;
            float springConst = 10f;
            float restLength = 0;
            bool attached = false;
            bool fired = false;

            bool dead = false;
            float deadTime = 0;

            Vector2 playerPos = windowSize/2;
            Vector2 playerVel = Vector2.UnitY*-200;
            Vector2 playerAcc = Vector2.Zero;

            SetExitKey(KeyboardKey.KEY_NULL);

            SetConfigFlags(ConfigFlags.FLAG_MSAA_4X_HINT);
            InitWindow((int)windowSize.X, (int)windowSize.Y, "");

            Vector2[][] surfaces = new Vector2[][] {
                new Vector2[] { new Vector2(0, 0.2f), new Vector2(1, 0.15f), },
                new Vector2[] { new Vector2(0, 0.1f), new Vector2(0.25f, 0.3f), new Vector2(0.6f, 0.05f), new Vector2(1f, 0.2f), },
                new Vector2[] { new Vector2(0, 0.15f), new Vector2(0.5f, 0.27f), new Vector2(0.8f, 0.5f), new Vector2(1f, 0.2f), },
            };

            while (!WindowShouldClose()) {
                float deltaT = GetFrameTime();

                Vector2 dir = Vector2.Normalize(playerPos-grappleHeadPos);
                if (!dead) {
                    if (IsKeyPressed(KeyboardKey.KEY_SPACE)) {
                        if (attached) { 
                            attached = false;
                            fired = false;
                        } else {
                            grappleHeadPos = playerPos;
                            grappleHeadVel = playerVel*0.25f+Vector2.Normalize(GetMousePosition()-playerPos)*500;
                            
                            fired = true;
                            attached = false;
                        }
                    }

                    if (!fired) { grappleHeadPos = playerPos; }

                    if (!attached && fired) {
                        grappleHeadAcc = Vector2.UnitY*200;

                        grappleHeadPos += grappleHeadVel*deltaT;
                        grappleHeadVel += grappleHeadAcc*deltaT;
                    }

                    if (attached) {
                        //f = -k*x
                        float extension = (playerPos-grappleHeadPos).Length()-restLength;
                        Vector2 force = -springConst*Math.Max(0,extension)*dir;
                        playerAcc += force;
                    }
                    
                    playerAcc += Vector2.UnitY*200;
                    playerPos += playerVel*deltaT;
                    playerVel += playerAcc*deltaT;

                    if (playerPos.Y > windowSize.Y*2) { 
                        dead = true;
                    }

                } else {
                    deadTime += deltaT;
                    if (deadTime > 3) {
                        grappleHeadPos = windowSize / 2;
                        grappleHeadVel = Vector2.Zero;
                        attached = false;
                        fired = false;

                        dead = false;
                        deadTime = 0;

                        playerPos = windowSize / 2;
                        playerVel = Vector2.UnitY * -200;
                    }
                }

                grappleHeadAcc = Vector2.Zero;
                playerAcc = Vector2.Zero;

                BeginDrawing();
                ClearBackground(Color.SKYBLUE);

                //draw surfaces
                for (int s = 0; s < surfaces.Length; s++) {
                    if (surfaces[s].Length < 2) { continue; }
                    for (int p = 1; p < surfaces[s].Length; p++) {
                        Vector2 point1 = surfaces[s][p-1]*(windowSize+Vector2.One);
                        Vector2 point2 = surfaces[s][p  ]*(windowSize+Vector2.One);

                        for (int x = (int)point1.X; x < (int)point2.X; x++) {
                            float y = Raymath.Lerp(point1.Y,point2.Y, (x-point1.X)/(point2.X-point1.X));
                            DrawLine(x,0, x,(int)y, Color.DARKGRAY);

                            if (!attached && (int)grappleHeadPos.X == x && grappleHeadPos.Y < y) { //becomes attached
                                attached = true;
                                restLength = (playerPos-grappleHeadPos).Length()*0.85f;
                            }

                            if ((int)playerPos.X == x && playerPos.Y < y) {
                                playerVel.X = 0;
                                playerVel.Y *= Math.Sign(playerVel.Y);
                                DrawLine(x, 0, x, (int)y, Color.PINK);
                            }

                        }

                    }
                }

                DrawLineEx(playerPos, grappleHeadPos, 3, Color.BROWN);
                DrawCircleV(grappleHeadPos, 4, Color.GRAY);
                DrawCircleV(playerPos, 15, Color.GREEN);

                //DrawCircleV(grappleHeadPos+dir*restLength, 2, Color.MAGENTA);

                if (dead) { DrawText("Dead", (int)(windowSize.X*0.5f-70), (int)(windowSize.Y*0.5f-15), 30, Color.RED); }

                EndDrawing();

            }
            CloseWindow();


        }
    }
}