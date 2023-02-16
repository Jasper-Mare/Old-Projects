using System;
using System.Collections.Generic;
using PixelEngine;

namespace raycaster {
    class Raycaster : Game {
        string map;
        int mapWidth = 33;
        int mapHeight = 33;
        int miniMapScale = 3;
        float distScale = 3f;

        Sprite emptytexture;
        Sprite walltexture;
        Sprite jarTexture;
        Sprite lampTexture;
        Sprite projectileTexture;

        List<group> groups;
        cell[,] cells;

        struct ObjectStruct {
            public float x;
            public float y;
            public float vx;
            public float vy;
            public bool remove;
            public Sprite sprite;
            public ObjectStruct(float _x, float _y, Sprite _sprite, float _vx = 0f, float _vy = 0f, bool _remove = false) {
                x = _x;
                y = _y;
                vx = _vx;
                vy = _vy;
                remove = _remove;
                sprite = _sprite;
            }
        }
        List<ObjectStruct> listObjects;

        struct PlayerStruct {
            public float x;
            public float y;
            public float angle;

            public float turnSpeed;
            public float moveSpeed;
        }
        PlayerStruct Player;

        float fov = PI / 4;
        float depth = 32.0f;
        float step = 0.01f;
        float[,] depthBuffer;

        struct movementStruct {
            public bool forwards;
            public bool backwards;
            public bool left;
            public bool right;
            public bool clockwise;
            public bool counterclockwise;
            public bool run;
        }
        movementStruct Movement;

        Dictionary<char, Sprite> solidObjects = new Dictionary<char, Sprite>();
        bool ShowDepth = false;

        //================ subroutines ================
        static void Main(string[] args) {
            Raycaster RC = new Raycaster();
            RC.Construct(300, 300, 4, 4);
            RC.PixelMode = Pixel.Mode.Alpha;
            RC.Start();
        }

        public override void OnCreate() {
            char[,] mazeTmp = generateMaze();
            for (int y = 0; y < mapHeight; y++) { for (int x = 0; x < mapWidth; x++) { map += mazeTmp[x, y]; } }

            //walls
            emptytexture = Sprite.Load("empty.png");
            walltexture = Sprite.Load("wall.png");
            //objects
            jarTexture = Sprite.Load("jar.png");
            lampTexture = Sprite.Load("lamp.png");
            projectileTexture = Sprite.Load("projectile.png");

            solidObjects.Add('#', walltexture);

            listObjects = new List<ObjectStruct>() {
                //new ObjectStruct(8.5f , 8.5f, jarTexture),
                //new ObjectStruct(7.5f , 7.5f, jarTexture),
                //new ObjectStruct(10.5f, 3.5f, jarTexture),
                //new ObjectStruct(5.5f, 1.5f, lampTexture)
            };

            Player.x = 2f; Player.y = 2f;
            Player.turnSpeed = 1; Player.moveSpeed = 3;

            depthBuffer = new float[ScreenWidth, ScreenHeight];
        }

