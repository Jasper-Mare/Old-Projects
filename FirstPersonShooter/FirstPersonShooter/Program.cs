using Raylib_cs;
using System.Numerics;
using static System.MathF;

namespace FirstPersonShooter {
    static class Program {
        static void Main(string[] args) {
            ConsoleExtension.Hide();

            const int screenW = 2560;
            const int screenH = 1440;
            const float degToRad = PI/180;

            float fov = 70;
            float sensitivity = 0.001f;
            float playerSpeed = 10;

            float plrPitch = 0;
            float plrYaw   = 0;

            int oldMouseX;
            int oldMouseY;

            Vector3 cameraForwards = new Vector3(1,0,0);
            Vector3 playerForwards;
            Vector3 playerSideways;

            Camera3D camera = new Camera3D(new Vector3(0,2,0), cameraForwards, Vector3.UnitY, fov, CameraProjection.CAMERA_PERSPECTIVE);
            Raylib.SetCameraMode(camera,CameraMode.CAMERA_FIRST_PERSON);


            planet sun = new planet(0, 2, 1, 0, 0);
            sun.satalites = new planet[] {
                new planet(2, 0.05f, 2, 1, 2.25f), //mercury
                new planet(3, 0.1f, 1.75f, 0.9f, 2.75f), //venus
                new planet(1, 0.2f, 1, 0.85f, 3.5f), //earth
                new planet(3, 0.15f, 0.95f, 0.8f, 4f), //mars
                new planet(4, 0.65f, 0.45f, 0.6f, 7), //jupiter
                new planet(4, 0.55f, 0.35f, 0.55f, 8.5f), //saturn
                new planet(5, 0.4f, 0.5f, 0.45f, 10), //uranus
                new planet(5, 0.45f, 1, 0.4f, 11.5f), //neptune
                new planet(2, 0.025f, 10, 0.25f, 15), //pluto
            };
            sun.satalites[2].satalites = new planet[1] { new planet(2, 0.025f, 1, 1.5f, 0.25f) };

            Raylib.InitWindow(screenW,screenH,"");
            Raylib.HideCursor();
            Raylib.DisableCursor();
            oldMouseX = Raylib.GetMouseX();
            oldMouseY = Raylib.GetMouseY();

            Mesh sphere = Raylib.GenMeshSphere(1,32,32);

            Texture2D[] textures = new Texture2D[] {
                Raylib.LoadTexture(@"textures\sun.png"),
                Raylib.LoadTexture(@"textures\earth.png"),
                Raylib.LoadTexture(@"textures\moon.png"),
                Raylib.LoadTexture(@"textures\mars.png"),
                Raylib.LoadTexture(@"textures\jupiter.png"),
                Raylib.LoadTexture(@"textures\neptune.png"),
            };
            Material[] mats = new Material[textures.Length];

            for (int i = 0; i < textures.Length; i++) {
                mats[i] = Raylib.LoadMaterialDefault();
                Raylib.SetMaterialTexture(ref mats[i],0,textures[i]);
            }

            while(!Raylib.WindowShouldClose()) {
                //============update=============
                float deltaT = Raylib.GetFrameTime();

                plrYaw -= (oldMouseX-Raylib.GetMouseX()) * sensitivity;
                plrPitch -= (oldMouseY-Raylib.GetMouseY()) * sensitivity;

                if (plrPitch < 1*degToRad) { plrPitch = 2*degToRad; }
                if (plrPitch > 179*degToRad) { plrPitch = 178*degToRad; }

                cameraForwards = GetTarget(plrPitch,plrYaw, false);
                playerForwards = GetTarget(plrPitch,plrYaw, true);
                playerSideways = GetTarget(plrPitch,plrYaw+PI/2, true);

                if (Raylib.IsKeyDown(KeyboardKey.KEY_W)) { camera.position.Z += playerForwards.Z*playerSpeed*deltaT; camera.position.X += playerForwards.X*playerSpeed*deltaT; }
                if (Raylib.IsKeyDown(KeyboardKey.KEY_S)) { camera.position.Z -= playerForwards.Z*playerSpeed*deltaT; camera.position.X -= playerForwards.X*playerSpeed*deltaT; }

                if (Raylib.IsKeyDown(KeyboardKey.KEY_D)) { camera.position.Z += playerSideways.Z*playerSpeed*deltaT; camera.position.X += playerSideways.X*playerSpeed*deltaT; }
                if (Raylib.IsKeyDown(KeyboardKey.KEY_A)) { camera.position.Z -= playerSideways.Z*playerSpeed*deltaT; camera.position.X -= playerSideways.X*playerSpeed*deltaT; }

                camera.target = camera.position + cameraForwards;

                oldMouseX = Raylib.GetMouseX();
                oldMouseY = Raylib.GetMouseY();

                //============drawing============
                Raylib.BeginDrawing();

                Raylib.ClearBackground(Color.BLACK);

                Raylib.BeginMode3D(camera);

                //Raylib.DrawPlane(Vector3.UnitY*-0.01f,new Vector2(25),Color.RAYWHITE);
                Raylib.DrawGrid(50,1);

                //spawn planets
                planet.transforms = new System.Collections.Generic.List<Matrix4x4>();
                planet.textures = new System.Collections.Generic.List<int>();
                sun.calcMatrix(Raymath.MatrixTranslate(0,1.5f,0)*Raymath.MatrixRotateX(3.1415f/2), (float)Raylib.GetTime()+60);

                for (int i = 0; i < planet.transforms.Count; i++) {
                    Raylib.DrawMesh(sphere, mats[planet.textures[i]], planet.transforms[i]);
                }

                Raylib.EndMode3D();

                Raylib.DrawFPS(5,5);

                Raylib.EndDrawing();
            }

            Raylib.EnableCursor();
            Raylib.ShowCursor();
            Raylib.CloseWindow();

            ConsoleExtension.Show();
            System.Console.ReadLine();
        }

        class planet {
            public static System.Collections.Generic.List<Matrix4x4> transforms;
            public static System.Collections.Generic.List<int> textures;

            public int texture = 0;
            public float scale = 1;
            public float rotSpeed = 1;
            public float orbSpeed = 1;
            public float dist = 1;
            public planet[] satalites = new planet[0];

            public planet(int texture = 0, float scale = 1, float rotSpeed = 1, float orbSpeed = 1, float dist = 1) {
                this.texture = texture; this.scale = scale; this.rotSpeed = rotSpeed; this.orbSpeed = orbSpeed; this.dist = dist;
                satalites = new planet[0];
            }

            public void calcMatrix(Matrix4x4 parentTransform, float timeSinceStart) {
                Matrix4x4 myMatrix = parentTransform*Raymath.MatrixRotateZ(timeSinceStart*orbSpeed)*Raymath.MatrixTranslate(0,dist,0)*Raymath.MatrixScale(scale,scale,scale)*Raymath.MatrixRotateZ(timeSinceStart*rotSpeed);
                Matrix4x4 passOnMatrix = parentTransform*Raymath.MatrixRotateZ(timeSinceStart*orbSpeed)*Raymath.MatrixTranslate(0,dist,0);

                transforms.Add(myMatrix);
                textures.Add(texture);

                foreach (planet child in satalites) {
                    if (child is null) { continue; }
                    child.calcMatrix(passOnMatrix, timeSinceStart);
                }

            }

        }

        static Vector3 GetTarget(float pitch, float yaw, bool player) {
            if(player) { return new Vector3(Cos(yaw),0,Sin(yaw)); } 
            else { return new Vector3(Cos(yaw)*Sin(pitch),Cos(pitch),Sin(yaw)*Sin(pitch)); }
        }
    }
}
