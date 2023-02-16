using PixelEngine;

namespace destroyable_terrain
{
    class Program : Game
    {
        //==========Variables==========
        const int mapWidth = 1024;   //must be part of the binary sequence (2^x)
        const int mapHeight = 512;   //must be part of the binary sequence (2^x)
        const float seaLevel = 0.45f; //sea level between 0f and 1f

        int[,] map = new int[mapWidth,mapHeight]; //the int refers to the terrain type

        float camX, camY, camSpeed = 60;
        int screenCenterX, screenCenterY;
        Sprite TextureSheet; int texSize = 16, spritesX, spritesY;

        enum tiles { 
            sky = -1,
            sand = 0,
            grass = 1,
            dirt = 2,
            stone = 3,
            water = 4,
            unknown = 5
        };

        //==========Functions==========
        static void Main(string[] args)
        {
            Program prog = new Program();
            prog.Construct(400,300,4,4);
            prog.PixelMode = Pixel.Mode.Alpha;
            prog.Start();
        }

        public override void OnCreate() {
            camX = mapWidth/2 - ScreenWidth/2;
            camY = mapHeight/2 - ScreenHeight/2;

            screenCenterX = ScreenWidth / 2;
            screenCenterY = ScreenHeight / 2;

            TextureSheet = Sprite.Load("TextureMap.png");
            spritesX = TextureSheet.Width/texSize;
            spritesY = TextureSheet.Height/texSize;

            MakeTerrain();
        }

        public override void OnUpdate(float delta)
        {
            //movement
            if (GetKey(Key.W).Down || GetKey(Key.Up   ).Down) { camY -= delta*camSpeed; }
            if (GetKey(Key.S).Down || GetKey(Key.Down ).Down) { camY += delta*camSpeed; }
            if (GetKey(Key.A).Down || GetKey(Key.Left ).Down) { camX -= delta*camSpeed; }
            if (GetKey(Key.D).Down || GetKey(Key.Right).Down) { camX += delta*camSpeed; }
            //edge collision
            if (camY < -5) { camY = -5; }
            if (camY + ScreenHeight > mapHeight + 5) { camY = mapHeight + 5 - ScreenHeight; }
            if (camX < -5) { camX = -5; }
            if (camX + ScreenWidth > mapWidth + 5) { camX = mapWidth + 5 - ScreenWidth; }

            //if space pressed make new terrain
            if (GetKey(Key.Space).Pressed) { MakeTerrain(); }

            //re-draw screen
            DrawScreen();

            AppName = "Camera pos: ("+(int)camX+","+(int)camY+")";
        }

        public override void OnMouseDown(Mouse m) {
            if (m == Mouse.Left) {
                MakeCircle((int)camX + MouseX, (int)camY + MouseY, 5);
            } else if (m == Mouse.Right) {
                MakeCircle((int)camX + MouseX, (int)camY + MouseY, 5, (int)tiles.unknown);
            }
            if (m == Mouse.Middle) {
                //move with the mouse dist from center
                camX += Clamp((MouseX-screenCenterX)/100f, -1, 1)*2;
                camY += Clamp((MouseY-screenCenterY)/100f, -1, 1)*2;
            }
        }

        void DrawScreen() {
            for (int y = (int)camY; y < (int)camY+ScreenHeight; y++) {
                Pixel skycol; if (y < mapHeight-(mapHeight*seaLevel)) { 
                    skycol = new Pixel(0,(byte)Lerp(0,255,Clamp(y/(mapHeight*seaLevel),0,1)),(byte)Lerp(128,255,Clamp(y/(mapHeight*seaLevel),0,1)));
                } else { skycol = new Pixel(0,(byte)Lerp(0,255,Clamp((1-((float)y/mapHeight))/seaLevel,0,1)),(byte)Lerp(0,256,Clamp((1-((float)y/mapHeight))/seaLevel,0,1))); }
                for (int x = (int)camX; x < (int)camX+ScreenWidth; x++) {
                    int scrX = x - (int)camX, scrY = y - (int)camY; //find where on screen this pixel goes
                    if (x < 0 || x >= mapWidth || y < 0 || y >= mapHeight) { Draw(scrX,scrY, Pixel.Presets.Black); continue; } //if out of bounds, just draw a border

                    //=====Find the pixel value and draw=====
                    Pixel col;
                    if (map[x, y] == -1) { Draw(scrX, scrY, skycol); } //sky
                    else if (map[x,y] == 4) { //water
                        if (y >= seaLevel*mapHeight-1 && y < seaLevel*mapHeight) {//sea edge
                            Draw(scrX, scrY, Pixel.Presets.Blue);
                        } else {
                            Draw(scrX, scrY, skycol);
                            int u = map[x,y]%spritesX, v = map[x,y]/spritesX;
                            col = TextureSheet[u*texSize+(x%texSize),v*texSize+(y%texSize)];
                            Draw(scrX, scrY, col);
                        }
                    }
                    else {//ground
                        int u, v;
                        if (map[x,y] >= spritesX*spritesY) { u = 5%spritesX; v = 5/spritesX; } 
                        else { u = map[x,y]%spritesX; v = map[x,y]/spritesX; }
                        col = TextureSheet[u*texSize+(x%texSize),v*texSize+(y%texSize)];
                        Draw(scrX, scrY, col);
                    }
                }
            }
        }

