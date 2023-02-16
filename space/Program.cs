using System;
using System.Numerics;
using Raylib_cs;

namespace space {
    internal class Program {
        static void Main(string[] args) {
            const int winW = 1024, winH = 1024;

            Raylib.InitWindow(winW, winH, "Universe");
            
            while (!Raylib.WindowShouldClose()) {

                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.BLACK);

                for (int x = 0; x < winW; x += 16) { 
                    for (int y = 0; y < winH; y += 16) {
                        Vector2 pos = new Vector2(x,y);

                        if ( ( (int)GetInRange(pos, 0, 100) & (int.MaxValue/64) ) != 0 ) { //no star
                            continue;
                        }

                        float radius = GetInRange(pos, 1, 10, x ^ y);
                        float temp   = GetInRange(pos, 0,  1, y << x);

                        //Raylib.DrawRectangleV(pos, Vector2.One*16, TempToColor(temp));

                        Raylib.DrawCircle(x, y, radius, TempToColor(temp));
                    }
                }


                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        }

        static Color TempToColor(float temp) {
            Color output = Color.WHITE;
            temp = Raymath.Remap(temp, 0,1, 15,150);

            if (temp < 66) output.r = 255;
            else {
                float red = temp - 60;
                red = 329.699f * MathF.Pow(red, -0.1332f);
                output.r = (byte)Raymath.Clamp(red, 0, 255);
            }

            if (temp < 66) { 
                float green = temp;
                green = 99.471f * MathF.Log(green)-161.12f;
                output.g = (byte)Raymath.Clamp(green, 0, 255);
            } else {
                float green = temp-60;
                green = 288.122f * MathF.Pow(green, -0.07551f);
                output.g = (byte)Raymath.Clamp(green, 0, 255);
            }

            if (temp > 66) output.b = 255;
            else if (temp < 19) {
                output.b = 0;
            } else {
                float blue = temp - 10;
                blue = 138.518f * MathF.Log(blue)-305.045f;
                output.b = (byte)Raymath.Clamp(blue, 0, 255);
            }

            return output;
        }

        static float GetInRange(Vector2 pos, float a, float b, int offset = 0) {
            return (float)Raymath.Remap(bitCycleLeft(pos.GetHashCode(), offset), int.MinValue, int.MaxValue, a, b);
        }

        static int bitCycleLeft(int value, int bits) {
            uint original = (uint)value;
            return (int)((original << bits) | (original >> (32 - bits)));
        }
    }
}
