using System;
using System.Diagnostics;
using Raylib_cs;

namespace LookupTables {
    internal class Program {
        
        static void Main(string[] args) {
            LookupTable<float, float> sinLookup = new LookupTable<float, float>( v => MathF.Sin(v) );
            float value = 0;

            Stopwatch sw = new Stopwatch();
            Random random = new Random();

            Queue<float> lookupGraph = new Queue<float>();
            Queue<float> sinGraph = new Queue<float>();

            Raylib.InitWindow(720, 720, "lookups");
            Thread.Sleep(1000);

            while (!Raylib.WindowShouldClose()) {
                sw.Start();
                for (float i = 0; i < 2*MathF.PI; i += 0.01f) { 
                    value = sinLookup[i];
                }
                sw.Stop();
                Console.WriteLine(sw.Elapsed.ToString());
                lookupGraph.Enqueue((float)sw.Elapsed.TotalSeconds);
                sw.Reset();

                sw.Start();
                for (float i = 0; i < 2*MathF.PI; i += 0.01f) { 
                    value = MathF.Sin(i);
                }
                sw.Stop();
                Console.WriteLine(sw.Elapsed.ToString());
                sinGraph.Enqueue((float)sw.Elapsed.TotalSeconds);
                sw.Reset();

                if (lookupGraph.Count > 16) { lookupGraph.Dequeue(); }
                if (sinGraph.Count > 16) { sinGraph.Dequeue(); }

                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.BLACK);

                float last = 0;
                int x = 10;
                foreach (float s in lookupGraph) {
                    if (last == 0) { last = s; continue; }
                    Raylib.DrawLine(x, (int)(last*10000), x+10, (int)(s*10000), Color.GREEN);
                    x += 10;
                }

                last = 0;
                x = 10;
                foreach (float s in sinGraph) { 
                    if (last == 0) { last = s; continue; }
                    Raylib.DrawLine(x, (int)(last*10000), x+10, (int)(s*10000), Color.BLUE);
                    x += 10;
                }

                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();

        }
    }

    class LookupTable<Tk, Tv> where Tk : notnull {
        Func<Tk, Tv> Process;
        Dictionary<Tk, Tv> Dictionary;

        public LookupTable(Func<Tk, Tv> ProcessForNew) { 
            Process = ProcessForNew;
            Dictionary = new Dictionary<Tk, Tv>();
        }

        public Tv this[Tk key] {
            get {
                if (Dictionary.ContainsKey(key)) { 
                    return Dictionary[key];
                } else { 
                    Tv val = Process.Invoke(key);
                    Dictionary.Add(key, val);
                    return val;
                }
            }
        }

    }
}
