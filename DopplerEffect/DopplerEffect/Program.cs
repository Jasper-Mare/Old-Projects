using System.Numerics;
using System.Collections.Generic;
using Raylib_cs;

namespace DopplerEffect {
    class Program {
        static void Main() {
            const int width = 2500, height = 900;
            float SourceX = 0, period = 0.05f;
            List<Vector3> wavefronts = new List<Vector3>();
            Vector2 listener = new Vector2(width/2, 4*height/5);


            Raylib.InitWindow(width, height, "");
            Raylib.InitAudioDevice();
            Sound beep = Raylib.LoadSound("beep.wav");
            
            float timeSinceLast = 1;
            while (!Raylib.WindowShouldClose()) {
                float deltaT = Raylib.GetFrameTime();

                SourceX += deltaT*130;
                if (SourceX > width*1.1f) {
                    SourceX -= width*1.1f;
                    wavefronts.Clear();
                }
                for (int i = wavefronts.Count-1; i >= 0; i--) {
                    Vector3 temp = wavefronts[i];
                    temp.Z += deltaT*150;
                    wavefronts[i] = temp;
                    //if (temp.Z > 255*4) { wavefronts.RemoveAt(i); }
                    float x = wavefronts[i].X, y = wavefronts[i].Y, z = wavefronts[i].Z;
                    float dist = (new Vector2(x,y)-listener).Length();
                    if(dist <= z) {
                        wavefronts.RemoveAt(i);
                        Raylib.PlaySoundMulti(beep);
                    }
                }
                timeSinceLast -= deltaT;
                if (timeSinceLast <= 0) {
                    timeSinceLast = (timeSinceLast+period*100)%period;
                    wavefronts.Add(new Vector3(SourceX, height/2f, 0));
                }

                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.BLACK);

                Raylib.DrawCircle((int)SourceX, height/2, 4, Color.LIME);
                wavefronts.ForEach(v => { Raylib.DrawCircleLines((int)v.X,(int)v.Y, v.Z, Color.YELLOW); });
                //wavefronts.ForEach(v => { Raylib.DrawCircleLines((int)v.X,(int)v.Y, v.Z, new Color(255,255,0, (int)Raymath.Clamp(256-v.Z/4,0, 255) )); });

                Raylib.DrawCircleV(listener, 6, Color.GRAY);

                Raylib.EndDrawing();
            }

            Raylib.CloseAudioDevice();
            Raylib.CloseWindow();

        }
    }
}
