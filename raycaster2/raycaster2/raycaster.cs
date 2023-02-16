using PixelEngine;

namespace raycaster2
{
    class raycaster : Game
    {
        int mapX = 8, mapY = 8, mapS = 64;
        int[] map = { 
        1,1,1,1,1,1,1,1,
        1,0,1,0,0,0,0,1,
        1,0,1,0,0,0,0,1,
        1,0,1,0,0,0,0,1,
        1,0,0,0,0,0,0,1,
        1,0,0,0,0,1,0,1,
        1,0,0,0,0,0,0,1,
        1,1,1,1,1,1,1,1,
        };

        struct PlayerStruct
        {
            public float x;
            public float y;
            public float dx;
            public float dy;
            public float angle;

            public float turnSpeed;
            public float moveSpeed;

            public PlayerStruct(float X, float Y, float Angle, float Movespeed = 1, float Turnspeed = 1) {
                x = X; dx = 0;
                y = Y; dy = 0;
                angle = Angle;
                turnSpeed = Turnspeed;
                moveSpeed = Movespeed;
            }
        }
        PlayerStruct Player;

        struct movementStruct
        {
            public bool forwards;
            public bool backwards;
            public bool clockwise;
            public bool counterclockwise;
            public bool run;
        }
        movementStruct Movement;

        static void Main(string[] args)
        {
            raycaster RC = new raycaster();
            RC.Construct(1024, 512, 1, 1);
            RC.PixelMode = Pixel.Mode.Alpha;
            RC.AppName = "Raycaster";
            RC.Start();
        }

        public override void OnCreate()
        {
            Player.dx = Cos(Player.angle) * 5; Player.dy = Sin(Player.angle) * 5;
            Player = new PlayerStruct(300,300,0, 20);
        }

        public override void OnUpdate(float delta) // ================== main loop ================
        {
            Clear(Pixel.Presets.DarkGrey);

            float fps = 1/delta;

            AppName = "Raycaster | FrameRate: " + fps.ToString().Substring(0,4);
            float FrameRateModifier = 1 / (float)fps;
            float runModifier = Movement.run ? 2 : 1;
            if (Movement.counterclockwise) {
                Player.angle -= Player.turnSpeed * FrameRateModifier * runModifier; if (Player.angle<0) { Player.angle+=2*PI;} 
                Player.dx = Cos(Player.angle)*5; Player.dy = Sin(Player.angle)*5;
            }
            if (Movement.clockwise) {
                Player.angle += Player.turnSpeed * FrameRateModifier * runModifier; if (Player.angle>2*PI) { Player.angle-= 2*PI; }
                Player.dx = Cos(Player.angle)*5; Player.dy = Sin(Player.angle)*5;
            }
            if (Movement.forwards) {
                Player.x += Player.dx * Player.moveSpeed * FrameRateModifier * runModifier; if (map[(((int)Player.y)>>6) * mapX + (((int)Player.x)>>6)] != 0) { Player.x -= Player.dx * Player.moveSpeed * FrameRateModifier * runModifier; }
                Player.y += Player.dy * Player.moveSpeed * FrameRateModifier * runModifier; if (map[(((int)Player.y)>>6) * mapX + (((int)Player.x)>>6)] != 0) { Player.y -= Player.dy * Player.moveSpeed * FrameRateModifier * runModifier; }

            }
            if (Movement.backwards) {
                Player.x -= Player.dx * Player.moveSpeed * FrameRateModifier * runModifier; if (map[(((int)Player.y)>>6) * mapX + (((int)Player.x)>>6)] != 0) { Player.x += Player.dx * Player.moveSpeed * FrameRateModifier * runModifier; }
                Player.y -= Player.dy * Player.moveSpeed * FrameRateModifier * runModifier; if (map[(((int)Player.y)>>6) * mapX + (((int)Player.x)>>6)] != 0) { Player.y += Player.dy * Player.moveSpeed * FrameRateModifier * runModifier; }
            }


            drawMap2d();
            drawRays3d();
            drawPlayer();
        }
        //================================== drawing ===========================
        void drawPlayer()
        {
            FillRect(new Point((int)(Player.x-4), (int)(Player.y-4)), 8, 8, new Pixel(255,255,0));
            DrawLine(new Point((int)Player.x, (int)Player.y), new Point((int)(Player.x+Player.dx*5), (int)(Player.y+Player.dy*5)), Pixel.Presets.Yellow);
        }

        void drawMap2d() {
            int x, y, xoffset, yoffset;
            for (y = 0; y < mapY; y++) {
                for (x = 0; x < mapX; x++) {
                    Pixel pix;
                    if (map[y * mapX + x] >= 1) {pix = new Pixel(255, 255, 255); } 
                    else {pix = new Pixel(0, 0, 0); }
                    xoffset = x * mapS; yoffset = y * mapS;
                    FillRect(new Point(xoffset+1, yoffset+1), mapS-1, mapS-1, pix);
                }
            }
        }

