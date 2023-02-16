using System;
using PixelEngine;

namespace Raycaster4
{
    class Raycaster : Game
    {
        //==============variables==============
        const int screenWidth = 300;
        const int screenHeight = 225;
        const int texWidth = 64;
        const int texHeight = 64;
        const int mapWidth = 24;
        const int mapHeight = 24;

        int[,] worldMap = new int[mapWidth, mapHeight] {
        {8,8,1,2,3,4,5,6,7,8,8,4,4,6,4,4,6,4,6,4,4,4,6,4},
        {8,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4},
        {8,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,6},
        {8,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,6},
        {8,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4},
        {8,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,6},
        {8,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,6},
        {7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,6},
        {7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,6},
        {7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4},
        {7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,6},
        {7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,6},
        {7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,3},
        {2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,3},
        {2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,3},
        {2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,3},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,3},
        {2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,5},
        {2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,5},
        {2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,5},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,5},
        {2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,5},
        {2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,5},
        {2,2,2,2,1,2,2,2,2,2,2,1,2,2,2,5,5,5,5,5,5,5,5,5}};
        int[,] roofMap = new int[mapWidth, mapHeight] {
        {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
        {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
        {1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1},
        {1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1},
        {1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1},
        {1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1},
        {1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1},
        {1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1},
        {1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1},
        {1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1},
        {1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1},
        {1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1},
        {1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1},
        {1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1},
        {1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1},
        {1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1},
        {1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1},
        {1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1},
        {1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1},
        {1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1},
        {1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1},
        {1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1},
        {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
        {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}};
        int[,] floorMap = new int[mapWidth, mapHeight] {
        {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,2,2,2,2,2,2,2},
        {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,2,2,2,2,2,2,2},
        {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,2,2,2,2,2,2,2},
        {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,2,2,2,2,2,2,2},
        {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,2,2,2,2,2,2,2},
        {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,2,2,2,2,2,2,2},
        {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,2,2,2,2,2,2,2},
        {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,2,2,2,2,2,2,2},
        {3,3,3,3,3,3,3,3,4,4,4,4,4,4,4,4,5,5,5,5,5,5,5,5},
        {3,3,3,3,3,3,3,3,4,4,4,4,4,4,4,4,5,5,5,5,5,5,5,5},
        {3,3,3,3,3,3,3,3,4,4,4,4,4,4,4,4,5,5,5,5,5,5,5,5},
        {3,3,3,3,3,3,3,3,4,4,4,4,4,4,4,4,5,5,5,5,5,5,5,5},
        {3,3,3,3,3,3,3,3,4,4,4,4,4,4,4,4,5,5,5,5,5,5,5,5},
        {3,3,3,3,3,3,3,3,4,4,4,4,4,4,4,4,5,5,5,5,5,5,5,5},
        {3,3,3,3,3,3,3,3,4,4,4,4,4,4,4,4,5,5,5,5,5,5,5,5},
        {3,3,3,3,3,3,3,3,4,4,4,4,4,4,4,4,5,5,5,5,5,5,5,5},
        {6,6,6,6,6,6,6,6,7,7,7,7,7,7,7,7,8,8,8,8,8,8,8,8},
        {6,6,6,6,6,6,6,6,7,7,7,7,7,7,7,7,8,8,8,8,8,8,8,8},
        {6,6,6,6,6,6,6,6,7,7,7,7,7,7,7,7,8,8,8,8,8,8,8,8},
        {6,6,6,6,6,6,6,6,7,7,7,7,7,7,7,7,8,8,8,8,8,8,8,8},
        {6,6,6,6,6,6,6,6,7,7,7,7,7,7,7,7,8,8,8,8,8,8,8,8},
        {6,6,6,6,6,6,6,6,7,7,7,7,7,7,7,7,8,8,8,8,8,8,8,8},
        {6,6,6,6,6,6,6,6,7,7,7,7,7,7,7,7,8,8,8,8,8,8,8,8},
        {6,6,6,6,6,6,6,6,7,7,7,7,7,7,7,7,8,8,8,8,8,8,8,8}};

        double posX = 22.0, posY = 11.5;  //x and y start position
        double dirX = -1.0, dirY = 0.0; //initial direction vector
        double planeX = 0.0, planeY = 0.66; //the 2d raycaster version of camera plane

        Sprite[] textures = new Sprite[8];
        //=============subroutines=============
        static void Main(string[] args)
        {
            Raycaster RC = new Raycaster();
            RC.Construct(screenWidth, screenHeight, 4, 4);
            RC.PixelMode = Pixel.Mode.Alpha;
            RC.Start();
        }
        public override void OnCreate()
        {
            //load textures
            textures[0] = Sprite.Load("pics/eagle.png");
            textures[1] = Sprite.Load("pics/redbrick.png");
            textures[2] = Sprite.Load("pics/purplestone.png");
            textures[3] = Sprite.Load("pics/greystone.png");
            textures[4] = Sprite.Load("pics/bluestone.png");
            textures[5] = Sprite.Load("pics/mossy.png");
            textures[6] = Sprite.Load("pics/wood.png");
            textures[7] = Sprite.Load("pics/colorstone.png");
        }
        public override void OnUpdate(float delta)
        {
            AppName = ((int)(1/delta)).ToString();

            //===================FLOOR CASTING========================
            for (int y = 0; y < screenHeight; y++)
            {
                // rayDir for leftmost ray (x = 0) and rightmost ray (x = w)
                float rayDirX0 = (float)(dirX - planeX);
                float rayDirY0 = (float)(dirY - planeY);
                float rayDirX1 = (float)(dirX + planeX);
                float rayDirY1 = (float)(dirY + planeY);

                // Current y position compared to the center of the screen (the horizon)
                int p = y - screenHeight / 2;

                // Vertical position of the camera.
                float posZ = (float)(0.5 * screenHeight);

                // Horizontal distance from the camera to the floor for the current row.
                // 0.5 is the z position exactly in the middle between floor and ceiling.
                float rowDistance = posZ / p;

                // calculate the real world step vector we have to add for each x (parallel to camera plane)
                // adding step by step avoids multiplications with a weight in the inner loop
                float floorStepX = rowDistance * (rayDirX1 - rayDirX0) / screenWidth;
                float floorStepY = rowDistance * (rayDirY1 - rayDirY0) / screenWidth;

                // real world coordinates of the leftmost column. This will be updated as we step to the right.
                float floorX = (float)(posX + rowDistance * rayDirX0);
                float floorY = (float)(posY + rowDistance * rayDirY0);

                for (int x = 0; x < screenWidth; ++x)
                {
                    // the cell coord is simply got from the integer parts of floorX and floorY
                    int cellX = (int)(floorX);
                    int cellY = (int)(floorY);

                    // get the texture coordinate from the fractional part
                    int tx = (int)(texWidth * (floorX - cellX)) & (texWidth - 1);
                    int ty = (int)(texHeight * (floorY - cellY)) & (texHeight - 1);

                    floorX += floorStepX;
                    floorY += floorStepY;

                    // choose texture and draw the pixel
                    int floorTexture   =floorMap[Math.Abs(cellX%mapWidth), Math.Abs(cellY%mapHeight)]-1;
                    int ceilingTexture = roofMap[Math.Abs(cellX%mapWidth), Math.Abs(cellY%mapHeight)]-1;
                    Pixel color;

                    // floor
                    if (floorTexture == -1) { color = Pixel.Presets.Black; }
                    else { color = textures[floorTexture][tx, ty]; }
                    Draw(x, y, color);

                    //ceiling (symmetrical, at screenHeight - y - 1 instead of y)
                    if (ceilingTexture == -1) { color = Pixel.Presets.Cyan; }
                    else { color = textures[ceilingTexture][tx, ty]; }
                    Draw(x, screenHeight - y - 1, color);
                }
            }

            //====================WALL CASTING==========================
            for (int x = 0; x < screenWidth; x++)
            {
                //calculate ray position and direction
                double cameraX = 2 * x / (double)(screenWidth) - 1; //x-coordinate in camera space
                double rayDirX = dirX + planeX * cameraX;
                double rayDirY = dirY + planeY * cameraX;

                //which box of the map we're in
                int mapX = (int)(posX);
                int mapY = (int)(posY);

                //length of ray from current position to next x or y-side
                double sideDistX;
                double sideDistY;

                //length of ray from one x or y-side to next x or y-side
                double deltaDistX = Math.Abs(1 / rayDirX);
                double deltaDistY = Math.Abs(1 / rayDirY);
                double perpWallDist;

                //what direction to step in x or y-direction (either +1 or -1)
                int stepX;
                int stepY;

                bool hit = false; //was there a wall hit?
                int side = -1; //was a NS or a EW wall hit?

                //calculate step and initial sideDist
                if (rayDirX < 0)
                {
                    stepX = -1;
                    sideDistX = (posX - mapX) * deltaDistX;
                }
                else
                {
                    stepX = 1;
                    sideDistX = (mapX + 1.0 - posX) * deltaDistX;
                }
                if (rayDirY < 0)
                {
                    stepY = -1;
                    sideDistY = (posY - mapY) * deltaDistY;
                }
                else
                {
                    stepY = 1;
                    sideDistY = (mapY + 1.0 - posY) * deltaDistY;
                }
                //perform DDA
                while (!hit)
                {
                    //jump to next map square, OR in x-direction, OR in y-direction
                    if (sideDistX < sideDistY)
                    {
                        sideDistX += deltaDistX;
                        mapX += stepX;
                        side = 0;
                    }
                    else
                    {
                        sideDistY += deltaDistY;
                        mapY += stepY;
                        side = 1;
                    }
                    //Check if ray has hit a wall
                    if (worldMap[mapX, mapY] > 0) hit = true;
                }

                //Calculate distance of perpendicular ray (Euclidean distance will give fisheye effect!)
                if (side == 0) perpWallDist = (mapX - posX + (1 - stepX) / 2) / rayDirX;
                else perpWallDist = (mapY - posY + (1 - stepY) / 2) / rayDirY;

                //Calculate height of line to draw on screen
                int lineHeight = (int)(screenHeight / perpWallDist);

                //calculate lowest and highest pixel to fill in current stripe
                int drawStart = -lineHeight / 2 + screenHeight / 2;
                if (drawStart < 0) drawStart = 0;
                int drawEnd = lineHeight / 2 + screenHeight / 2;
                if (drawEnd >= screenHeight) drawEnd = screenHeight - 1;
                //texturing calculations
                int texNum = worldMap[mapX, mapY] - 1; //1 subtracted from it so that texture 0 can be used!
                
                //calculate value of wallX
                double wallX; //where exactly the wall was hit
                if (side == 0) wallX = posY + perpWallDist * rayDirY;
                else wallX = posX + perpWallDist * rayDirX;
                wallX -= Math.Floor(wallX);

                //x coordinate on the texture
                int texX = (int)(wallX * texWidth);
                if (side == 0 && rayDirX > 0) texX = texWidth - texX - 1;
                if (side == 1 && rayDirY < 0) texX = texWidth - texX - 1;

                // How much to increase the texture coordinate per screen pixel
                double step = 1.0 * texHeight / lineHeight;
                // Starting texture coordinate
                double texPos = (drawStart - screenHeight / 2 + lineHeight / 2) * step;
                for (int y = drawStart; y < drawEnd; y++)
                {
                    // Cast the texture coordinate to integer, and mask with (texHeight - 1) in case of overflow
                    int texY = (int)texPos & (texHeight - 1);
                    texPos += step;
                    Pixel color = textures[texNum][texX, texY];
                    Draw(x, y, color);
                }
            }

            //speed modifiers
            double moveSpeed = delta * 3.0; //the constant value is in squares/second
            double rotSpeed = delta * 2.0; //the constant value is in radians/second

            //move forward if no wall in front of you
            if (GetKey(Key.Up).Down)
            {
                if (worldMap[(int)(posX + dirX * moveSpeed),(int)(posY)] == 0) posX += dirX * moveSpeed;
                if (worldMap[(int)(posX),(int)(posY + dirY * moveSpeed)] == 0) posY += dirY * moveSpeed;
            }
            //move backwards if no wall behind you
            if (GetKey(Key.Down).Down)
            {
                if (worldMap[(int)(posX - dirX * moveSpeed),(int)(posY)] == 0) posX -= dirX * moveSpeed;
                if (worldMap[(int)(posX),(int)(posY - dirY * moveSpeed)] == 0) posY -= dirY * moveSpeed;
            }
            //rotate to the right
            if (GetKey(Key.Right).Down)
            {
                //both camera direction and camera plane must be rotated
                double oldDirX = dirX;
                dirX = dirX * Cos((float)-rotSpeed) - dirY * Sin((float)-rotSpeed);
                dirY = oldDirX * Sin((float)-rotSpeed) + dirY * Cos((float)-rotSpeed);
                double oldPlaneX = planeX;
                planeX = planeX * Cos((float)-rotSpeed) - planeY * Sin((float)-rotSpeed);
                planeY = oldPlaneX * Sin((float)-rotSpeed) + planeY * Cos((float)-rotSpeed);
            }
            //rotate to the left
            if (GetKey(Key.Left).Down)
            {
                //both camera direction and camera plane must be rotated
                double oldDirX = dirX;
                dirX = dirX * Cos((float)rotSpeed) - dirY * Sin((float)rotSpeed);
                dirY = oldDirX * Sin((float)rotSpeed) + dirY * Cos((float)rotSpeed);
                double oldPlaneX = planeX;
                planeX = planeX * Cos((float)rotSpeed) - planeY * Sin((float)rotSpeed);
                planeY = oldPlaneX * Sin((float)rotSpeed) + planeY * Cos((float)rotSpeed);
            }
        }
    }
}
