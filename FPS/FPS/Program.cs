using System.Numerics;
using Raylib_cs;
//using System;

namespace FPS {
    class Program {
        static void Main(string[] args) {
            System.Random rng = new System.Random();

            Camera3D cam = new Camera3D(Vector3.UnitY, Vector3.UnitY + Vector3.UnitZ, Vector3.UnitY, 60, CameraProjection.CAMERA_PERSPECTIVE);

            const int Width = 1600, Height = 1200;
            const float UIscale = 2;

            float playerAng = 0;
            float playerspeed = 5f;
            float playerYVel = 0;
            const float grav = 9.81f;
            bool grounded;
            float timeWalking = 0;
            float attackTime = 0;
            const float timeToKill = 0.5f;
            Vector3 playerPos = cam.position;

            Prey[] prey = new Prey[5];
            for (int i = 0; i < prey.Length; i++) {
                prey[i] = new Prey(new Vector3(rng.Next(-200,200)/100f, 0, rng.Next(-200,200)/100f), 2);
            }
            Vector2[,] spritePoints = new Vector2[5,4] { //frame, angle
                { new Vector2(1,1  ), new Vector2(36,1  ), new Vector2(71,1  ), new Vector2(106,1  ) },
                { new Vector2(1,58 ), new Vector2(36,58 ), new Vector2(71,58 ), new Vector2(106,58 ) },
                { new Vector2(1,115), new Vector2(36,115), new Vector2(71,115), new Vector2(106,115) },
                { new Vector2(1,172), new Vector2(36,172), new Vector2(71,172), new Vector2(106,172) },
                { new Vector2(1,229), new Vector2(36,229), new Vector2(71,229), new Vector2(106,229) },
            };

            Raylib.InitWindow(Width,Height,"");
            Raylib.DisableCursor();
            
            Texture2D preyTextureMap = Raylib.LoadTexture(@"textures\preySheet.png");

            Texture2D[] hands = new Texture2D[] { Raylib.LoadTexture(@"textures\hand.png"), Raylib.LoadTexture(@"textures\hand2.png") };
            float oldMouseX = Raylib.GetMouseX();
            while (!Raylib.WindowShouldClose()) {
                float deltaT = Raylib.GetFrameTime();

                int frameNum = (int)(Raylib.GetTime()*10)%4;

                grounded = false;
                if (playerPos.Y <= 1) { playerPos.Y = 1; grounded = true; }

                playerYVel -= grav*deltaT;
                playerPos += Vector3.UnitY*deltaT*playerYVel;
                if (grounded) { playerYVel = 0;}

                Vector3 camForwards = Raymath.Vector3RotateByQuaternion(Vector3.UnitX, Raymath.QuaternionFromAxisAngle(Vector3.UnitY, playerAng));
                Vector3 camSideways = Raymath.Vector3RotateByQuaternion(Vector3.UnitZ, Raymath.QuaternionFromAxisAngle(Vector3.UnitY, playerAng));
                
                bool moving = false;
                if (Raylib.IsKeyDown(KeyboardKey.KEY_W)) { playerPos += camForwards*deltaT*playerspeed; moving = true; }
                if (Raylib.IsKeyDown(KeyboardKey.KEY_S)) { playerPos -= camForwards*deltaT*playerspeed; moving = true; }
                if (Raylib.IsKeyDown(KeyboardKey.KEY_A)) { playerPos -= camSideways*deltaT*playerspeed; moving = true; }
                if (Raylib.IsKeyDown(KeyboardKey.KEY_D)) { playerPos += camSideways*deltaT*playerspeed; moving = true; }
                if (moving) { timeWalking += deltaT*10; } else if (timeWalking >= 0) { timeWalking = (timeWalking-deltaT*10)%(3.1415f); }

                if (Raylib.IsKeyDown(KeyboardKey.KEY_SPACE) && grounded) { if (Raylib.IsKeyDown(KeyboardKey.KEY_W)) { playerYVel = 6.5f; } else { playerYVel = 5; } }

                if (attackTime == 0 && Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON)) { attackTime += deltaT; }
                if (attackTime != 0) { if(attackTime <= timeToKill) { attackTime += deltaT; } else {
                        //attack
                        attackTime = 0;
                        foreach (Prey p in prey) {
                            if (p.dead) { return; }
                            if ((p.position-playerPos).LengthSquared() <= 2) { p.dead = true; }
                        }
                        
                    }
                }

                playerAng += ((oldMouseX-Raylib.GetMouseX())/Width);
                oldMouseX = Raylib.GetMouseX();

                cam.position = playerPos+(Vector3.UnitY*System.MathF.Cos(timeWalking)*0.05f);
                cam.target = camForwards+cam.position;

                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.SKYBLUE);

                Raylib.BeginMode3D(cam);

                Raylib.DrawGrid(20,1);
                Raylib.DrawCube(new Vector3(0,2,10), 10, 2,5,Color.DARKGRAY);
                Raylib.DrawCubeWires(new Vector3(0,2,10), 10, 2,5,Color.BLACK);

                foreach (Prey p in prey) {
                    p.Move(deltaT, playerPos);
                    if (p.dead) {
                        Raylib.DrawCube(p.position, 0.25f, 0.25f, 0.25f, Color.RED);
                    } else { 
                        Raylib.DrawBillboardRec(cam,preyTextureMap, new Rectangle(spritePoints[4,frameNum].X, spritePoints[4,frameNum].Y, 34,56),p.position+Vector3.UnitY*0.75f,1,Color.WHITE);
                    }
                }

                Raylib.EndMode3D();

                Raylib.DrawTextureEx(hands[0], new Vector2(Width+(-hands[0].width+100+(int)(System.MathF.Sin(-timeWalking+1)*10))*UIscale,Height+(-hands[0].height+100+(int)(System.MathF.Sin(timeWalking)*15)-System.MathF.Sin(attackTime*3.1415f/timeToKill)*30)*UIscale), 0, 2, Color.WHITE);
                Raylib.DrawTextureEx(hands[1], new Vector2((-100-(int)(System.MathF.Sin(-timeWalking+1)*15))*UIscale,Height+(-hands[1].height+100+(int)(System.MathF.Sin(timeWalking)*10)-System.MathF.Sin(attackTime*3.1415f/timeToKill)*30)*UIscale), 0, 2, Color.WHITE);

                Raylib.DrawText(cam.position.ToString(), (int)(5*UIscale),(int)(5*UIscale),(int)(10*UIscale), Color.BLACK);
                Raylib.DrawText(attackTime.ToString(),(int)(5*UIscale),(int)(15*UIscale),(int)(10*UIscale), Color.BLACK);

                Raylib.EndDrawing();


            }
            Raylib.EnableCursor();
            Raylib.CloseWindow();
        }
    }

    class Prey {
        public Vector3 position;
        public readonly float speed; 
        public bool dead = false;
        private Vector3 dest;
        private System.Random rng;

        public Prey(Vector3 position, float speed) {
            this.position = position;
            this.speed = speed;
        }

        public void Move(float deltaT, Vector3 playerPos) {
            if (dead) { return; }
            Vector3 moveDir = position - playerPos;
            if (Raymath.Vector3Length(moveDir) > 10) { 
                
            } else {
                moveDir = Raymath.Vector3Normalize(moveDir);
                moveDir.Y = 0;
                position += moveDir * deltaT * speed;
            }
            
        }
    }
}
