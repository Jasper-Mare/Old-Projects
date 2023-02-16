using System.Linq;
using System.Numerics;
using System.Threading;
using static System.MathF;
using System.Collections.Generic;
using Raylib_cs;

namespace CityGenerator2 {
    class Program {
        static int W = (int)(1920*0.85f), H = (int)(1080*0.85f);
        static void Main() {

            City city = new City(2500,2500);
            city.Create();

            Raylib.InitWindow(W, H, "");
            Raylib.SetExitKey(KeyboardKey.KEY_NULL);

            Camera2D camera = new Camera2D(new Vector2(W/2f, H/2f), Vector2.Zero, 0, 1);

            int currentRoad = 0;

            while (!Raylib.WindowShouldClose()) {
                float deltaT = Raylib.GetFrameTime();
                float speed = 100/camera.zoom;
                const float turnRate = 45;

                if (Raylib.IsKeyDown(KeyboardKey.KEY_W)) { camera.target.Y -= speed * deltaT * Cos(camera.rotation * PI / 180); camera.target.X -= speed * deltaT * Sin(camera.rotation * PI / 180); }
                if (Raylib.IsKeyDown(KeyboardKey.KEY_S)) { camera.target.Y += speed * deltaT * Cos(camera.rotation * PI / 180); camera.target.X += speed * deltaT * Sin(camera.rotation * PI / 180); }

                if (Raylib.IsKeyDown(KeyboardKey.KEY_A)) { camera.target.Y -= speed * deltaT * Cos(( camera.rotation + 90 ) * PI / 180); camera.target.X -= speed * deltaT * Sin(( camera.rotation + 90 ) * PI / 180); }
                if (Raylib.IsKeyDown(KeyboardKey.KEY_D)) { camera.target.Y += speed * deltaT * Cos(( camera.rotation + 90 ) * PI / 180); camera.target.X += speed * deltaT * Sin(( camera.rotation + 90 ) * PI / 180); }

                if (Raylib.IsKeyDown(KeyboardKey.KEY_Q)) { camera.rotation = ( camera.rotation + deltaT * turnRate + 360 ) % 360; }
                if (Raylib.IsKeyDown(KeyboardKey.KEY_E)) { camera.rotation = ( camera.rotation - deltaT * turnRate ) % 360; }

                if (Raylib.IsKeyReleased(KeyboardKey.KEY_BACKSPACE)) { camera.rotation = 0; }

                if (Raylib.IsKeyDown(KeyboardKey.KEY_R)) { camera.zoom *= 1 + deltaT; }
                if (Raylib.IsKeyDown(KeyboardKey.KEY_F)) { camera.zoom *= 1 - deltaT; }

                if (Raylib.IsKeyReleased(KeyboardKey.KEY_TAB)) { currentRoad = (currentRoad+1)%city.roads.Count; }

                bool debuginfo = Raylib.IsKeyDown(KeyboardKey.KEY_TAB);

                Thread.Sleep(0);

                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.BLACK);
                Raylib.BeginMode2D(camera);

                Vector2 Offset = new Vector2(-city.CityW*0.5f, -city.CityH*0.5f);

                Raylib.DrawRectangle((int)Offset.X-20, (int)Offset.Y-20, (int)(city.CityW)+40, (int)(city.CityH)+40, Color.WHITE);
                Raylib.DrawRectangle((int)Offset.X, (int)Offset.Y, (int)city.CityW, (int)city.CityH, Color.BLACK);

                foreach ((int a, int b) line in city.VoroniEdges) {
                    Raylib.DrawLineV(city.VoroniVertexes[line.a]+Offset, city.VoroniVertexes[line.b]+Offset, Color.RAYWHITE);
                }

                if (debuginfo) { 
                    foreach (int i in city.junctions) {
                        Raylib.DrawCircleV(city.VoroniVertexes[i]+Offset, 8/camera.zoom, Color.GREEN);
                    }
                }

                //foreach (Road rd in city.roads) {
                //    DrawRoad(rd, Offset);
                //}
                DrawRoad(city.roads[currentRoad], Offset);

                foreach ((int a, int b) i in city.VoroniEdges) {
                    Raylib.DrawLineV(city.VoroniVertexes[i.a]+Offset, city.VoroniVertexes[i.b]+Offset, Color.SKYBLUE);
                }

                Raylib.EndMode2D();
                Raylib.DrawFPS(5,5);
                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();


        }

        static void DrawRoad(Road road, Vector2 offset, bool showNodes = false) {
            Color colour = Color.WHITE;
            float size = 0;
            switch (road.type) {
                case RoadType.Boulevard:
                colour = Color.RED;
                size = 14;
                break;
                case RoadType.Avenue:
                colour = Color.ORANGE;
                size = 8;
                break;
                case RoadType.Street:
                colour = Color.YELLOW;
                size = 4;
                break;
                case RoadType.Passage:
                colour = Color.WHITE;
                size = 2;
                break;
            }

            Vector2[] points = road.getDrawPoints();
            Raylib.DrawCircleV(points[0]+offset, size*0.5f, colour);
            for (int i = 0; i < points.Length-1; i++) {
                Raylib.DrawCircleV(points[i+1]+offset, size*0.5f, colour);
                Raylib.DrawLineEx(points[i]+offset, points[i+1]+offset, size, colour);
            }

            if (showNodes) { 
                for (int i = 0; i < road.nodes.Count; i++) {
                    Raylib.DrawCircleV(road.nodes[i].pos-road.nodes[i].dir*(size+1)+offset, size*0.2f, Color.BLACK);
                    Raylib.DrawCircleV(road.nodes[i].pos+offset, size*0.4f, Color.BLACK);
                    Raylib.DrawCircleV(road.nodes[i].pos+road.nodes[i].dir*(size+1)+offset, size*0.2f, Color.BLACK);
                }
            }


        }


    }
}