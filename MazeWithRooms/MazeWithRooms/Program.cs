using System;
using Raylib_cs;
using System.Linq;
using System.Numerics;
using static System.MathF;
using static Raylib_cs.Raylib;
using static Raylib_cs.Raymath;
using System.Collections.Generic;

namespace MazeWithRooms {
    class Program {
        const int texWidth = 32, texHeight = 32;
        static Image[] textures;

        static void Main() {
            const int winW = 1080, winH = 720;
            InitWindow(winW, winH, "Dungeon");
            Rectangle drawSpace = new Rectangle(winW/4,winH/8, (winW/4)*2,(winH/8)*6);
            Console.WriteLine(drawSpace.ToString());

            textures = new Image[] { 
                LoadImage(@"wall.png"), //wall
                LoadImage(@"error.png"), //space
                LoadImage(@"door.png") //door
            };

            int selectedCorner = -1;
            bool exit = false, debugInfo = false, paused = false, viewmode = false;
            float speed = 2.5f;
            
            Vector2 playerPos = new Vector2();
            Vector2 playerDir = new Vector2(-1, 0);
            Vector2 camPlane = new Vector2(0, 0.66f);

            float[] ZBuffer = new float[winW];
            Dungeon dungeon;
            setup(0);

            void setup(int seed) { 
                dungeon = new Dungeon(new intVector(100, 100));
                dungeon.Generate(1000, 0.25f, 0, 0.25f, seed);

                for (int i = 0; i < dungeon.Cells.Length; i++) {
                    int x = i%dungeon.Size.x, y = i/dungeon.Size.x;
                    if (dungeon.Cells[x,y] == gridTile.space) {
                        playerPos = new Vector2(x+0.5f,y+0.5f);
                        break;
                    }
                }
            }

            SetExitKey(KeyboardKey.KEY_NULL);
            while (!exit) {
                exit = WindowShouldClose();
                float deltaT;
                if (paused) {
                    deltaT = 0;
                } else { 
                    deltaT = GetFrameTime();
                }

                //resize the view port 
                {
                    if (IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT)) {
                        if (Near(GetMouseX()-(int)GetMouseDelta().X,(int)drawSpace.x, 10) && Near(GetMouseY()-(int)GetMouseDelta().Y,(int)drawSpace.y, 10)) {
                            selectedCorner = 0;
                        } else if (Near(GetMouseX()-(int)GetMouseDelta().X,(int)(drawSpace.x+drawSpace.width), 10) && Near(GetMouseY()-(int)GetMouseDelta().Y, (int)(drawSpace.y+drawSpace.height), 10)) {
                            selectedCorner = 1;
                        }
                    }
                    if (IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT)) {
                        selectedCorner = -1;
                    }

                    int mx = (int)(Clamp(GetMouseX(), 20, winW-20)/20f)*20, my = (int)(Clamp(GetMouseY(), 20, winH-20)/20f)*20;
                    if (selectedCorner == 0) {
                        int x2 = (int)Max(drawSpace.x+drawSpace.width , drawSpace.x+20), 
                            y2 = (int)Max(drawSpace.y+drawSpace.height, drawSpace.y+20);
                        drawSpace.x = Min(mx, winW-40); 
                        drawSpace.y = Min(my, winH-40);
                        drawSpace.width  = Clamp(x2-drawSpace.x,20,winW-drawSpace.x-20); 
                        drawSpace.height = Clamp(y2-drawSpace.y,20,winH-drawSpace.y-20);
                        if (drawSpace.width  <= 20) { drawSpace.x = x2-20; } 
                        if (drawSpace.height <= 20) { drawSpace.y = y2-20; }

                    } else if (selectedCorner == 1) { 
                        drawSpace.width  = Clamp(mx-drawSpace.x,20,winW-drawSpace.x-20); 
                        drawSpace.height = Clamp(my-drawSpace.y,20,winH-drawSpace.y-20);
                    }
                }