        void MakeTerrain() {
            float[] noiseSeed1D = new float[mapWidth];
            float[] perlin1D = new float[mapWidth];
            float[] bias = new float[mapWidth];
            int bX = 0, bOldX;
            while (bX < mapWidth) {
                int w = (int)Random(0, mapWidth / 10f);
                float v = Random(1.5f, 2.5f);
                bOldX = bX;
                for (; bX < w+bOldX && bX < mapWidth; bX++) {
                    bias[bX] = v;
                }
            }
            noiseSeed1D[0] = 0.5f; //make the land start in the middle
            for (int i = 1; i < mapWidth; i++) { noiseSeed1D[i] = Random(0.0f, 1.0f); }

            MakePerlinNoise1D(mapWidth, ref noiseSeed1D, 10, ref perlin1D, ref bias);

            //assemble map
            for (int x = 0; x < mapWidth; x++) {
                int colH = (int)(perlin1D[x]*mapHeight);
                for (int y = 0; y < mapHeight; y++) {
                    if (y < colH) {//in the land
                        if (y > colH - 4) {//surface
                            if (colH <= seaLevel*mapHeight+4) {//is it below a bit above sea level
                                map[x, mapHeight - y - 1] = 0; //sand
                            } else if (y <= seaLevel * mapHeight + 4) {//transition
                                map[x, mapHeight - y - 1] = Random(0, 2);
                            } else {
                                map[x, mapHeight - y - 1] = 1; //grass
                            }
                        } else {//underground
                            if (y > colH - 16) { //is it the dirt layer
                                map[x, mapHeight - y - 1] = 2; //dirt
                            } else {
                                map[x, mapHeight - y - 1] = 3; //stone
                            }
                        }
                    } else {//above the land
                        if (y > seaLevel * mapHeight) {
                            map[x, mapHeight - y - 1] = -1; //sky
                        } else {
                            map[x, mapHeight - y - 1] = 4; //water
                        }
                    }
                }
            }
            //add caves
            //sub surface caves
            int numbCaves = Random(10, 26);
            for (int caveN = 0; caveN < numbCaves; caveN++) {
                //pick a random point to be the cave start
                float CX = Random(10,mapWidth-9);
                float CY = Random(mapHeight-20, mapHeight-(perlin1D[(int)CX]*mapHeight - 30));
                int distance = Random(50, 500); //dig for a random distance
                float CW = 0.5f;
                float angle = Random(0,2) == 1 ? Random(0.25f*PI, 0.75f*PI) : Random(1.25f*PI, 1.75f*PI);
                while (CW < 10f) {
                    MakeCircle((int)CX, (int)CY, (int)(CW / 2f));
                    angle += Random(-0.75f, 0.75f);
                    CX += Sin(angle);
                    CY += Cos(angle);
                    CW += Random(0.1f, 0.7f);
                }
                for (int i = 0; i < distance; i++) {
                    MakeCircle((int)CX, (int)CY, (int)(CW/2f));
                    angle += Random(-0.15f, 0.15f);
                    CX += Sin(angle);
                    CY += Cos(angle);
                    if (CX < CW || CX > mapWidth-CW) { break; } if (CY < CW || CY > mapHeight-CW) { break; }
                    if (map[(int)(CX+(Sin(angle)*CW)), (int)(CY+(Cos(angle)*CW))] == (int)tiles.dirt) { break; }
                    CW += Random(-0.5f, 0.5f);
                }
                while (CW > 0.5f) {
                    MakeCircle((int)CX, (int)CY, (int)(CW / 2f));
                    angle += Random(-0.75f, 0.75f);
                    CX += Sin(angle);
                    CY += Cos(angle);
                    CW -= Random(0.1f, 0.7f);
                }
            }
            //caverns
            numbCaves = Random(5, 15);
            for (int caveN = 0; caveN < numbCaves; caveN++) {
                //pick a random point to be the cave start
                float CX = Random(10,mapWidth-9);
                float CY = Random(mapHeight-20, mapHeight-(perlin1D[(int)CX]*mapHeight - 120));
                int distance = Random(50, 100); //dig for a random distance
                float CW = 0.5f;
                float angle = Random(0,2) == 1 ? Random(0.25f*PI, 0.75f*PI) : Random(1.25f*PI, 1.75f*PI);
                while (CW < 30f) {
                    MakeCircle((int)CX, (int)CY, (int)(CW / 2f));
                    angle += Random(-0.75f, 0.75f);
                    CX += Sin(angle);
                    CY += Cos(angle);
                    CW += Random(0.1f, 0.7f);
                }
                for (int i = 0; i < distance; i++) {
                    MakeCircle((int)CX, (int)CY, (int)(CW/2f));
                    angle += Random(-0.15f, 0.15f);
                    CX += Sin(angle);
                    CY += Cos(angle);
                    if (CX < CW || CX > mapWidth-CW) { break; } if (CY < CW || CY > mapHeight-CW) { break; }
                    if (map[(int)(CX+(Sin(angle)*CW)), (int)(CY+(Cos(angle)*CW))] == (int)tiles.dirt) { break; }
                    CW += Random(-0.5f, 0.5f);
                }
                while (CW > 0.5f) {
                    MakeCircle((int)CX, (int)CY, (int)(CW / 2f));
                    angle += Random(-0.75f, 0.75f);
                    CX += Sin(angle);
                    CY += Cos(angle);
                    CW -= Random(0.1f, 0.7f);
                }
            }
        }

