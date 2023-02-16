using System.Linq;
using System.Numerics;
using System.Threading;
using static System.MathF;
using System.Collections.Generic;
using Raylib_cs;

namespace CityGenerator {
    class Program {
        const int W = 2048, H = 1152;
        
        static void Main() {
            Thread GenerateThread = new Thread(Generate);
            GenerateThread.Start();

            Raylib.InitWindow(W, H, "");
            Raylib.SetExitKey(KeyboardKey.KEY_NULL);

            Camera2D camera = new Camera2D(new Vector2(W/2f, H/2f), Vector2.Zero, 0, 1);

            while (!Raylib.WindowShouldClose()) {
                float deltaT = Raylib.GetFrameTime();
                float speed = 100/camera.zoom;
                const float turnRate = 45;

                if (Raylib.IsKeyDown(KeyboardKey.KEY_W)) { camera.target.Y -= speed*deltaT*Cos(camera.rotation*PI/180); camera.target.X -= speed*deltaT*Sin(camera.rotation*PI/180); }
                if (Raylib.IsKeyDown(KeyboardKey.KEY_S)) { camera.target.Y += speed*deltaT*Cos(camera.rotation*PI/180); camera.target.X += speed*deltaT*Sin(camera.rotation*PI/180); }

                if (Raylib.IsKeyDown(KeyboardKey.KEY_A)) { camera.target.Y -= speed*deltaT*Cos((camera.rotation+90)*PI/180); camera.target.X -= speed*deltaT*Sin((camera.rotation+90)*PI/180); }
                if (Raylib.IsKeyDown(KeyboardKey.KEY_D)) { camera.target.Y += speed*deltaT*Cos((camera.rotation+90)*PI/180); camera.target.X += speed*deltaT*Sin((camera.rotation+90)*PI/180); }

                if (Raylib.IsKeyDown(KeyboardKey.KEY_Q)) { camera.rotation = (camera.rotation+deltaT*turnRate+360)%360; }
                if (Raylib.IsKeyDown(KeyboardKey.KEY_E)) { camera.rotation = (camera.rotation-deltaT*turnRate)%360; }

                if (Raylib.IsKeyDown(KeyboardKey.KEY_R)) { camera.zoom *= 1+deltaT; }
                if (Raylib.IsKeyDown(KeyboardKey.KEY_F)) { camera.zoom *= 1-deltaT; }

                if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE)) { resume = !resume; }
                if (Raylib.IsKeyPressed(KeyboardKey.KEY_BACKSPACE) && !GenerateThread.IsAlive) { GenerateThread = new Thread(Generate); GenerateThread.Start(); }

                Thread.Sleep(0);

                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.BLACK);
                Raylib.BeginMode2D(camera);

                if (!(Regions is null)) { 
                    foreach (Region reg in Regions.GetLeafNodes().ToList() ) {
                        //Raylib.DrawLineStrip(reg.ToArray(), reg.Count, Color.YELLOW);
                        Raylib.DrawLineStrip(reg.GetForDrawing(), reg.Count+1, Color.RAYWHITE);
                    }
                }