        public override void OnUpdate(float elapsed) {
            if (GetKey(Key.Tab).Pressed) { ShowDepth = !ShowDepth; }
            List<(float, float)> rayends = new List<(float, float)>();
            for (int x = 0; x < ScreenWidth; x++) { //itterate accross the screen
                float rayAngle = (Player.angle - fov / 2) + ((float)x / (float)ScreenWidth) * fov; //get starting angle

                float distanceToWall = 0;
                bool hitWall = false;

                float eyeX = Sin(rayAngle); //unit vector for ray in player space
                float eyeY = Cos(rayAngle);

                float sampleX = 0.0f;

                int testX = 0, testY = 0;
                while (!hitWall && distanceToWall < depth) {
                    distanceToWall += step;

                    testX = (int)(Player.x + eyeX * distanceToWall);
                    testY = (int)(Player.y + eyeY * distanceToWall);

                    //test if ray is out of bounds
                    if (testX < 0 || testX >= mapWidth || testY < 0 || testY >= mapHeight) {
                        hitWall = true;
                        distanceToWall = depth;
                    } else {
                        //ray is in bounds
                        if (solidObjects.ContainsKey(map[testY * mapWidth + testX])) {
                            hitWall = true;
                            //find where the ray hit the wall
                            float BlockMidX = (float)testX + 0.5f;
                            float BlockMidY = (float)testY + 0.5f;

                            float TestPointX = Player.x + eyeX * distanceToWall;
                            float TestPointY = Player.y + eyeY * distanceToWall;

                            float testAngle = (float)Math.Atan2((TestPointY - BlockMidY), (TestPointX - BlockMidX));

                            if (testAngle >= -PI * 0.25f && testAngle < PI * 0.25f) {
                                sampleX = TestPointY - (float)testY;
                            }
                            if (testAngle >= PI * 0.25f && testAngle < PI * 0.75f) {
                                sampleX = TestPointX - (float)testX;
                            }
                            if (testAngle < -PI * 0.25 && testAngle >= -PI * 0.75f) {
                                sampleX = TestPointX - (float)testX;
                            }
                            if (testAngle >= PI * 0.75f || testAngle < -PI * 0.75f) {
                                sampleX = TestPointY - (float)testY;
                            }
                        }
                    }
                }
                float rayEndX, rayEndY;
                rayEndX = Sin(rayAngle) * distanceToWall + Player.x;
                rayEndY = Cos(rayAngle) * distanceToWall + Player.y;
                rayends.Add((rayEndX, rayEndY));

                distanceToWall *= distScale;
                distanceToWall = distanceToWall * (float)Math.Cos(Player.angle - rayAngle) - 0.1f;

                //calculate distance to ceiling and floor
                int ceilling = (int)((float)(ScreenHeight / 2.0) - ScreenHeight / ((float)distanceToWall));
                int floor = ScreenHeight - ceilling;

                for (int y = 0; y < ScreenHeight; y++) {
                    if (y > ceilling && y <= floor) { depthBuffer[x, y] = distanceToWall; } else { depthBuffer[x, y] = 2 * depth; }
                }
                //====================drawing=========================
                if (!ShowDepth) {
                    for (int y = 0; y < ScreenHeight; y++) {
                        if (y <= ceilling) { //current cell is part of the ceiling
                            Draw(x, y, Pixel.Presets.Cyan); //draw ceiling
                            //shade ceiling based on distance
                            float b = (((float)y - ScreenHeight / 2f) / ((float)ScreenHeight / 2f));
                            //Draw(x, y, new Pixel(0, 0, 0, (byte)(b * 255)));
                        }
                        else if (y > ceilling && y <= floor) { //current cell is part of the wall
                            //put texture logic in here
                            float sampleY = ((float)y - (float)ceilling) / ((float)floor - (float)ceilling);
                            Sprite texture;
                            char wallName = map[testY * mapWidth + testX];
                            if (solidObjects.ContainsKey(wallName)) {
                                texture = solidObjects[wallName];
                            } else {
                                texture = emptytexture;
                            }

                            Draw(x, y, sampleTexture(texture, sampleX, sampleY));

                            //Draw(x, y, new Pixel(0, 0, 0, (byte)Math.Min((distanceToWall / depth) * 255, 255)));
                        }
                        else { //current cell is part of the floor
                            Draw(x, y, Pixel.Presets.DarkGreen); //draw floor
                            //shade floor based on distance
                            float b = 1.0f - (((float)y - ScreenHeight / 2f) / ((float)ScreenHeight / 2f));
                            //Draw(x, y, new Pixel(0, 0, 0, (byte)(b * 255)));
                        }
                    }
                }
            }
            //draw objects
            for (int i = 0; i < listObjects.Count; i++) {
                ObjectStruct obj = listObjects[i];
                //update object physics
                obj.x += obj.vx * elapsed;
                obj.y += obj.vy * elapsed;

                //check if object is inside a wall
                if (solidObjects.ContainsKey(map[(int)obj.y * mapWidth + (int)obj.x])) {
                    obj.remove = true;
                }
                listObjects[i] = obj;

                //can the object be seen?
                float vecX = obj.x - Player.x;
                float vecY = obj.y - Player.y;
                float distanceFromPlayer = Sqrt(vecX * vecX + vecY * vecY);

                //calculate angle between player and object to find if it is in the feild of view
                float eyeX = Sin(Player.angle);
                float eyeY = Cos(Player.angle);
                float objectAngle = (float)Math.Atan2(eyeY, eyeX) - (float)Math.Atan2(vecY, vecX);
                if (objectAngle < -PI) { objectAngle += 2 * PI; }
                if (objectAngle > PI) { objectAngle -= 2 * PI; }

                bool inPlayerFOV = (Math.Abs(objectAngle) <= fov / 2.0f);

                if (inPlayerFOV && distanceFromPlayer >= 0.5f && distanceFromPlayer < depth) {
                    float objectCeiling = (float)(ScreenHeight / 2.0f) - ScreenHeight / ((float)distanceFromPlayer);
                    float objectFloor = ScreenHeight - objectCeiling;
                    float objectHeight = objectFloor - objectCeiling;
                    float objectAspectRatio = (float)obj.sprite.Height / (float)obj.sprite.Width;
                    float objectWidth = objectHeight / objectAspectRatio;

                    float middleOfObject = (0.5f * (objectAngle / (fov / 2.0f)) + 0.5f) * (float)ScreenWidth;

                    for (float lx = 0; lx < objectWidth; lx++) {
                        for (float ly = 0; ly < objectHeight; ly++) {
                            float sampleX = lx / objectWidth;
                            float sampleY = ly / objectHeight;

                            Pixel c = sampleTexture(obj.sprite, sampleX, sampleY);
                            int objectColumn = (int)(middleOfObject + lx - (objectWidth / 2.0f));
                            if (objectColumn >= 0 && objectColumn < ScreenWidth && (int)(objectCeiling + ly) >= 0 && (int)(objectCeiling + ly) < ScreenHeight) {
                                if (depthBuffer[objectColumn, (int)(objectCeiling + ly)] >= distanceFromPlayer && c.A > 254) {
                                    if (!ShowDepth) {
                                        Draw(objectColumn, (int)(objectCeiling + ly), c);
                                        Draw(objectColumn, (int)(objectCeiling + ly), new Pixel(0, 0, 0, (byte)((distanceFromPlayer / depth) * 255))); //distance darkness
                                    }
                                    depthBuffer[objectColumn, (int)(objectCeiling + ly)] = distanceFromPlayer;
                                }
                            }
                        }
                    }
                }
            }
            listObjects.RemoveAll(item => item.remove);

            if (ShowDepth) {
                for (int x = 0; x < ScreenWidth; x++) {
                    for (int y = 0; y < ScreenHeight; y++) {
                        byte c = (byte)(-255 * Math.Min(Math.Max(depthBuffer[x, y] / depth, 0),1));
                        Draw(x, y, new Pixel(c, c, c));
                    }
                }
            }
            //=========== Draw mini map ===================
            for (int Mx = 0; Mx < mapWidth; Mx++) {
                for (int My = 0; My < mapHeight; My++) {
                    char tileValue = map[My * mapWidth + Mx];
                    if (solidObjects.ContainsKey(tileValue)) {
                        FillRect(new Point(miniMapScale * Mx, miniMapScale * My), miniMapScale, miniMapScale, new Pixel(255, 255, 255, 175));
                    } else {
                        FillRect(new Point(miniMapScale * Mx, miniMapScale * My), miniMapScale, miniMapScale, new Pixel(0, 0, 0, 175));
                    }
                }
            }

            foreach ((float, float) p in rayends) {//draw rays
                DrawLine(new Point((int)(Player.x * miniMapScale), (int)(Player.y * miniMapScale)), new Point((int)(p.Item1 * miniMapScale), (int)(p.Item2 * miniMapScale)), Pixel.Presets.Green);
            }
            foreach (ObjectStruct obj in listObjects) { //draw objects
                FillCircle(new Point((int)(obj.x * miniMapScale), (int)(obj.y * miniMapScale)), miniMapScale / 4, Pixel.Presets.Orange);
            }
            //draw player on it
            FillCircle(new Point((int)(Player.x * miniMapScale), (int)(Player.y * miniMapScale)), miniMapScale / 2, Pixel.Presets.Yellow);
            
            //================= move =======================
            float runModifier = Movement.run ? 2 : 1;
            if (Movement.counterclockwise) {//ccw rotation
                Player.angle -= Player.turnSpeed * elapsed * runModifier;
                Player.angle = ReturnWrapped(Player.angle, 0, 2 * PI);
            }
            if (Movement.clockwise) {//cw rotation
                Player.angle += Player.turnSpeed * elapsed * runModifier;
                Player.angle = ReturnWrapped(Player.angle, 0, (2 * PI));
            }
            if (Movement.forwards) {//forwards
                Player.x += Sin(Player.angle) * Player.moveSpeed * elapsed * runModifier;
                if (solidObjects.ContainsKey(map[(int)Player.y * mapWidth + (int)Player.x])) { Player.x -= Sin(Player.angle) * Player.moveSpeed * elapsed * runModifier; }
                Player.y += Cos(Player.angle) * Player.moveSpeed * elapsed * runModifier;
                if (solidObjects.ContainsKey(map[(int)Player.y * mapWidth + (int)Player.x])) { Player.y -= Cos(Player.angle) * Player.moveSpeed * elapsed * runModifier; }
            }
            if (Movement.backwards) {//backwards
                Player.x -= Sin(Player.angle) * Player.moveSpeed * elapsed * runModifier;
                if (solidObjects.ContainsKey(map[(int)Player.y * mapWidth + (int)Player.x])) { Player.x += Sin(Player.angle) * Player.moveSpeed * elapsed * runModifier; }
                Player.y -= Cos(Player.angle) * Player.moveSpeed * elapsed * runModifier;
                if (solidObjects.ContainsKey(map[(int)Player.y * mapWidth + (int)Player.x])) { Player.y += Cos(Player.angle) * Player.moveSpeed * elapsed * runModifier; }
            }
            if (Movement.left) {//left
                Player.x -= Cos(Player.angle) * Player.moveSpeed * elapsed * runModifier;
                if (solidObjects.ContainsKey(map[(int)Player.y * mapWidth + (int)Player.x])) { Player.x += Cos(Player.angle) * Player.moveSpeed * elapsed * runModifier; }
                Player.y += Sin(Player.angle) * Player.moveSpeed * elapsed * runModifier;
                if (solidObjects.ContainsKey(map[(int)Player.y * mapWidth + (int)Player.x])) { Player.y -= Sin(Player.angle) * Player.moveSpeed * elapsed * runModifier; }
            }
            if (Movement.right) {//right
                Player.x += Cos(Player.angle) * Player.moveSpeed * elapsed * runModifier;
                if (solidObjects.ContainsKey(map[(int)Player.y * mapWidth + (int)Player.x])) { Player.x -= Cos(Player.angle) * Player.moveSpeed * elapsed * runModifier; }
                Player.y -= Sin(Player.angle) * Player.moveSpeed * elapsed * runModifier;
                if (solidObjects.ContainsKey(map[(int)Player.y * mapWidth + (int)Player.x])) { Player.y += Sin(Player.angle) * Player.moveSpeed * elapsed * runModifier; }
            }

            AppName = "FPS: " + Math.Truncate(1 / elapsed);
        }

