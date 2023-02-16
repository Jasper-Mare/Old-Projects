using System.Numerics;
using Raylib_cs;

namespace vectorHashing {

    internal class Program {

        const int spacing = 1;

        static void Main(string[] args) {

            Raylib.InitWindow(1024,1024, "");

            int[,] colours = new int[1024,1024];

            int GetInRange(Vector2 vector2) {
                return (int)Raymath.Remap(vector2.GetHashCode(), int.MinValue, int.MaxValue, 0, 255);
            }

            for (int x = 0; x < 1024; x++) {
                for (int y = 0; y < 1024; y++) {

                    Vector2 vector2 = new Vector2(x/spacing, y/spacing);
                    colours[x, y] = (int)Raymath.Clamp(GetInRange(vector2), 0, 255);

                    //colours[x, y] = (int)Raymath.Clamp(
                    //    Raymath.Lerp(
                    //        Raymath.Lerp(GetColour(new Vector2(x/spacing, y/spacing+0)), GetColour(new Vector2(x/spacing+1, y/spacing+0)), (float)(x%spacing)/spacing),
                    //        Raymath.Lerp(GetColour(new Vector2(x/spacing, y/spacing+1)), GetColour(new Vector2(x/spacing+1, y/spacing+1)), (float)(x%spacing)/spacing),
                    //        (float)(y%spacing)/spacing),
                    //    0, 255);

                }
            }

            

            while (!Raylib.WindowShouldClose()) {
                



                Raylib.BeginDrawing();

                for (int x = 0; x < 1024; x++) {
                    for (int y = 0; y < 1024; y++) {
                        int val = colours[x,y];
                        Raylib.DrawPixel(x,y, new Color(val,val,val, 255));
                    }
                }

                Raylib.EndDrawing();

            }


            Raylib.CloseWindow();


        }

    }

}