        void drawRays3d() {
            int ray = 0, mx = 0, my = 0, mp = 0, depthOfFeild = 0;

            //-------draw floor and sky -----------
            FillRect(new Point(526, 0), 478, 160, Pixel.Presets.Cyan); //sky
            FillRect(new Point(526,160), 478, 160, Pixel.Presets.DarkMagenta); //floor
            float rayX = 0, rayY = 0, rayAngle = 0, xoffset = 0, yoffset = 0, distT = 0;
            rayAngle = Player.angle-Radians(30); if (rayAngle<0) { rayAngle += 2*PI; } if (rayAngle>2*PI) { rayAngle-=2*PI; }

            for (ray = 0; ray < 60; ray++) {
                //------check horizontal lines-------
                depthOfFeild = 0;
                float distH = 100000000, hx = Player.x, hy = Player.y;
                float aTan = -1 / Tan(rayAngle);
                if (rayAngle > PI) { //looking up
                    rayY = (((int)Player.y >> 6) << 6) - 0.0001f; rayX=(Player.y-rayY)*aTan+Player.x;
                    yoffset = -64; xoffset = -yoffset * aTan;
                }
                if (rayAngle < PI) { //looking down
                    rayY = (((int)Player.y >> 6) << 6) +64; rayX=(Player.y-rayY)*aTan+Player.x;
                    yoffset = 64; xoffset = -yoffset * aTan;
                }
                if(rayAngle==0 || rayAngle==PI) { rayX = Player.x; rayY = Player.y; depthOfFeild = 8; } //looking straight left or right

                while (depthOfFeild < 8) {
                    mx = (int)(rayX) >> 6; my = (int)(rayY) >> 6; mp = my * mapX + mx;
                    if ((mp < mapX * mapY && mp>0) && map[mp] == 1) { hx = rayX; hy = rayY; distH = dist(Player.x, Player.y, hx, hy, rayAngle); depthOfFeild = 8; } //hit wall
                    else { rayX += xoffset; rayY += yoffset; depthOfFeild += 1; } //next line
                }
                //------check vertical lines-------
                depthOfFeild = 0;
                float distV = 100000000, vx = Player.x, vy = Player.y;
                float nTan = -Tan(rayAngle);
                if (rayAngle > PI/2 && rayAngle < (3*PI)/2) { //looking left
                    rayX = (((int)Player.x >> 6) << 6) - 0.0001f; rayY = (Player.x - rayX) * nTan + Player.y;
                    xoffset = -64; yoffset = -xoffset * nTan;
                }
                if (rayAngle < PI/2 || rayAngle > (3*PI)/2) { //looking right
                    rayX = (((int)Player.x >> 6) << 6) + 64; rayY = (Player.x - rayX) * nTan + Player.y;
                    xoffset = 64; yoffset = -xoffset * nTan;
                }
                if (rayAngle == 0 || rayAngle == PI) { rayX = Player.x; rayY = Player.y; depthOfFeild = 8; } //looking straight up or down

                while (depthOfFeild < 8) {
                    mx = (int)(rayX) >> 6; my = (int)(rayY) >> 6; mp = my * mapX + mx;
                    if ((mp < mapX * mapY && mp > 0) && map[mp] == 1) {vx = rayX; vy = rayY; distV = dist(Player.x, Player.y, vx, vy, rayAngle); depthOfFeild = 8; } //hit wall
                    else { rayX += xoffset; rayY += yoffset; depthOfFeild += 1; } //next line
                }

                Pixel pix = new Pixel();
                if (distV < distH) { rayX = vx; rayY = vy; distT = distV; pix = new Pixel(240, 0, 0); } //vertical wall hit
                if (distH < distV) { rayX = hx; rayY = hy; distT = distH; pix = new Pixel(255, 0, 0); } //horizontal wall hit

                DrawLine(new Point((int)Player.x, (int)Player.y), new Point((int)rayX, (int)rayY), pix);
                //-------draw 3d walls----------
                float ca = Player.angle - rayAngle; if (ca<0) { ca += 2*PI; } if (ca>2*PI) { ca-=2*PI; } distT = distT * Cos(ca);
                float lineH = (mapS * 320) / distT; if (lineH > 320) { lineH = 320; } //line height
                float lineO = 160 - lineH / 2;
                FillRect(new Point((int)((ray-0.6f)*8)+530, (int)lineO), new Point((int)((ray+0.6f)*8)+530, (int)(lineH+lineO)), pix);

                rayAngle+=Radians(1); if (rayAngle<0) { rayAngle += 2*PI; } if (rayAngle>2*PI) { rayAngle-=2*PI; }
            }
        }

        //================================== input ===========================
        public override void OnKeyDown(Key k)
        {   //controlls
            //ccw rotation
            if ((k == Key.A || k == Key.Left) && !Movement.counterclockwise) {
                Movement.counterclockwise = true;
            }
            //cw rotation
            if ((k == Key.D || k == Key.Right) && !Movement.clockwise) {
                Movement.clockwise = true;
            }
            //forwards
            if ((k == Key.W || k == Key.Up) && !Movement.forwards) {
                Movement.forwards = true;
            }
            //backwards
            if ((k == Key.S || k == Key.Down) && !Movement.backwards) {
                Movement.backwards = true;
            }
            //run
            if ((k == Key.LShift || k == Key.RShift || k == Key.Shift) && !Movement.run) {
                Movement.run = true;
            }
        }

        public override void OnKeyRelease(Key k)
        {//controlls
            //ccw rotation
            if ((k == Key.A || k == Key.Left) && Movement.counterclockwise) {
                Movement.counterclockwise = false;
            }
            //cw rotation
            if ((k == Key.D || k == Key.Right) && Movement.clockwise) {
                Movement.clockwise = false;
            }
            //forwards
            if ((k == Key.W || k == Key.Up) && Movement.forwards) {
                Movement.forwards = false;
            }
            //backwards
            if ((k == Key.S || k == Key.Down) && Movement.backwards) {
                Movement.backwards = false;
            }
            //run
            if ((k == Key.LShift || k == Key.RShift || k == Key.Shift) && Movement.run) {
                Movement.run = false;
            }
        }

        //===================other========================

        float dist(float ax, float ay, float bx, float by, float ang) {
            return (Sqrt((bx-ax)*(bx-ax)+(by-ay)*(by-ay)) );
        }
    }
}