using Microsoft.VisualBasic;
using Raylib_cs;
using System.Numerics;


namespace racingGame {

    internal class Program {
        static void Main(string[] args) {
            int winW = 640, winH = 480;
            int scrW, scrH;
            bool fullScreen = false;

            float lookAng = 0;
            Console.Write("Input fov in degrees > ");
            float fov = int.Parse(Console.ReadLine()) * (MathF.PI/180);
            float turnspeed = 2*MathF.PI/3;

            Texture2D background;

            Raylib.SetConfigFlags(ConfigFlags.FLAG_WINDOW_RESIZABLE | ConfigFlags.FLAG_WINDOW_ALWAYS_RUN);
            Raylib.InitWindow(winW, winH, "Racing Game");
            Raylib.SetExitKey(KeyboardKey.KEY_NULL);

            background = Raylib.LoadTexture("textures/background e.png");

            {
                int display = Raylib.GetCurrentMonitor();
                scrW = Raylib.GetMonitorWidth(display);
                scrH = Raylib.GetMonitorHeight(display);
            }

            while (!Raylib.WindowShouldClose()) {
                float deltaT = Raylib.GetFrameTime();

                if (Raylib.IsKeyDown(KeyboardKey.KEY_A)) {
                    lookAng -= deltaT * turnspeed;
                }
                if (Raylib.IsKeyDown(KeyboardKey.KEY_D)) {
                    lookAng += deltaT * turnspeed;
                }
                lookAng = (lookAng+2*MathF.PI)%(2*MathF.PI);

                if (Raylib.IsWindowResized() && !Raylib.IsWindowFullscreen()) {
                    winW = Raylib.GetScreenWidth();
                    winH = Raylib.GetScreenHeight();
                }

                if (Raylib.IsKeyPressed(KeyboardKey.KEY_ENTER) && (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_ALT) || Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_ALT))) {
 		        	int display = Raylib.GetCurrentMonitor();
                    
                    fullScreen = !fullScreen;
                     
                    if (!fullScreen) {
                        Raylib.ToggleFullscreen();
                        Raylib.SetWindowSize(winW, winH);
                    } else {
                        scrW = Raylib.GetMonitorWidth(display);
                        scrH = Raylib.GetMonitorHeight(display);
                        Raylib.SetWindowSize(scrW, scrH);
                        Raylib.ToggleFullscreen();
                    }

 		        }


                Raylib.BeginDrawing();
                int pntW, pntH;
                if (fullScreen) {
                    pntW = scrW; pntH = scrH;
                } else {
                    pntW = winW; pntH = winH;
                }

                //draw background
                Raylib.ClearBackground(Color.BLACK);
                // int lookWidth = (int)(background.height*(float)pntW/pntH);
                // Rectangle sampleRect = new Rectangle(lookAng*(background.width/(2*MathF.PI))-lookWidth/2,0, lookWidth, background.height);
                // Raylib.DrawTexturePro(background, sampleRect, new Rectangle(0,0, pntW,pntH), Vector2.Zero, 0, Color.WHITE);

                float angL = lookAng-fov/2, angR = lookAng+fov/2;
                float edgeL = background.width*angL/(2*MathF.PI), edgeR = background.width*angR/(2*MathF.PI);
                int scaledHeight = (int)(pntH*((edgeR-edgeL)/pntW));
                int verticalMissing = background.height-scaledHeight;

                Raylib.DrawTexturePro(background, new Rectangle(edgeL, verticalMissing/2, edgeR-edgeL, scaledHeight), new Rectangle(0,0, pntW,pntH), Vector2.Zero, 0, Color.WHITE);

                //draw track
                for (int col = 0; col < pntW; col++) {
                    float ang = getAngleOfScreenColumn(col, pntW, fov, lookAng);

                    Color ground;
                    if (-MathF.PI*2/10f < ang && ang < MathF.PI*2/10f) {
                        ground = Color.DARKGRAY;
                    } else {
                        ground = Color.DARKGREEN;
                    }



                }

                Raylib.DrawText("Press Alt + Enter to Toggle full screen!", pntW/2 - 215 -1, 10 -1, 20, Color.BLACK);
                Raylib.DrawText("Press Alt + Enter to Toggle full screen!", pntW/2 - 215, 10, 20, Color.WHITE);

                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();

        }

        static float getAngleOfScreenColumn(int col, int pntW, float fov, float lookAng = 0) { 
            float screenProportion = (float)col/pntW;
            float angle = MathF.Atan((screenProportion*2 -1)*MathF.Tan(fov/2));
            angle = (angle+lookAng+(2*MathF.PI))%(2*MathF.PI);
            return angle;
        }
    }
}