                //keypresses
                { 
                    if (IsKeyDown(KeyboardKey.KEY_W)) {
                        playerPos.X += playerDir.X*deltaT*speed;
                        if (dungeon.Cells[(int)playerPos.X, (int)playerPos.Y] == gridTile.wall) {
                            playerPos.X -= playerDir.X*deltaT*speed;
                        }
                        playerPos.Y += playerDir.Y*deltaT*speed;
                        if (dungeon.Cells[(int)playerPos.X, (int)playerPos.Y] == gridTile.wall) {
                            playerPos.Y -= playerDir.Y*deltaT*speed;
                        }
                    }
                    if (IsKeyDown(KeyboardKey.KEY_S)) {
                        playerPos.X -= playerDir.X*deltaT*speed;
                        if (dungeon.Cells[(int)playerPos.X, (int)playerPos.Y] == gridTile.wall) {
                            playerPos.X += playerDir.X*deltaT*speed;
                        }
                        playerPos.Y -= playerDir.Y*deltaT*speed;
                        if (dungeon.Cells[(int)playerPos.X, (int)playerPos.Y] == gridTile.wall) {
                            playerPos.Y += playerDir.Y*deltaT*speed;
                        }
                    }
                    if (IsKeyDown(KeyboardKey.KEY_A)) {
                        playerDir = Vector2Rotate(playerDir, deltaT);
                        camPlane = Vector2Rotate(camPlane,   deltaT);
                    }
                    if (IsKeyDown(KeyboardKey.KEY_D)) {
                        playerDir = Vector2Rotate(playerDir,-deltaT);
                        camPlane = Vector2Rotate(camPlane,  -deltaT);
                    }
                    
                    if (IsKeyPressed(KeyboardKey.KEY_F3)) {
                        debugInfo = !debugInfo;
                    }
                    if (IsKeyPressed(KeyboardKey.KEY_ESCAPE)) {
                        paused = !paused;
                        if (viewmode & !paused) { DisableCursor(); } 
                        else { EnableCursor(); }
                    }
                    if (IsKeyPressed(KeyboardKey.KEY_ENTER)) {
                        setup(GetFPS());
                    }
                    if (IsKeyPressed(KeyboardKey.KEY_TAB)) {
                        viewmode = !viewmode;
                        if (viewmode) { DisableCursor(); } 
                        else { EnableCursor(); }
                    }

                    if (viewmode & !paused) { 
                        float Mdx = -GetMouseDelta().X/winW * 2f;
                        playerDir = Vector2Rotate(playerDir, Mdx);
                        camPlane  = Vector2Rotate(camPlane,  Mdx);
                        SetMousePosition(winW/2, winH/2);
                    }

                }

                BeginDrawing();