                Raylib.EndMode2D();
                Raylib.EndDrawing();
            }
            exit = true;
            Raylib.CloseWindow();
        }


        static Tree<Region> Regions;
        static bool resume = true;
        static bool exit = false;
        static void Generate() {
            System.Random rng = new System.Random();

            void WaitThread(bool forceStop, int sleepTime = 0) {
                Thread.Sleep(sleepTime);
                if (forceStop) { 
                    resume = false;
                }
                while (!resume) {
                    if (exit) { return; }
                    Thread.Sleep(0);
                }
            }

            //the city boundries
            {
                Region startRegion = new Region();
                //startRegion.GenerateRandom(W/2);
                startRegion.AddRange(new Vector2[] { new Vector2(-500, -500), new Vector2(500, -500), new Vector2(500, 500), new Vector2(-500, 500) });
                Regions = new Tree<Region>(startRegion);
                
            }
            if (exit) { return; }
            WaitThread(true);

            //split in roads
            for (int i = 0; i < 10; i++) {
                List<int> leaves = Regions.GetLeafNodeIndexes();
                while (leaves.Count > 0) {
                    int currentSplittingIndex = leaves[rng.Next(0, leaves.Count)];
                    leaves.Remove(currentSplittingIndex);

                    if (rng.NextDouble() < 0.25f) { continue; }

                    Region[] splits = Regions[currentSplittingIndex].Split(10/(i+1));

                    Regions.Add(splits[0], currentSplittingIndex);
                    Regions.Add(splits[1], currentSplittingIndex);

                    if (exit) { return; }
                    WaitThread(false, 0);
                }
            }

            if (exit) { return; }
            WaitThread(true);

            //split buildings ect
            for (int i = 0; i < 4; i++) {
                List<int> leaves = Regions.GetLeafNodeIndexes();
                while (leaves.Count > 0) { 
                    int currentSplittingIndex = leaves[rng.Next(0, leaves.Count)];
                    leaves.Remove(currentSplittingIndex);

                    if (rng.NextDouble() < 0.1f) { continue; }

                    Region[] splits = Regions[currentSplittingIndex].Split();

                    Regions.Add(splits[0], currentSplittingIndex);
                    Regions.Add(splits[1], currentSplittingIndex);

                    if (exit) { return; }
                    WaitThread(false, 0);
                }
            }


        }

    }

    class Tree<T> {
        List<(T Data, List<int> Children)> values;

        public Tree(T startNode) {
            values = new List<(T Data, List<int> Children)>();
            values.Add((startNode, new List<int>() ));
        }

        public T this[int index] {
            get {
                return values[index].Data;
            }
        }

        public List<int> GetChildren(int index) {
            return values[index].Children;
        }

        public void Add(T Data, int parentId) {
            values.Add((Data, new List<int>() ));
            values[parentId].Children.Add(values.Count-1);
        }

        public List<T> GetLeafNodes() {
            return values.Where(n => n.Children.Count == 0).Select(n => n.Data).ToList();
        }

        public List<int> GetLeafNodeIndexes() {
            List<int> output = new List<int>();

            for (int i = 0; i < values.Count; i++) {
                if (values[i].Children.Count == 0) {
                    output.Add(i);
                }
            }

            return output;
        }
    }

    class Region : List<Vector2> {
        Vector2 calculateCentre() {
            Vector2 sum = Vector2.Zero;
            foreach (Vector2 vec in this) {
                sum += vec;
            }
            return sum/Count;
        }

        public Vector2[] GetForDrawing(float inset = 0) {
            if (Count == 0) {
                return default;
            }
            List<Vector2> data = this.ToList();

            if (inset > 0) { 
                Vector2 centre = calculateCentre();
                for (int i = 0; i < data.Count; i++) {
                    data[i] -= Vector2.Normalize(data[i]-centre)*inset;
                }
            }

            data.Add(data[0]);
            return data.ToArray();
        }

        public void GenerateRandom(float radius) {
            Clear();
            System.Random rng = new System.Random();
            int n = rng.Next(8, 20);
            
            List<float> angles = new List<float>();
            for (int i = 0; i < n; i++) {
                angles.Add((float)(rng.NextDouble()*2*PI));
            }
            angles.Sort();

            for (int i = 0; i < n; i++) {
                Add(new Vector2(Sin(angles[i])*radius, Cos(angles[i])*radius));
            }

        }

        public void MergeByDistance(float threshold) {
            List<Vector2> output = this.ToList();
            for (int i = 0; i < Count-2; i++) {
                if ((output[i]-output[(i+1)%output.Count]).LengthSquared() < threshold*threshold) {
                    Vector2 value = (output[i]+output[(i+1)%output.Count])/2;
                    output[i] = value;
                    output[(i+1)%output.Count] = value;
                }
            }

            Clear();
            AddRange(output.Distinct());
        }

        public Region[] Split(float inset = 0) {
            Region[] regions = new Region[2] { new Region(), new Region() };

            System.Random rng = new System.Random();

            float theta = (float)rng.NextDouble()*2*PI;
            Vector2 cutNormal = new Vector2(Cos(theta), Sin(theta));
            Vector2 centre = calculateCentre();
            int prevRegion = (Vector2.Dot(cutNormal, centre-this[Count-1]) > 0)? 0 : 1;
            for (int i = 0; i < Count; i++) {
                int currentRegion;

                float dotProd = Vector2.Dot(cutNormal, centre-this[i]);
                if (dotProd > 0) {
                    currentRegion = 0;
                } else {
                    currentRegion = 1;
                }

                if (prevRegion != currentRegion) {
                    Vector2 cutPoint = LineLineIntersection(this[i], Vector2.Normalize(this[(i-1+Count)%Count]-this[i]), centre, new Vector2(cutNormal.Y, -cutNormal.X));
                    regions[0].Add(cutPoint-cutNormal*inset);
                    regions[1].Add(cutPoint+cutNormal*inset);
                } 
                regions[currentRegion].Add(this[i]);
                

                prevRegion = currentRegion;
            }


            return regions;
        }

        private Vector2 LineLineIntersection(Vector2 l1P, Vector2 l1D, Vector2 l2P, Vector2 l2D) {
            float Cross(Vector2 a, Vector2 b) { 
                return b.X*a.Y - b.Y*a.X;
            }
            float t = Cross(l2D, l1P-l2P)/Cross(l1D, l2D);
            return l1P+t*l1D;
        }
    }
}
