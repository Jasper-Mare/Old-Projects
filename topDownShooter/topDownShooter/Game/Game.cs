using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace topDownShooter.Game {
    class Game {
        GameOptions options;

        public Game(GameOptions options) {
            this.options = options;
        }

        public void Run() {
            bool exit = false;
            Camera2D cam = new Camera2D(new Vector2(400,300), new Vector2(), 0, 1);
            World world = new World(20,20);
            float timeSinceShot = -1;
            Vector2 PlayerPos = new Vector2(0,0);
            Vector2 startPos = new Vector2(0,0), targetDir = new Vector2(0,0);

            float shakeT = 0;
            Vector2 shakedir = Vector2.Zero;
            float shakeDur = 0;

            Rectangle floorTexBound = new Rectangle(0, 0, 1000, 1000);
            Texture2D floorTexture;

            InitWindow(800, 600, "Top-Down Shooter");

            floorTexture = LoadTextureFromImage(GenImageCellular((int)floorTexBound.width,(int)floorTexBound.height, 10));

            SetExitKey(KeyboardKey.KEY_NULL);
            while (!exit) {
                float deltaT = GetFrameTime();

                if (timeSinceShot >= 0) {
                    timeSinceShot += deltaT;
                }
                if (timeSinceShot > 0.1f) { timeSinceShot = -1; }
                if (timeSinceShot == -1 && IsMouseButtonDown(MouseButton.MOUSE_LEFT_BUTTON)) { 
                    timeSinceShot = 0;
                    startPos = PlayerPos;
                    targetDir = Vector2.Normalize(GetMousePosition()-(cam.offset+cam.target)+startPos);
                    shakeT = 0;
                    shakeDur = GetRandomValue(1, 10)*0.01f;
                    shakedir = Raymath.Vector2Rotate(targetDir, GetRandomValue(-20, 20))*5;
                }

                cam.target = PlayerPos;

                if (shakeT < shakeDur) { 
                    shakeT += deltaT;
                    Vector2 shake = shakedir * (System.MathF.Abs(shakeT/shakeDur -0.5f)*-2+1);
                    cam.target += shake;
                }

                if (IsKeyDown(KeyboardKey.KEY_W)) { PlayerPos.Y += 30*deltaT; }
                if (IsKeyDown(KeyboardKey.KEY_S)) { PlayerPos.Y -= 30*deltaT; }
                if (IsKeyDown(KeyboardKey.KEY_A)) { PlayerPos.X += 30*deltaT; }
                if (IsKeyDown(KeyboardKey.KEY_D)) { PlayerPos.X -= 30*deltaT; }


                BeginDrawing();
                BeginMode2D(cam);

                DrawTextureTiled(floorTexture, floorTexBound, new Rectangle(PlayerPos.X-floorTexBound.width, PlayerPos.Y-floorTexBound.height, 2*floorTexBound.width, 2*floorTexBound.height), -PlayerPos, 0, 1, Color.WHITE);

                if (0 <= timeSinceShot && timeSinceShot <= 0.1f ) {
                    DrawLineV(startPos, startPos+targetDir*1000, Color.YELLOW);
                }

                DrawCubeWires(Vector3.Zero, 20, 20, 20, Color.YELLOW);
                DrawCircleV(PlayerPos, 10, Color.RED);
                EndMode2D();

                DrawText(PlayerPos.ToString(), 0,0, 30, Color.BLUE);

                EndDrawing();
                exit = WindowShouldClose();
            }
            CloseWindow();

        }

    }

    struct GameOptions {
        
    }

}