                ClearBackground(Color.BLACK);
                if (!paused) {
                    //raycasting
                    {
                        //wallcasting
                        for (int x = 0; x < winW; x++) {
                            //calculate ray pos and dir
                            float cameraX = 2 * x / (float)winW - 1; //x-coord in cam space
                            Vector2 raydir = playerDir+camPlane*cameraX;

                            //which map box we're in
                            int mapX = (int)playerPos.X;
                            int mapY = (int)playerPos.Y;

                            //length of ray from current pos to next x or y side
                            float sideDistX, sideDistY;

                            //length of ray from one x or y-side to next x or y-side
                            float deltaDistX = (raydir.X == 0) ? 1e30f : Abs(1/raydir.X);
                            float deltaDistY = (raydir.Y == 0) ? 1e30f : Abs(1/raydir.Y);
                            float perpWallDist;
                            //what direction to step in x or y-direction (either +1 or -1)
                            int stepX, stepY;

                            bool hit = false; //was a wall hit
                            int side = 0; //was a NS or EW wall hit

                            //calculate step and initial sideDist
                            if (raydir.X < 0) {
                                stepX = -1;
                                sideDistX = (playerPos.X-mapX)*deltaDistX;
                            } else {
                                stepX = 1;
                                sideDistX = (mapX+1-playerPos.X)*deltaDistX;
                            }
                            if (raydir.Y < 0) {
                                stepY = -1;
                                sideDistY = (playerPos.Y-mapY)*deltaDistY;
                            } else {
                                stepY = 1;
                                sideDistY = (mapY+1-playerPos.Y)*deltaDistY;
                            }

                            //perform DDA
                            while (!hit) {
                                //jump to next map square, either in x-direction, or in y-direction
                                if (sideDistX < sideDistY) {
                                    sideDistX += deltaDistX;
                                    mapX += stepX;
                                    side = 0;
                                } else {
                                    sideDistY += deltaDistY;
                                    mapY += stepY;
                                    side = 1;
                                }
                                //Check if ray has hit a wall
                                hit = dungeon.Cells[mapX,mapY] != gridTile.space;
                            }

                            //Calculate distance of perpendicular ray
                            if (side == 0) perpWallDist = (sideDistX - deltaDistX);
                            else perpWallDist = (sideDistY - deltaDistY);

                            dungeon.visibility[mapX, mapY] = (byte)Min(dungeon.visibility[mapX, mapY], perpWallDist*32);

                            //Calculate height of line to draw on screen
                            int lineHeight = (int)(winH/perpWallDist);
                            
                            //calculate lowest and highest pixel to fill in current stripe
                            int drawStart = -lineHeight/2 + winH/2;
                            if (drawStart < 0) drawStart = 0;
                            int drawEnd = lineHeight/2 + winH/2;
                            if (drawEnd >= winH) drawEnd = winH-1;

                            //calculate value of wallX
                            float wallX; //where exactly the wall was hit
                            if (side == 0) wallX = playerPos.Y + perpWallDist * raydir.Y;
                            else wallX = playerPos.X + perpWallDist * raydir.X;
                            wallX -= Floor(wallX);

                            //x coordinate on the texture
                            int texX = (int)(wallX*texWidth);
                            if (side == 0 && raydir.X > 0) texX = texWidth - texX - 1;
                            if (side == 1 && raydir.Y < 0) texX = texWidth - texX - 1;

                            if (viewmode) { 
                                // How much to increase the texture coordinate per screen pixel
                                float step = 1.0f*texHeight/lineHeight;
                                // Starting texture coordinate
                                float texPos = (drawStart-winH/2+lineHeight/2)*step;
                                for (int y = drawStart; y < drawEnd; y++) {
                                    // Cast the texture coordinate to integer, and mask with (texHeight - 1) in case of overflow
                                    int texY = (int)texPos & (texHeight - 1);
                                    texPos += step;
                                    Color color = GetImageColor(textures[(int)dungeon.Cells[mapX,mapY]], texX, texY);

                                    //make color darker for y-sides: R, G and B byte each divided through two with a "shift" and an "and"
                                    //apply lighting
                                    if (side == 1) {
                                        color.r = (byte)(color.r*1.1f);
                                        color.g = (byte)(color.g*1.1f);
                                        color.b = (byte)(color.b*1.1f);
                                    }

                                    DrawPixel(x,y, color);
                                }
                            }

                            //SET THE ZBUFFER FOR THE SPRITE CASTING
                            ZBuffer[x] = perpWallDist; //perpendicular distance is used
                        }
                    }
                    if (!viewmode) { 
                        //draw map
                        DrawRectangleRec(drawSpace, Color.DARKGRAY);
                        dungeon.Draw(drawSpace, playerPos, playerDir);

                        DrawRectangle((int)drawSpace.x-1, (int)drawSpace.y-1, 3,3, Color.ORANGE);
                        DrawRectangle((int)(drawSpace.x+drawSpace.width)-1, (int)(drawSpace.y+drawSpace.height)-1, 3,3, Color.ORANGE);
                        if (selectedCorner == 0) {
                            DrawRectangle((int)drawSpace.x-10, (int)drawSpace.y-10, 20,20, Color.ORANGE);
                        } else if (selectedCorner == 1) { 
                            DrawRectangle((int)(drawSpace.x+drawSpace.width)-10, (int)(drawSpace.y+drawSpace.height)-10, 20,20, Color.ORANGE);
                        }
                    }
                } else {
                    DrawText("Paused", 5, 25, 20, Color.LIME);
                }

                if (debugInfo) { DrawFPS(5,5); }