        public override void OnKeyDown(Key k) { //controlls
            //ccw rotation
            if ((k == Key.A || k == Key.Num4) && !Movement.counterclockwise) {
                Movement.counterclockwise = true;
            }
            //cw rotation
            if ((k == Key.D || k == Key.Num6) && !Movement.clockwise) {
                Movement.clockwise = true;
            }
            //left
            if ((k == Key.Q || k == Key.Num7) && !Movement.left) {
                Movement.left = true;
            }
            //right
            if ((k == Key.E || k == Key.Num9) && !Movement.right) {
                Movement.right = true;
            }
            //forwards
            if ((k == Key.W || k == Key.Num8) && !Movement.forwards) {
                Movement.forwards = true;
            }
            //backwards
            if ((k == Key.S || k == Key.Num5) && !Movement.backwards) {
                Movement.backwards = true;
            }
            //run
            if ((k == Key.LShift || k == Key.RShift || k == Key.Shift || k == Key.Separator) && !Movement.run) {
                Movement.run = true;
            }
        }
        public override void OnKeyRelease(Key k) { //controlls
            //ccw rotation
            if ((k == Key.A || k == Key.Num4) && Movement.counterclockwise) {
                Movement.counterclockwise = false;
            }
            //cw rotation
            if ((k == Key.D || k == Key.Num6) && Movement.clockwise) {
                Movement.clockwise = false;
            }
            //left
            if ((k == Key.Q || k == Key.Num7) && Movement.left) {
                Movement.left = false;
            }
            //right
            if ((k == Key.E || k == Key.Num9) && Movement.right) {
                Movement.right = false;
            }
            //forwards
            if ((k == Key.W || k == Key.Num8) && Movement.forwards) {
                Movement.forwards = false;
            }
            //backwards
            if ((k == Key.S || k == Key.Num5) && Movement.backwards) {
                Movement.backwards = false;
            }
            //run
            if ((k == Key.LShift || k == Key.RShift || k == Key.Shift || k == Key.Separator) && Movement.run) {
                Movement.run = false;
            }

            //fire projectile
            if (k == Key.Space || k == Key.Num0) {
                ObjectStruct o;
                o.x = Player.x;
                o.y = Player.y;

                float noise = Random(-0.1f, 0.1f);
                float speed = Random(7.8f, 8.2f);
                o.vx = Sin(Player.angle + noise) * speed;
                o.vy = Cos(Player.angle + noise) * speed;

                o.sprite = projectileTexture;
                o.remove = false;
                listObjects.Add(o);
            }
        }

