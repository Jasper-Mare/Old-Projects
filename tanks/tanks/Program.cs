using System.Collections.Generic;
using System;
using PixelEngine;

namespace tanks
{
    class Program : Game
    {
        //==========Variables==========
        const int mapWidth = 1024;   //must be part of the binary sequence (2^x)
        const int mapHeight = 512;   //must be part of the binary sequence (2^x)

        int[,] map = new int[mapWidth,mapHeight]; //the int refers to the terrain type

        float camX, camY;
        int screenCenterX, screenCenterY;
        Sprite TextureSheet; int texSize = 16, spritesX, spritesY;

        enum tiles { 
            sky = 0,
            grass = 1,
            dirt = 2,
            stone = 3,
        };
        enum teams { 
            red,
            green,
            blue,
            yellow,
        };

        List<tank> Tanks = new List<tank>();
        List<shell> Shells = new List<shell>();
        int focusedTank = 0;

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

            //setup tank
            for (int i = 0; i < 4; i++) {
                float TX = (float)(i+1)/5*mapWidth+Random(-10, 10);
                float TY = 50;
                Tanks.Add(new tank(TX, TY, (teams)i, 100, Random(0.5f,1.5f)*PI));
            }
            MakeTerrain();
        }

        public override void OnUpdate(float delta)
        {
            //camera movement
            if (Shells.Count != 0 && GetMouse(Mouse.Middle).Up) {
                camX = Shells[0].X-ScreenWidth/2;
                camY = Shells[0].Y-ScreenHeight/2;
            } else if (Tanks.Count != 0 && GetKey(Key.Tab).Down) {
                camX = Tanks[focusedTank].X-ScreenWidth/2;
                camY = Tanks[focusedTank].Y-ScreenHeight/2;
            }

            //camera edge collision
            if (camY < -5) { camY = -5; }
            if (camY + ScreenHeight > mapHeight + 5) { camY = mapHeight + 5 - ScreenHeight; }
            if (camX < -5) { camX = -5; }
            if (camX + ScreenWidth > mapWidth + 5) { camX = mapWidth + 5 - ScreenWidth; }

            //if back space pressed make new terrain
            if (GetKey(Key.Back).Pressed) { MakeTerrain(); }
            if (GetKey(Key.Space).Pressed){ Shells.Add(new shell(MouseX+camX, MouseY+camY, 5, -50)); }

            //========Opperate the tanks state machine========

            //do gravity on tanks
            for (int a = 0; a < 10; a++) {
                for (int i = 0; i < Tanks.Count; i++) {
                    tank tmp = Tanks[i];
                    tmp.Y += 9.81f * delta *0.1f;
                    if (map[(int)tmp.X, (int)tmp.Y] != (int)tiles.sky) { tmp.Y -= 9.81f * delta * 0.1f; }
                    Tanks[i] = tmp;
                }
                //do movement on shells
                for (int i = 0; i < Shells.Count; i++) {
                    shell tmp = Shells[i];
                    if (!tmp.explodeing) {
                        tmp.velY += 9.81f * delta * 0.1f;
                        tmp.X += tmp.velX * delta * 0.1f;
                        if (tmp.Y >= 0 && tmp.X >= 0 && tmp.Y < 90 && tmp.X < 160) { if (map[(int)tmp.X, (int)tmp.Y] != (int)tiles.sky) { tmp.velX *= -0.5f; if (Math.Abs(tmp.velX) < 0.3f) { tmp.velX = 0; } } }
                        tmp.Y += tmp.velY * delta * 0.1f;
                        if (tmp.Y >= 0 && tmp.X >= 0 && tmp.Y < 90 && tmp.X < 160) { if (map[(int)tmp.X, (int)tmp.Y] != (int)tiles.sky) { tmp.velY *= -0.5f; if (Math.Abs(tmp.velY) < 0.3f) { tmp.velY = 0; } } }
                        if ((tmp.velX == 0 && tmp.velY == 0) || tmp.Y < 0 || tmp.X < 0 || tmp.X > 160) {//stopped
                            tmp.explodeing = true;
                            MakeCircle((int)tmp.X, (int)tmp.Y, 5);
                        }
                        Shells[i] = tmp;
                    }
                }
            }
            for (int i = 0; i < Shells.Count; i++) {
                shell tmp = Shells[i];
                if (tmp.explodeing) {
                    tmp.explodeingTimer += delta*50;
                    Shells[i] = tmp;
                    if (Shells[i].explodeingTimer > 20) {
                        Shells.RemoveAt(i);
                    }
                }
            }

            //re-draw screen
            DrawScreen();

            AppName = "Camera pos: ("+(int)camX+","+(int)camY+")";
        }

        public override void OnMouseDown(Mouse m) {
            if (m == Mouse.Middle) {
                //move with the mouse dist from center
                camX += Clamp((MouseX-screenCenterX)/100f, -1, 1)*2;
                camY += Clamp((MouseY-screenCenterY)/100f, -1, 1)*2;
            }
        }

