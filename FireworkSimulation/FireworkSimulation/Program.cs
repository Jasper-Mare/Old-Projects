using System;
using System.Numerics;
using System.Collections.Generic;
using Raylib_cs;

namespace FireworkSimulation {
    class Program {
        static void Main(string[] args) {
            int Width = 1200, Height = 1200;
            float timeScale = 4, timeToNext = 0;
            Vector2 middle = new Vector2(0.5f, 0.5f);
            Random rng = new Random();

            List<Particle> Fireworks = new List<Particle>();
            List<Particle> Sparks = new List<Particle>();

            Raylib.InitWindow(Width,Height, "Fireworks");
            
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.BLACK);
            Raylib.EndDrawing();

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.BLACK);
            Raylib.EndDrawing();

            while (!Raylib.WindowShouldClose()) { 
                float deltaT = Raylib.GetFrameTime()/timeScale;

                timeToNext -= deltaT;
                if (timeToNext < 0) {
                    timeToNext = (float)rng.NextDouble()*0.25f;

                    float angle = MathF.PI*0.15f*((float)rng.NextDouble()-0.5f)+MathF.PI/2;
                    float mag = 3.0f+(float)rng.NextDouble();

                    Fireworks.Add(new Particle(){ 
                        pos = new Vector2((float)rng.NextDouble(), 0),
                        vel = new Vector2(MathF.Cos(angle)*mag, MathF.Sin(angle)*mag),
                        col = Raylib.ColorFromHSV(rng.Next(0,360),1,1),
                        lifetime = 100
                    });
                }

                for (int i = Fireworks.Count-1; i >= 0; i--) {
                    //Fireworks[i].acc = Vector2.Normalize(Fireworks[i].pos-middle)*-9.81f;
                    Fireworks[i].acc = Vector2.UnitY*-9.81f;
                    Fireworks[i].Update(deltaT);
                    //if (Fireworks[i].vel.LengthSquared() <= 0.1f) {
                    if(Fireworks[i].vel.Y <= 0.25f) {
                        int num = rng.Next(50,150);
                        for (int a = 0; a < num; a++) {
                            float angle = (a/(float)num)*2*MathF.PI;
                            float mag = (float)(rng.NextDouble()+1)*0.5f;
                            Sparks.Add(new Particle() {
                                pos = Fireworks[i].pos,
                                vel = new Vector2(MathF.Cos(angle)*mag, MathF.Sin(angle)*mag)+Fireworks[i].vel*0.5f+Vector2.UnitY*0.5f,
                                col = Fireworks[i].col,
                                lifetime = 0.25f
                            });
                        }
                        Fireworks.RemoveAt(i); 
                    } else if (Fireworks[i].pos.Y <= 0) { Fireworks.RemoveAt(i); }

                }

                for (int i = Sparks.Count-1; i >= 0; i--) {
                    //Sparks[i].acc = Vector2.Normalize(Sparks[i].pos-middle)*-2.45f;
                    Sparks[i].acc = Vector2.UnitY*-9.81f;
                    Sparks[i].Update(deltaT);
                    if (Sparks[i].lifetime < 0) { Sparks.RemoveAt(i); }

                }

                Raylib.BeginDrawing();
                Raylib.DrawRectangle(0,0,Width,Height,new Color(0,0,0, 8));
                
                foreach (Particle F in Fireworks) {
                    Raylib.DrawCircle((int)Raymath.Remap(F.pos.X, 0, 1, 0, Width), (int)Raymath.Remap(1-F.pos.Y, 0, 1, 0, Height), 2, F.col);
                }

                foreach(Particle S in Sparks) {
                    Raylib.DrawCircle((int)Raymath.Remap(S.pos.X,0,1,0,Width),(int)Raymath.Remap(1-S.pos.Y,0,1,0,Height),2,new Color(S.col.r,S.col.g,S.col.b, (int)(200*(S.lifetime))));
                }

                Raylib.DrawRectangle(0, Height-70, 150, 70, Color.BLACK);
                Raylib.DrawRectangleLines(0, Height-70, 150, 70, Color.DARKGRAY);
                Raylib.DrawFPS(5, Height-25);
                Raylib.DrawText("Fireworks: "+Fireworks.Count.ToString(), 5, Height-45, 20, Color.LIME);
                Raylib.DrawText("Sparks: "+Sparks.Count.ToString(), 5, Height-65, 20, Color.LIME);

                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();

        }
    }
}

class Particle {
    public Vector2 pos;
    public Vector2 vel;
    public Vector2 acc;
    public float lifetime;
    public Color col;

    public void Update(float deltaT) {
        vel += acc*deltaT;
        pos += vel*deltaT;
        lifetime -= deltaT;
    }

}
