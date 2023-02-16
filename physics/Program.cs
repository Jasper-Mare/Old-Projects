using Raylib_cs;
using System.Numerics;
using System.Collections.Generic;
using System.Threading;

namespace physics {
    static class Program {
        static bool running = true;
        static float physDeltaT = 0;

        static List<particle> particles = new List<particle>();

        static void Main(string[] args) {
            int w = 1024, h = 1024;
            Console.WriteLine("started");
            Random rng = new Random();

            //for (int i = 0; i < 50; i++) {
            //    particles.Add(new particle() {
            //        pos = new Vector2(rng.Next(0,100), rng.Next(0, 100)),
            //        vel = new Vector2((float)rng.NextDouble()-0.5f, (float)rng.NextDouble()-0.5f)*10,
            //        mass = (float)rng.NextDouble()+0.5f,
            //        col = Raylib.ColorFromHSV((float)rng.NextDouble()*360, 0.5f,1),
            //    });
            //}

            PhysicsLoop();
            Console.WriteLine("started physics");

            Raylib.InitWindow(w, h, "physics");
            Raylib.SetExitKey(KeyboardKey.KEY_NULL);

            while (running) {
                if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE)) {
                    Vector2 mousepoint = ScreenToPoint(Raylib.GetMousePosition(), w,h);
                    Color color = Raylib.ColorFromHSV((float)rng.NextDouble()*360, 0.5f,1);
                    for (int i = 0; i < 5; i++) {
                        particles.Add(new particle() {
                            pos = mousepoint + new Vector2((float)rng.NextDouble()-0.5f, (float)rng.NextDouble()-0.5f)*2,
                            vel = new Vector2((float)rng.NextDouble()-0.5f, (float)rng.NextDouble()-0.5f)*10,
                            mass = (float)rng.NextDouble()+0.5f,
                            col = color,
                        });
                    }
                }

                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.BLACK);

                Color gridCol = new Color(50,50,50, 255);
                for (int i = 1; i < 100; i++) {
                    int pos = (int)(i/100f*w);
                    Raylib.DrawLine(pos, 0, pos, h, gridCol);
                }

                for (int i = 1; i < 100; i++) {
                    int pos = (int)(i/100f*h);
                    Raylib.DrawLine(0, pos, w, pos, gridCol);
                }

                for (int i = 0; i < particles.Count; i++) {
                    Vector2 drawpos = PointToScreen(particles[i].pos, w, h);
                    //Raylib.DrawPixelV(drawpos, particles[i].col);
                    Raylib.DrawCircleV(drawpos, 3, particles[i].col);
                    //Raylib.DrawText(particles[i].acc.ToString(), (int)drawpos.X,(int)drawpos.Y, 15, Color.DARKBLUE);
                }

                if (Raylib.WindowShouldClose()) { running = false; }
                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        }

        static async void PhysicsLoop() {
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

            while (running) {
                float deltaT = (float)watch.Elapsed.TotalSeconds;
                physDeltaT = deltaT;
                watch.Restart();

                for (int a = 0; a < particles.Count; a++) {
                    particle tmp = particles[a];

                    tmp.pos += tmp.vel * deltaT;
                    tmp.vel += tmp.acc * deltaT;

                    tmp.acc = Vector2.Zero;

                    //tmp.pos.X = (tmp.pos.X+100)%100;
                    //tmp.pos.Y = (tmp.pos.Y+100)%100;

                    particles[a] = tmp;
                }

                for (int a = 0; a < particles.Count; a++) {
                    particle tmp = particles[a];

                    //tmp.acc = Vector2.UnitY*-2f; //gravity
                    //if (tmp.pos.Y < 0) { tmp.acc += Vector2.UnitY*4; } //floor

                    for (int b = 0; b < particles.Count; b++) {
                        if (a == b) { continue; }
                        //if ((particles[a].pos-particles[b].pos).LengthSquared() < 0.125f) {
                        //    particles.RemoveAt(b);
                        //    b--;
                        //    continue;
                        //}
                        tmp.acc += Force1(tmp.pos, particles[b].pos, tmp.mass);
                    }

                    if (tmp.pos.X < 0  ) { tmp.vel = Vector2.Reflect(tmp.vel, Vector2.UnitX); }
                    if (tmp.pos.Y < 0  ) { tmp.vel = Vector2.Reflect(tmp.vel, Vector2.UnitY); }
                    if (tmp.pos.X > 100) { tmp.vel = Vector2.Reflect(tmp.vel,-Vector2.UnitX); }
                    if (tmp.pos.Y > 100) { tmp.vel = Vector2.Reflect(tmp.vel,-Vector2.UnitY); }

                    particles[a] = tmp;
                }
                
                await Task.Yield();
            }


        }

        static Vector2 PointToScreen(Vector2 point, int w, int h) {
            point /= 100;
            point.Y = 1-point.Y;
            point *= new Vector2(w,h);

            return point;
        }
        static Vector2 ScreenToPoint(Vector2 point, int w, int h) {
            point /= new Vector2(w,h);
            point.Y = 1-point.Y;
            point *= 100;

            return point;
        }

        static Vector2 Force1(Vector2 apos, Vector2 bpos, float amass) {
            Vector2 dir = Vector2.Normalize(apos - bpos);
            if (!float.IsFinite(dir.X) || !float.IsFinite(dir.Y)) { dir = Vector2.Zero; }

            float dist = (apos-bpos).Length();
            float magnitude = 50*MathF.Exp(-1/amass*MathF.Abs(dist));

            Vector2 acc = dir*magnitude;

            return acc;
        }

    }

    struct particle {
        public float mass;
        public Vector2 pos;
        public Vector2 vel;
        public Vector2 acc;
        public Color col;
    }
}