        void DrawScreen() {
            for (int y = (int)camY; y < (int)camY+ScreenHeight; y++) {
                Pixel skycol = new Pixel(0,(byte)Lerp(0,255,Clamp(y/(float)(mapHeight*0.5f),0,1)),(byte)Lerp(64,255,Clamp(y/(float)(mapHeight*0.5f),0,1)));
                for (int x = (int)camX; x < (int)camX+ScreenWidth; x++) {
                    int scrX = x - (int)camX, scrY = y - (int)camY; //find where on screen this pixel goes
                    if (x < 0 || x >= mapWidth || y < 0 || y >= mapHeight) { Draw(scrX,scrY, Pixel.Presets.Black); continue; } //if out of bounds, just draw a border

                    //=====Find the pixel value and draw=====
                    Pixel col;
                    if (map[x, y] == 0) { Draw(scrX, scrY, skycol); } //sky
                    else {//ground
                        int u, v;
                        if (map[x,y] >= spritesX*spritesY) { u = 5%spritesX; v = 5/spritesX; } 
                        else { u = map[x,y]%spritesX; v = map[x,y]/spritesX; }
                        col = TextureSheet[u*texSize+(x%texSize),v*texSize+(y%texSize)];
                        Draw(scrX, scrY, col);
                    }
                }
            }
            //draw tanks and shells
            foreach (tank tank in Tanks) {
                Pixel col = Pixel.Presets.Grey;
                switch (tank.Team) {
                    case teams.red:    col = Pixel.Presets.Red;    break;
                    case teams.green:  col = Pixel.Presets.Green;  break;
                    case teams.blue:   col = Pixel.Presets.Blue;   break;
                    case teams.yellow: col = Pixel.Presets.Yellow; break;
                }
                //draw tank
                //FillCircle(new Point((int)tank.X-(int)camX, (int)tank.Y-(int)camY), 1, col);
                DrawLine(new Point((int)tank.X-(int)camX, (int)tank.Y-(int)camY-6), new Point((int)tank.X-(int)camX+(int)(Sin(tank.angle)*8), (int)tank.Y-(int)camY+(int)(Cos(tank.angle)*8)-6), Pixel.Presets.Grey);//turret
                DrawSprite(new Point((int)tank.X-tank.Sprite.Width/2-(int)camX, (int)tank.Y-tank.Sprite.Height-(int)camY+1), tank.Sprite);
                //draw health
                FillRect(new Point((int)tank.X-tank.Sprite.Width/2-(int)camX, (int)tank.Y+1-(int)camY), new Point((int)tank.X-tank.Sprite.Width/2+(int)(tank.Sprite.Width*(tank.health/tank.maxHealth)-(int)camX), (int)tank.Y+4-(int)camY), col);
                FillRect(new Point((int)tank.X-tank.Sprite.Width/2+(int)(tank.Sprite.Width*(tank.health/tank.maxHealth))-1-(int)camX, (int)tank.Y+1-(int)camY), new Point((int)tank.X-tank.Sprite.Width/2+(int)(tank.Sprite.Width*tank.maxHealth/100f)-(int)camX, (int)tank.Y+4-(int)camY), Pixel.Presets.Black);
            }
            foreach (shell shell in Shells) {
                if (!shell.explodeing) {
                    FillCircle(new Point((int)(shell.X-camX), (int)(shell.Y-camY)), 1, Pixel.Presets.Black);
                } else {
                    DrawPartialSprite(new Point((int)(shell.X-camX-shell.Sprite.Width/2f), (int)(shell.Y-camY-shell.Sprite.Height/2f)), shell.explosionAnimation, new Point(16*((int)shell.explodeingTimer%5),16*((int)shell.explodeingTimer/5)), 16,16);
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
                            map[x, mapHeight - y - 1] = 1; //grass
                        } else {//underground
                            if (y > colH - 16) { //is it the dirt layer
                                map[x, mapHeight - y - 1] = 2; //dirt
                            } else {
                                map[x, mapHeight - y - 1] = 3; //stone
                            }
                        }
                    } else {//above the land
                        map[x, mapHeight - y - 1] = 0; //sky
                    }
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

        void MakeCircle(int xCenter, int yCenter, int radius, int fill = (int)tiles.sky) {
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

        struct tank {
            public static Sprite Sprite = Sprite.Load("Tank.png");
            public static float maxHealth = 100f;
            public float X;
            public float Y;
            public teams Team;
            public float health;
            public float angle;

            public tank(float _X, float _Y, teams _team, float _health, float _angle) {
                X = _X; Y = _Y;
                Team = _team;
                health = _health;
                angle = _angle;
            }
        }
        struct shell {
            public static Sprite Sprite = Sprite.Load("Shell.png");
            public static Sprite explosionAnimation = Sprite.Load("explosion.png");

            public float X;
            public float Y;

            public float velX;
            public float velY;

            public bool explodeing;
            public float explodeingTimer;

            public shell(float _X, float _Y, float _velX, float _velY) {
                X = _X; Y = _Y;
                velX = _velX; velY = _velY;
                explodeing = false;
                explodeingTimer = 0;
            }
        }
    }
}
