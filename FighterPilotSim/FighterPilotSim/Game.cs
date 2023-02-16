using System.Numerics;
using static System.MathF;
using Raylib_cs;

namespace FighterPilotSim {
    class Game {
        /// <summary>
        /// Runs the game.
        /// </summary>
        /// <returns>Whether the game should restart once closing</returns>
        public bool Run() {


            Raylib.InitWindow(Options.Width, Options.Height, "Jet Fighter - " + Options.playerName);
            Raylib.SetExitKey(KeyboardKey.KEY_NULL);
            Resources.Setup();
            Raylib.SetWindowIcon(Resources.Icon);

            Camera3D camera = new Camera3D(Vector3.One*20, Vector3.Zero, Vector3.UnitY, 70, CameraProjection.CAMERA_PERSPECTIVE);


            float time = 0;
            while (!Raylib.WindowShouldClose()) {
                float deltaT = Raylib.GetFrameTime();
                time += deltaT;




                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.SKYBLUE);


                Raylib.BeginMode3D(camera);
                Raylib.DrawModelEx(Resources.Plane, Vector3.Zero, Vector3.UnitY, time*4, Vector3.One, Color.WHITE);
                Raylib.EndMode3D();


                Raylib.EndDrawing();

            }

            Raylib.CloseWindow();

            return false;
        }

    }
}