                EndDrawing();
            }

        }

        static bool Near(int a, int b,int dist) {
            return (b-dist < a && a < b+dist);
        }

    }

    enum gridTile : byte { 
        wall = 0, space = 1, door = 2
    }

    struct intVector {
        public int x, y;
        public intVector(int X, int Y) {
            x = X; y = Y;
        }
    }

    class Dungeon {
        public intVector Size;
        public gridTile[,] Cells;
        public byte[,] visibility;
        public Dungeon() {
            Cells = new gridTile[Size.x, Size.y];
        }
        public Dungeon(intVector mazeSize) {
            mazeSize.x = (int)Floor(mazeSize.x/2f)*2+1;
            mazeSize.y = (int)Floor(mazeSize.y/2f)*2+1;
            Size = mazeSize;
            Cells = new gridTile[Size.x, Size.y];
            visibility = new byte[Size.x, Size.y];
        }

        private struct room {
            public intVector Pos, Size;
            public room(intVector position, intVector roomSize) {
                Pos = position;
                Size = roomSize;
            }
        }

        public void Generate(int numbRoomTries, float extraConnectorChance, int roomExtraSize, float windingChance, int seed) {
            Random rng = new Random(seed);
            List<room> roomList = new List<room>();

            for (int x = 0; x < Size.x; x++) { 
                for (int y = 0; y < Size.y; y++) {
                    visibility[x, y] = 255;
                }
            }
            
            //Add rooms
            for (int z = 0; z < numbRoomTries; z++) {
                int tmpRoomSize = rng.Next(3,3+roomExtraSize)*2;
                int rectangularity = rng.Next(0, 1+tmpRoomSize/2)*2;

                intVector roomSize = new intVector(tmpRoomSize, tmpRoomSize);
                if (rng.Next(0, 2) == 1) {
                    roomSize.x += rectangularity;
                } else {
                    roomSize.y += rectangularity;
                }

                intVector newRoomPos = new intVector((int)Floor(rng.Next(2, Size.x-roomSize.x-2)/2)*2+1, (int)Floor(rng.Next(2, Size.y-roomSize.y-2)/2)*2+1);

                //Check if it overlaps an existing room
                if (roomList.Any(r => (newRoomPos.x <= r.Pos.x + r.Size.x &&
                                       newRoomPos.x + roomSize.x >= r.Pos.x &&
                                       newRoomPos.y <= r.Pos.y + r.Size.y &&
                                       newRoomPos.y + roomSize.y >= r.Pos.y))) {
                    continue;
                }

                room newRoom = new room(newRoomPos, roomSize);
                roomList.Add(newRoom);

                //carving
                for (int x = 0; x < newRoom.Size.x - 1; x++) {
                    for (int y = 0; y < newRoom.Size.y - 1; y++) {
                        intVector pos = new intVector(newRoomPos.x + x, newRoomPos.y + y);
                        SetTile(pos, gridTile.space);
                    }
                }
            }

            //maze generation
            for (int y = 1; y < Size.y; y += 2) {
                for (int x = 1; x < Size.x; x += 2) {
                    intVector pos = new intVector(x, y);
                    if (Cells[x, y] != gridTile.wall) { continue; }

                    //grow maze
                    List<intVector> cells = new List<intVector>();
                    int lastDir = -1;

                    //carve
                    SetTile(pos);

                    cells.Add(pos);

                    while (cells.Count > 0) {
                        intVector cell = cells.Last();

                        int[] unmadeCells = Enumerable.Range(0, 4).Where(dir => { 
                            intVector newPos = AddDir(cell,dir,2); 
                            return (InMaze(newPos) && Cells[newPos.x, newPos.y] == gridTile.wall); 
                        }).ToArray();

                        if (unmadeCells.Length != 0) {
                            //applying windiness
                            int dir;
                            if (unmadeCells.Contains(lastDir) && rng.NextDouble() > windingChance) {
                                dir = lastDir;
                            } else {
                                dir = unmadeCells[rng.Next(0, unmadeCells.Length)];
                            }

                            //carving
                            intVector cell1 = AddDir(cell, dir);
                            intVector cell2 = AddDir(cell, dir, 2);
                            SetTile(cell1);
                            SetTile(cell2);
                            cells.Add(cell2);
                            lastDir = dir;
                        } else {
                            cells.RemoveAt(cells.Count-1);
                            lastDir = -1;
                        }
                    }
                }
            }

            //connect rooms
            for (int i = roomList.Count-1; i >= 0; i--) {
                room r = roomList[i];

                List<int> walls = Enumerable.Range(0,4).ToList();

                int addNewDoor() { return ((rng.NextDouble() <= extraConnectorChance)?1:0); }

                for (int numbDoors = 1 + addNewDoor()+addNewDoor()+addNewDoor(); numbDoors > 0; numbDoors--) {
                    int wall = walls[rng.Next(walls.Count-1)];
                    walls.Remove(wall);

                    if (wall == 0) { //North wall 
                        int a = (int)Floor(rng.Next(2, r.Size.x-2)/2f)*2+r.Pos.x;
                        SetTile(new intVector(a, r.Pos.y-1), gridTile.door);
                    } else if (wall == 1) { //East wall
                        int a = (int)Floor(rng.Next(2, r.Size.y-2)/2f)*2+r.Pos.y;
                        SetTile(new intVector(r.Pos.x-1, a), gridTile.door);
                    } else if (wall == 2) { //South wall
                        int a = (int)Floor(rng.Next(2, r.Size.x-2)/2f)*2+r.Pos.x;
                        SetTile(new intVector(a, r.Pos.y+r.Size.y-1), gridTile.door);
                    } else if (wall == 3) { //West wall
                        int a = (int)Floor(rng.Next(2, r.Size.y-2)/2f)*2+r.Pos.y;
                        SetTile(new intVector(r.Pos.x+r.Size.x-1, a), gridTile.door);
                    }
                }

                roomList.RemoveAt(roomList.Count-1);
            }

            //remove dead ends
            Queue<intVector> deadEnds = new Queue<intVector>();
            //Stack<vec> deadEnds = new Stack<vec>();
            for (int y = 1; y < Size.y; y += 2) {
                for (int x = 1; x < Size.x; x += 2) {
                    if (IsDeadEnd(new intVector(x,y))) {
                        deadEnds.Enqueue(new intVector(x,y));
                        //deadEnds.Push(new vec(x, y));
                    }
                }
            }
            while (deadEnds.Count > 0) {
                intVector cell = deadEnds.Dequeue();
                //vec cell = deadEnds.Pop();
                Cells[cell.x, cell.y] = gridTile.wall;
                SetTile(cell, gridTile.wall);

                for (int i = 0; i < 4; i++) { 
                    intVector neighbour = AddDir(cell, i);
                    if (InMaze(neighbour) && Cells[neighbour.x, neighbour.y] != gridTile.wall && IsDeadEnd(neighbour)) {
                        deadEnds.Enqueue(neighbour);
                        //deadEnds.Push(neighbour);
                    }
                }

                //Draw();
            }

        }
        private bool IsDeadEnd(intVector pos) {
            int neighbours = 0;
            
            if (InMaze(new intVector(pos.x+0,pos.y+1)) && Cells[pos.x+0,pos.y+1] != gridTile.wall) { neighbours++; }
            if (InMaze(new intVector(pos.x+1,pos.y+0)) && Cells[pos.x+1,pos.y+0] != gridTile.wall) { neighbours++; }
            if (InMaze(new intVector(pos.x+0,pos.y-1)) && Cells[pos.x+0,pos.y-1] != gridTile.wall) { neighbours++; }
            if (InMaze(new intVector(pos.x-1,pos.y+0)) && Cells[pos.x-1,pos.y+0] != gridTile.wall) { neighbours++; }

            return (neighbours <= 1);
        }
        private void SetTile(intVector pos, gridTile tile = gridTile.space) {
            if (InMaze(pos)) {
                Cells[pos.x, pos.y] = tile;
            }
        }
        public bool InMaze(intVector pos) {
            return (-1 < pos.x && pos.x < Size.x && -1 < pos.y && pos.y < Size.y);
        }
        private intVector AddDir(intVector pos, int dir, int amount = 1) {
            switch (dir) {
                case 0:
                    return new intVector(pos.x, pos.y-amount);
                case 1:
                    return new intVector(pos.x+amount, pos.y);
                case 2:
                    return new intVector(pos.x, pos.y+amount);
                case 3:
                    return new intVector(pos.x-amount, pos.y);
                default:
                    return pos;
            }
        }

        public void Draw(Rectangle drawSpace, Vector2 playerPos, Vector2 playerdir) {
            int scale = (int)Min(Floor(drawSpace.width/(Size.x+2)), Floor(drawSpace.height/(Size.y+2)));
            int xoff = (int)((drawSpace.width-(scale*Size.x))/2+drawSpace.x), yoff = (int)((drawSpace.height-(scale*Size.y))/2+drawSpace.y);

            for (int y = 0; y < Size.y; y++) {
                for (int x = 0; x < Size.x; x++) {
                    int i = (x*scale)+xoff, j =(y*scale)+yoff;
                    switch (Cells[x,y]) {
                        case gridTile.wall:
                            DrawRectangle(i,j, scale,scale, Color.GRAY);
                            break;
                        case gridTile.space:
                            DrawRectangle(i,j, scale,scale, Color.BLACK);
                            break;
                        case gridTile.door:
                            DrawRectangle(i,j, scale,scale, Color.BROWN);
                            break;
                    }
                    DrawRectangle(i,j, scale,scale, new Color((byte)0,(byte)0,(byte)0, visibility[x,y]));
                }
            }

            DrawCircle((int)(playerPos.X*scale+xoff), (int)(playerPos.Y*scale+yoff), Sqrt(scale), Color.BLUE);
            DrawLine((int)(playerPos.X*scale+xoff), (int)(playerPos.Y*scale+yoff), (int)(playerPos.X*scale+playerdir.X*scale+xoff), (int)(playerPos.Y*scale+playerdir.Y*scale+yoff), Color.YELLOW);

        }



    }
}