        //========================== helpers ========================
        public float ReturnWrapped(float val, float min, float max) {
            if (val > max) { return val - max + min; }
            if (val < min) { return val - min + max; }

            return val;
        }

        public Pixel sampleTexture(Sprite sprite, float sx, float sy) {
            return sprite[(int)((sx % 1) * sprite.Width), (int)((sy % 1) * sprite.Height)];
        }

        public char[,] generateMaze() {
            char[,] maze = new char[mapWidth, mapHeight];
            int cellsX = (mapWidth-1)/2, cellsY = (mapHeight-1)/2;

            groups = new List<group>();
            cells = new cell[cellsX, cellsY];
            for (int x = 0; x < cellsX; x++) {
                for (int y = 0; y < cellsY; y++) {
                    cells[x, y] = new cell(x,y,(x+cellsX*y));
                    group newGroup = new group((x+cellsX*y));
                    newGroup.Add(cells[x,y]);
                    groups.Add(newGroup);
                }
            }

            while (groups.Count > 1) { merge(); }

            for (int x = 0; x < mapWidth; x++) { for (int y = 0; y < mapHeight; y++) { maze[x, y] = '#'; } }

            for (int x = 0; x < cellsX; x++) { 
                for (int y = 0; y < cellsY; y++) {
                    maze[x*2+1, y*2+1] = ' ';
                    cell c = cells[x,y];
                    foreach (cell con in c.connections) {
                        int oX = (c.position.X - con.position.X), oY = (c.position.Y - con.position.Y);
                        maze[x*2+1-oX, y*2+1-oY] = ' ';
                    }
                }
            }

            for (int y = 0; y < mapHeight; y++) { for (int x = 0; x < mapWidth; x++) { Console.Write(maze[x, y]); } Console.WriteLine(); }
            return maze;
        }
        private void merge() {
            Random rand = new Random();
            int cellsX = (mapWidth-1)/2, cellsY = (mapHeight-1)/2;
            cell c1 = null, c2 = null;
            if (groups.Count <= 1) { return; }
            while (c1 == null || c2 == null || c1.groupNumb == c2.groupNumb) {
                //pick a cell
                c1 = cells[rand.Next(cellsX), rand.Next(cellsY)];
                //pick a surrounding cell
                c2 = null;
                int a = rand.Next(0, 4), c2x = -1, c2y = -1;
                while (!isInBounds(c2x, c2y, cellsX, cellsY)) {
                    if (a == 0) { c2x = c1.position.X - 1; c2y = c1.position.Y; }
                    else if (a == 1) { c2x = c1.position.X; c2y = c1.position.Y - 1; }
                    else if (a == 2) { c2x = c1.position.X + 1; c2y = c1.position.Y; }
                    else if (a == 3) { c2x = c1.position.X; c2y = c1.position.Y + 1; }
                    a = (a + 1) % 4;
                }
                c2 = cells[c2x, c2y];
            }
            //if they are not the in same group, merge their groups
            int g1 = -1, g2 = -1, i = 0;
            foreach (group g in groups) {
                if (g.Number == c1.groupNumb) { g1 = i; }
                if (g.Number == c2.groupNumb) { g2 = i; }
                if (g1 != -1 && g2 != -1) { break; }
                i++;
            }
            if (g1 == -1 || g2 == -1) { return; }
            groups[g1].AddRange(groups[g2]);
            for (int c = 0; c < groups[g1].Count; c++) { groups[g1][c].groupNumb = groups[g1].Number; }
            groups.RemoveAt(g2);
            //add connections between them
            c1.connections.Add(c2);
            c2.connections.Add(c1);
        }
        bool isInBounds(int x, int y, int width, int height) {
            return (x >= 0 && x < width && y >= 0 && y < height);
        }
    }

    class group : List<cell> {
        public int Number;
        public group(int numb) : base() {
            Number = numb;
        }
    }
    class cell {
        public List<cell> connections = new List<cell>();
        public Point position;
        public int groupNumb;

        public cell(int x, int y, int n) { position = new Point(x, y); groupNumb = n; }
    }
}