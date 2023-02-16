using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using PixelEngine;

namespace worldGenTopDown
{
    class Program
    {
        static void Main(string[] args) {
            start:
            Console.WriteLine("1: Perlin noise 1d/2d");
            Console.WriteLine("2: Maze generator");

            switch (Console.ReadLine()) {
                case "1":
                    PerlinGenerator perlinGenerator = new PerlinGenerator();
                    perlinGenerator.Construct(512, 256, 3, 3);
                    perlinGenerator.Start();
                    break;

                case "2":
                    MazeGenerator mazeGenerator = new MazeGenerator();
                    mazeGenerator.Construct(500,500,2,2);
                    mazeGenerator.Start();
                    break;

                default:
                    Console.WriteLine("There is not an option of that number.");
                    goto start;
            }
        }
    }
    class PerlinGenerator : Game
    {   
        int octaveCount = 1;
        int dimensions = 0;
        int seed = 0;
        float scaleBias = 2.0f;
        bool colourBanding = false;
        bool colourise = false;
        bool cycleSeeds = false;
        public override void OnCreate() {
            setupColours();
            AppName = dimensions + "d, " + octaveCount + " octaves, Seed: " + seed + ", Scale bias: " + scaleBias + ", Cycling seeds: " + cycleSeeds;
        }
        public override void OnUpdate(float delta) {
            if (cycleSeeds) {
                seed++;
                AppName = dimensions + "d, " + octaveCount + " octaves, Seed: " + seed + ", Scale bias: " + scaleBias + ", Cycling seeds: " + cycleSeeds;
                if (dimensions == 1) { OneDimension(); }
                if (dimensions == 2) { TwoDimension(); }
                Delay(1);
            }
        }
        public override void OnKeyRelease(Key k) {
            if (k == Key.S) {
                SaveScreen();
            }
            switch (k) {
                case Key.Space:
                    cycleSeeds = !cycleSeeds;
                    break;

                case Key.K0:
                    dimensions = 0;
                    break;
                case Key.K1:
                    OneDimension();
                    dimensions = 1;
                    break;

                case Key.K2:
                    TwoDimension();
                    dimensions = 2;
                    break;

                    case Key.Plus:
                        octaveCount++;
                        if (octaveCount == 9) { octaveCount = 1; }
                        if (dimensions == 1) { OneDimension(); }
                        if (dimensions == 2) { TwoDimension(); }
                        break;

                    case Key.Minus:
                        octaveCount--;
                        if (octaveCount == 0) { octaveCount = 8; }
                        if (dimensions == 1) { OneDimension(); }
                        if (dimensions == 2) { TwoDimension(); }
                        break;

                    case Key.OpenBracket:
                        scaleBias += 0.2f;
                        if (scaleBias == 0) { scaleBias = +0.2f; }
                        if (dimensions == 1) { OneDimension(); }
                        if (dimensions == 2) { TwoDimension(); }
                        break;

                    case Key.CloseBracket:
                        scaleBias -= 0.2f;
                        if (scaleBias == 0) { scaleBias = -0.2f; }
                        if (dimensions == 1) { OneDimension(); }
                        if (dimensions == 2) { TwoDimension(); }
                        break;

                    case Key.Enter:
                        seed = Random(int.MaxValue);
                        if (dimensions == 1) { OneDimension(); }
                        if (dimensions == 2) { TwoDimension(); }
                        break;

                    case Key.Tab:
                        colourBanding = !colourBanding;
                        if (dimensions == 1) { OneDimension(); }
                        if (dimensions == 2) { TwoDimension(); }
                        break;

                    case Key.S:
                        if (dimensions == 1) { OneDimension(); }
                        if (dimensions == 2) { TwoDimension(); }
                        break;

                    case Key.C:
                        colourise = !colourise;
                        if (dimensions == 1) { OneDimension(); }
                        if (dimensions == 2) { TwoDimension(); }
                        break;

                    default:
                        DrawText(new Point(1,1),k.ToString() + " doesn't do anything.", Pixel.Presets.White);
                        break;
                }
            AppName = dimensions + "d, " + octaveCount + " octaves, Seed: " + seed + ", Scale bias: " + scaleBias + ", Cycling seeds: " + cycleSeeds;
        }
        void OneDimension() {
            Random rand = new Random(seed);
            int outputSize = ScreenWidth;
            float[] noiseSeed1D = new float[outputSize];
            float[] perlinNoise1D = new float[outputSize];
            for (int i = 0; i < outputSize; i++) { noiseSeed1D[i] = (float)rand.NextDouble(); }

            MakePerlinNoise1D(outputSize, ref noiseSeed1D, octaveCount, ref perlinNoise1D, scaleBias);

            Clear(Pixel.Presets.Black);
            for (int x = 0; x < outputSize; x++) {
                int y = (int)(-(perlinNoise1D[x]*ScreenHeight/2.0f) + ScreenHeight/2.0f);
                for (int f = y; f < ScreenHeight / 2; f++) {
                    byte val;
                    if (colourise) {
                        if (f < y + 4) {
                            Draw(x, f, getHeightCol(f/(ScreenHeight*0.5f)));
                        } else {
                            val = (byte)(255 - (f / (ScreenHeight * 0.5m) * 255m));
                            Draw(x, f, new Pixel(val, val, val));
                        }
                    } else {
                        if (colourBanding) { val = (byte)(255 - ((int)(f / (ScreenHeight * 0.5m) * 25.5m) * 10)); }
                        else { val = (byte)(255 - (f / (ScreenHeight * 0.5m) * 255m)); }
                        Draw(x, f, new Pixel(val, val, val));
                    }
                }
            }
        }
        void TwoDimension() {
            Random rand = new Random(seed);
            int outputWidth = ScreenWidth;
            int outputHeight = ScreenHeight;
            int longside = Math.Max(outputHeight, outputWidth);
            float[] noiseSeed2D = new float[longside*longside];
            float[] perlinNoise2D = new float[outputWidth * outputHeight];

            for (int i = 0; i < outputWidth * outputHeight; i++) { noiseSeed2D[i] = (float)rand.NextDouble(); }

            MakePerlinNoise2D(outputWidth, outputHeight, ref noiseSeed2D, octaveCount, ref perlinNoise2D, scaleBias);

            for (int x = 0; x < outputWidth; x++) {
                for (int y = 0; y < outputHeight; y++) {
                    if (colourise) {
                        Draw(x, y, getHeightCol(perlinNoise2D[y * outputWidth + x]));
                    } else {
                        byte val;
                        if (colourBanding) { val = (byte)(255 - ((int)(perlinNoise2D[y * outputWidth + x] * 25.5f) * 10)); }
                        else { val = (byte)(255 - (perlinNoise2D[y * outputWidth + x] * 255)); }
                        Draw(x, y, new Pixel(val, val, val));
                    }
                }
            }
        }
        void MakePerlinNoise1D(int Count, ref float[] Seed, int octaves, ref float[] output, float bias = 2.0f) {
            for (int x = 0; x < Count; x++) {
                float Noise = 0.0f;
                float Scale = 1.0f;
                float ScaleAcc = 0.0f;

                for (int o = 0; o < octaves; o++) {
                    int Pitch = Count >> o;
                    int Sample1 = (x / Pitch) * Pitch;
                    int Sample2 = (Sample1 + Pitch) % Count;

                    float Blend = (float)(x - Sample1) / (float)Pitch;
                    float Sample = (1.0f - Blend) * Seed[Sample1] + Blend * Seed[Sample2];

                    Noise += Sample * Scale;
                    ScaleAcc += Scale;
                    Scale /= bias;
                }
                output[x] = Noise / ScaleAcc;
            }
        }
        void MakePerlinNoise2D(int width, int height, ref float[] Seed, int octaves, ref float[] output, float bias = 2.0f) {
            int sidel = Math.Max(width, height);
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    float Noise = 0.0f;
                    float Scale = 1.0f;
                    float ScaleAcc = 0.0f;

                    for (int o = 0; o < octaves; o++) {
                        int Pitch = sidel >> o;
                        int SampleX1 = (x / Pitch) * Pitch;
                        int SampleY1 = (y / Pitch) * Pitch;
                        int SampleX2 = (SampleX1 + Pitch) % sidel;
                        int SampleY2 = (SampleY1 + Pitch) % sidel;

                        float BlendX = (float)(x - SampleX1) / (float)Pitch;
                        float BlendY = (float)(y - SampleY1) / (float)Pitch;

                        float SampleT = (1.0f - BlendX) * Seed[SampleY1*sidel+SampleX1] + BlendX * Seed[SampleY1*sidel+SampleX2];
                        float SampleB = (1.0f - BlendX) * Seed[SampleY2*sidel+SampleX1] + BlendX * Seed[SampleY2*sidel+SampleX2];

                        Noise += (BlendY*(SampleB - SampleT)+SampleT) * Scale;
                        ScaleAcc += Scale;
                        Scale /= bias;
                    }
                    output[y*width+x] = Noise / ScaleAcc;
                }
            }
        }

        Dictionary<string, Pixel> colours = new Dictionary<string, Pixel>() {
            {"snow", new Pixel(245,245,250)}, {"stone", new Pixel(142,148,148)}, {"dirt", new Pixel(110,83,59)}, {"grass", new Pixel(0,154,23)}, {"sand", new Pixel(249,233,142)},
            {"waterS", new Pixel(116,204,244)}, {"waterM", new Pixel(28,163,236)}, {"waterD", new Pixel(15,94,156)}};
        string[] heightColours = new string[256];
        void setupColours() {
            List<string> tmp = new List<string>();
            tmp.AddRange(Enumerable.Repeat("snow",32));//255
            tmp.AddRange(Enumerable.Repeat("stone",64));//223
            tmp.AddRange(Enumerable.Repeat("dirt",32));//159
            tmp.AddRange(Enumerable.Repeat("grass",61));//127
            tmp.AddRange(Enumerable.Repeat("sand",4));//66
            tmp.AddRange(Enumerable.Repeat("waterS",20));//62 - water level
            tmp.AddRange(Enumerable.Repeat("waterM",21));//42
            tmp.AddRange(Enumerable.Repeat("waterD",21));//21
            tmp.Reverse();
            tmp.CopyTo(heightColours);
        }
        Pixel getHeightCol(float h) {
            float height = 254 * (float)(1-h);
            Pixel c1 = colours[heightColours[(int)height]];
            Pixel c2 = colours[heightColours[(int)height+1]];
            if (c1 == c2 || colourBanding) { return c1; }
            Pixel colourReturn = new Pixel((byte)Lerp(c1.R, c2.R, height%1),(byte)Lerp(c1.G, c2.G, height%1),(byte)Lerp(c1.B, c2.B, height%1));
            return colourReturn;
        }
        void SaveScreen() { 
            Pixel[,] screenPixels = GetScreen();
            System.Drawing.Bitmap screen = new System.Drawing.Bitmap(screenPixels.GetLength(1), screenPixels.GetLength(0));
            for (int x = 0; x < screenPixels.GetLength(1); x++) {
                for (int y = 0; y < screenPixels.GetLength(0); y++) {
                    System.Drawing.Color col = System.Drawing.Color.FromArgb(screenPixels[x,y].R,screenPixels[x,y].G, screenPixels[x,y].B);
                    screen.SetPixel(x, y, col);
                }
            }
            int fileNumb = 0;
            while (File.Exists(fileNumb.ToString() + ".PNG")) { fileNumb++; }
            screen.Save(fileNumb.ToString() + ".PNG", System.Drawing.Imaging.ImageFormat.Png);
        }
    }
    class MazeGenerator : Game
    {
        int W=5, H=5, Se=0, R = 0;
        float Sc;

        public override void OnCreate() {
            Sc = Math.Min((float)ScreenWidth/W, (float)ScreenHeight /H);
            AppName = "Size: ("+W+","+H+") Scale: "+Sc+" Holes: ~"+R+" Seed: "+Se;
            generateMaze(W, H, Sc, Se);
        }

        public override void OnKeyRelease(Key k) {
            Clear(Pixel.Presets.Black);
            switch (k) {
                case Key.Up:
                    H++;
                    break;
                case Key.Down:
                    H--;
                    if (H == 1) { H = 2; }
                    break;
                case Key.Right:
                    W++;
                    break;
                case Key.Left:
                    W--;
                    if (W == 1) { W = 2; }
                    break;
                case Key.CloseBracket:
                    R++;
                    break;
                case Key.OpenBracket:
                    R--;
                    if (R == -1) { R = 0; }
                    break;
                case Key.Space:
                    Se = new Random().Next();
                    break;
            }
            Sc = Math.Min((float)ScreenWidth/W, (float)ScreenHeight/H);
            AppName = "Size: ("+W+","+H+") Scale: "+Sc+" Holes: ~"+R+" Seed: "+Se;
            generateMaze(W, H, Sc, Se);
        }

        void generateMaze(int width, int height, float Scale, int seed = -1) {
            Random random = seed == -1 ? new Random() : new Random(seed);
            Dictionary<int,Node> maze = new Dictionary<int,Node>();
            //================create nodes================
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    int id = posToId(x, y, width, height);
                    maze.Add(id, new Node(new Point(x, y), id));
                }
            }
            //================link nodes================
            // === get a node
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    int id = posToId(x, y, width, height);
                    Node tmpLNode = maze[id];
                    // === find it's neighbors and link them (t,l,b,r)
                    if (y > 0)       {int idTop    = posToId(x  ,y-1,width,height);if(maze.ContainsKey(idTop   )){tmpLNode.linkedNodeIds.Add(idTop   );}}
                    if (x > 0)       {int idLeft   = posToId(x-1,y  ,width,height);if(maze.ContainsKey(idLeft  )){tmpLNode.linkedNodeIds.Add(idLeft  );}}
                    if (y < height-1){int idBottom = posToId(x  ,y+1,width,height);if(maze.ContainsKey(idBottom)){tmpLNode.linkedNodeIds.Add(idBottom);}}
                    if (x < width-1) {int idRight  = posToId(x+1,y  ,width,height);if(maze.ContainsKey(idRight )){tmpLNode.linkedNodeIds.Add(idRight );}}
                }
            }
            //================proccess the nodes================
            for (int i = 0; i < R; i++) {
                int index = random.Next(maze.Count);
                Node tmpPNode = maze[index];
                if (tmpPNode.linkedNodeIds.Count == 0) {continue; }
                tmpPNode.unlink(tmpPNode.linkedNodeIds[random.Next(tmpPNode.linkedNodeIds.Count)], ref maze);
                maze[index] = tmpPNode;
            }
            Console.WriteLine();

            //================draw nodes================
            int offX = (int)((ScreenWidth * 0.5)  - ((width -1) * Scale * 0.5f));
            int offY = (int)((ScreenHeight * 0.5) - ((height-1) * Scale * 0.5f));
            foreach (KeyValuePair<int, Node> entry in maze) {
                Point p = entry.Value.pos;
                FillCircle(new Point((int)(p.X*Scale)+offX, (int)(p.Y*Scale)+offY), (int)(Scale/8), Pixel.Presets.Yellow);
                foreach (int n in entry.Value.linkedNodeIds) {
                    Point lp = maze[n].pos;
                    DrawLine(new Point((int)(p.X*Scale)+offX, (int)(p.Y*Scale)+offY), new Point((int)(lp.X*Scale)+offX, (int)(lp.Y*Scale)+offY), Pixel.Presets.Green);
                }
            }
        }

        struct Node {
            public List<int> linkedNodeIds;
            public Point pos;
            public int Id;
            public Node(Point _pos, int _id) {
                pos = _pos;
                Id = _id;
                linkedNodeIds = new List<int>();
            }

            public void unlink(int _id, ref Dictionary<int, Node> _maze) {
                //remove this node from the other one, then remove that node from this one
                Node thatnode = _maze[_id];
                thatnode.linkedNodeIds.Remove(Id); //this node's id
                _maze[_id] = thatnode;
                linkedNodeIds.Remove(_id); //that node's id
            }
        }
        int posToId(Point nPos, int mWidth, int mHeight) {
            if (nPos.X < 0 || nPos.X >= mWidth) { throw new ArgumentOutOfRangeException("nPos.X"); }
            if (nPos.Y < 0 || nPos.Y >= mHeight) { throw new ArgumentOutOfRangeException("nPos.Y"); }
            return nPos.X + nPos.Y * mWidth;
        }
        int posToId(int X, int Y, int mWidth, int mHeight) {
            if (X < 0 || X >= mWidth) { throw new ArgumentOutOfRangeException("X"); }
            if (Y < 0 || Y >= mHeight) { throw new ArgumentOutOfRangeException("Y"); }
            return X + Y * mWidth;
        }
        Point IdToPos(int nId, int mWidth) {
            return new Point(nId/mWidth, nId%mWidth);
        }
    }
}