        void MakePerlinNoise1D(int Count, ref float[] Seed, int octaves, ref float[] output, ref float[] biasArray) {
            for (int x = 0; x < Count; x++) {
                float Noise = 0.0f;
                float Scale = 1.0f;
                float ScaleAcc = 0.0f;

                for (int o = 0; o < octaves; o++) {
                    int Pitch = Count >> o;
                    int Sample1 = (x / Pitch) * Pitch;
                    int Sample2 = (Sample1 + Pitch) % Count;

                    float Blend = (x - Sample1) / (float)Pitch;
                    float Sample = (1.0f - Blend) * Seed[Sample1] + Blend * Seed[Sample2];
                    float bias = (1.0f - Blend) * biasArray[Sample1] + Blend * biasArray[Sample2];

                    Noise += Sample * Scale;
                    ScaleAcc += Scale;
                    Scale /= bias;
                }
                output[x] = Noise / ScaleAcc;
            }
        }

        void MakeCircle(int xCenter, int yCenter, int radius, int fill = -1) {
            for (int x = xCenter - radius; x <= xCenter; x++) {
                for (int y = yCenter - radius; y <= yCenter; y++) {
                    // we don't have to take the square root, it's slow
                    if ((x - xCenter) * (x - xCenter) + (y - yCenter) * (y - yCenter) <= radius * radius) {
                        int xSym = xCenter - (x - xCenter);
                        int ySym = yCenter - (y - yCenter);
                        // (x, y), (x, ySym), (xSym , y), (xSym, ySym) are in the circle
                        if (x >= 0 && x < mapWidth) {
                            if (y >= 0 && y < mapHeight) { map[x, y] = fill; }
                            if (ySym >= 0 && ySym < mapHeight) { map[x, ySym] = fill; }
                        }
                        if (xSym >= 0 && xSym < mapWidth) { 
                            if (y >= 0 && y < mapHeight) { map[xSym, y] = fill; }
                            if (ySym >= 0 && ySym < mapHeight) { map[xSym, ySym] = fill; }
                        }
                    }
                }
            }
        }
    }
}