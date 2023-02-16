using System;
using System.Collections;
using System.Collections.Generic;
using PixelEngine;
namespace raycaster3
{
    class raycaster : Game
    {
        //=============structs================
        struct Smovement {
            public bool forwards;
            public bool backwards;
            public bool left;
            public bool right;
            public bool clockwise;
            public bool counterclockwise;
            public bool run;

            public float turnSpeed;
            public float moveSpeed;
        }
        //==============vars==================
        float mapCanvasWidth; float mapCanvasHeight;
        float mapHeight = 30f; float mapWidth = 30f;
        float mapscale;
        Smovement movement;
        Pixel backGroundCol = new Pixel(171, 194, 203);
        Pixel gridLineCol   = new Pixel(207, 206, 198);
        Pixel boundriesCol  = new Pixel(214, 118, 103);

        float depthOfField;
        Sprite TextureBrick;
        Sprite TextureConcrete;
        List<Boundry> walls;
        Player Player;

        float playerHeight = 0.5f;
        float screenDistance = 0.3f;
        //============functions===============
        static void Main(string[] args)
        {
            raycaster RC = new raycaster();
            RC.Construct(600, 301, 4, 4);
            RC.PixelMode = Pixel.Mode.Alpha;
            RC.Start();
        }
        public override void OnCreate() 
        {
            TextureBrick = Sprite.Load("brick.png");
            TextureConcrete = Sprite.Load("concrete.png");
            walls = new List<Boundry>() {
                //edge walls
                new Boundry(00,00,00,30, TextureConcrete),
                new Boundry(00,00,30,00, TextureConcrete),
                new Boundry(00,30,30,30, TextureConcrete),
                new Boundry(30,00,30,30, TextureConcrete),
                //building1
                new Boundry(01,01,01,07, TextureBrick),
                new Boundry(01,01,03,02, TextureBrick),
                new Boundry(03,02,08,02, TextureBrick),
                new Boundry(08,02,10,01, TextureBrick),
                new Boundry(10,01,10,07, TextureBrick),
                new Boundry(08,07,10,07, TextureBrick),
                new Boundry(08,07,08,5.5f, TextureBrick),
                new Boundry(08,06,06,06, TextureBrick),
                new Boundry(03,06,05,06, TextureBrick),
                new Boundry(03,07,03,5.5f, TextureBrick),
                new Boundry(03,07,01,07, TextureBrick),
                new Boundry(03,04,03,05, TextureBrick),
                new Boundry(03,4.5f,08,4.5f, TextureBrick),
                new Boundry(08,04,08,05, TextureBrick),
                new Boundry(03,02,03,3.5f, TextureBrick),
                new Boundry(08,02,08,3.5f, TextureBrick),
            };

            mapCanvasWidth = ScreenWidth / 2;
            mapCanvasHeight = ScreenHeight;
            float w = mapCanvasWidth  / mapWidth;
            float h = mapCanvasHeight / mapHeight;
            mapscale = Math.Min(w, h);
            depthOfField = 30;

            movement.turnSpeed = 1;
            movement.moveSpeed = 1;
            
            Player = new Player(new System.Windows.Point(15, 15), 1);
            Player.pointAt(0);
        }
        void drawBoundries(Boundry[] boundries) {
            foreach (Boundry bound in boundries) { drawBoundry(bound); }
        }
        void drawBoundry(Boundry bo) {
            DrawLine(new Point((int)(bo.a.X*mapscale), (int)(bo.a.Y*mapscale)), new Point((int)(bo.b.X*mapscale), (int)(bo.b.Y*mapscale)), boundriesCol);
        }
        void drawRays(Ray[] rays, Pixel colour) {
            foreach (Ray ray in rays) { drawRay(ray, colour); }
        }
        void drawRay(Ray ray, Pixel col) {
            DrawLine(new Point((int)(ray.pos.X * mapscale), (int)(ray.pos.Y * mapscale)), new Point((int)((ray.pos.X*mapscale)+(ray.dir.X*ray.distance*mapscale)), (int)((ray.pos.Y*mapscale)+(ray.dir.Y*ray.distance*mapscale))), col);
        }

        public override void OnUpdate(float delta) {
            FillRect(Point.Origin, (int)mapCanvasWidth, (int)mapCanvasHeight, backGroundCol);
            for (int i = 0; i <= mapWidth; i++) { DrawLine(new Point((int)(i*mapscale), 0), new Point((int)(i*mapscale), (int)(mapHeight*mapscale)), gridLineCol); } //draw grid horizontal
            for (int i = 0; i <= mapHeight; i++) { DrawLine(new Point(0, (int)(i*mapscale)), new Point((int)(mapWidth*mapscale), (int)(i*mapscale)), gridLineCol); } //draw grid vertical
            drawBoundries(walls.ToArray());
            
            float[] distances = new float[Player.rays.Length];
            float[] wallPoints = new float[Player.rays.Length];
            Sprite[] textures = new Sprite[Player.rays.Length];
            int index = Player.rays.Length-1;
            foreach (Ray r in Player.rays) {
                r.cast(walls.ToArray());
                if (r.hitSomething) {
                    drawRay(r, Pixel.Presets.Green);
                } else {
                    drawRay(r, Pixel.Presets.Red);
                }
                textures[index] = r.hitWall.texture;
                wallPoints[index] = r.DistAlongWall;
                distances[index] = r.distance*Cos(Math.Abs(r.angle-Player.angle));
                index--;
            }
            
            //draw the player
            Draw((int)(Player.Pos.X*mapscale), (int)(Player.Pos.Y*mapscale), Pixel.Presets.Yellow);
            DrawLine(new Point((int)(Player.Pos.X * mapscale), (int)(Player.Pos.Y * mapscale)), new Point((int)((Player.Pos.X*mapscale)+(Player.dir.X*0.5f*mapscale)), (int)((Player.Pos.Y*mapscale)+(Player.dir.Y*0.5f*mapscale))), Pixel.Presets.Yellow);

            //draw camera view
            //view canvas is from 1/2 to full width
            for (int x = (int)mapCanvasWidth+1; x < ScreenWidth; x++) {
                index = (int)Map(x, mapCanvasWidth, ScreenWidth, 0, distances.Length);
                float distanceToWall = distances[index];
                int ceilling = (int)((float)(ScreenHeight / 2.0) - ScreenHeight / ((float)distanceToWall));
                int floor = ScreenHeight - ceilling;
                float sampleX = wallPoints[index];

                for (int y = 0; y < ScreenHeight; y++) { 
                    if (y <= ceilling) { //current cell is part of the ceiling
                        Draw(x,y, Pixel.Presets.Cyan); //draw ceiling
                        //shade ceiling based on distance
                        float b = 0;// ((y - ScreenHeight / 2f) / (ScreenHeight / 2f));
                        Draw(x, y, new Pixel(0, 0, 0, (byte)(b * 255)));
                    }
                    else if (y > ceilling && y < floor) { //current cell is part of the wall
                        //put texture logic in here
                        float sampleY = (y-(float)ceilling)/(floor - ceilling);
                        Draw(x, y, sampleTexture(textures[index], sampleX, sampleY));
                        Draw(x, y, new Pixel(0, 0, 0, (byte)(Clamp(distanceToWall / depthOfField, 0, 1) * 255) ));
                    }
                    else { //current cell is part of the floor
                        float distToFloor = (y - ScreenHeight / 2f)/(0.5f*ScreenHeight);
                        distToFloor       = (float)Math.Atan(distToFloor / screenDistance);
                        distToFloor       = PI/2 - distToFloor;
                        distToFloor       = (float)Math.Cos(distToFloor);
                        distToFloor       = distToFloor * playerHeight;

                        if (Math.Abs((1/distToFloor)%1*Sin(Player.angle))+Math.Abs((1/distToFloor)%1*Cos(Player.angle)) < 0.2) {
                            Draw(x, y, Pixel.Presets.Teal);
                        } else {
                            Draw(x, y, Pixel.Presets.Green);
                        }
                        //shade floor based on distance
                        Draw(x, y, new Pixel(0, 0, 0, (byte)(255-(distToFloor*255))));
                    }
                }
            }

            PlayerMovement(delta);
            AppName = (1/delta).ToString();

            //DrawText(new Point(2,2), "PlayerHeight: " + playerHeight, Pixel.Presets.White);
            //DrawText(new Point(2,8), "ScreenDist: " + screenDistance, Pixel.Presets.White);
        }

        //============movement==============
        void PlayerMovement(float delta) 
        {
            float runModifier = movement.run ? 2 : 1;
            if (movement.clockwise) {
                Player.turn(movement.turnSpeed*runModifier*delta);
            }
            if (movement.counterclockwise) {
                Player.turn(-movement.turnSpeed * runModifier * delta);

            }
            if (movement.forwards) {
                Player.move(0, Cos(Player.angle) * movement.moveSpeed * delta * runModifier);
                Player.checkUp.cast(walls.ToArray());
                Player.checkDown.cast(walls.ToArray());
                if (Player.checkUp.distance <= Player.radius || Player.checkDown.distance <= Player.radius) {
                    Player.move(0, -Cos(Player.angle) * movement.moveSpeed * delta * runModifier);
                }
                Player.move(Sin(Player.angle) * movement.moveSpeed * delta * runModifier, 0);
                Player.checkLeft.cast(walls.ToArray());
                Player.checkRight.cast(walls.ToArray());
                if (Player.checkLeft.distance <= Player.radius || Player.checkRight.distance <= Player.radius) {
                    Player.move(-Sin(Player.angle) * movement.moveSpeed * delta * runModifier, 0);
                }
            }
            if (movement.backwards) {
                Player.move(0, -Cos(Player.angle) * movement.moveSpeed * delta * runModifier);
                Player.checkUp.cast(walls.ToArray());
                Player.checkDown.cast(walls.ToArray());
                if (Player.checkUp.distance <= Player.radius || Player.checkDown.distance <= Player.radius) {
                    Player.move(0, Cos(Player.angle) * movement.moveSpeed * delta * runModifier);
                }
                Player.move(-Sin(Player.angle) * movement.moveSpeed * delta * runModifier, 0);
                Player.checkLeft.cast(walls.ToArray());
                Player.checkRight.cast(walls.ToArray());
                if (Player.checkLeft.distance <= Player.radius || Player.checkRight.distance <= Player.radius) {
                    Player.move(Sin(Player.angle) * movement.moveSpeed * delta * runModifier, 0);
                }
            }
            if (movement.right) {
                Player.move(Cos(Player.angle) * movement.moveSpeed * delta * runModifier, 0);
                Player.checkLeft.cast(walls.ToArray());
                Player.checkRight.cast(walls.ToArray());
                if (Player.checkLeft.distance <= Player.radius || Player.checkRight.distance <= Player.radius) {
                    Player.move(-Cos(Player.angle) * movement.moveSpeed * delta * runModifier, 0);
                }
                Player.move(0, -Sin(Player.angle) * movement.moveSpeed * delta * runModifier);
                Player.checkUp.cast(walls.ToArray());
                Player.checkDown.cast(walls.ToArray());
                if (Player.checkUp.distance <= Player.radius || Player.checkDown.distance <= Player.radius) {
                    Player.move(0, Sin(Player.angle) * movement.moveSpeed * delta * runModifier);
                }
            }
            if (movement.left) {
                Player.move(-Cos(Player.angle) * movement.moveSpeed * delta * runModifier, 0);
                Player.checkLeft.cast(walls.ToArray());
                Player.checkRight.cast(walls.ToArray());
                if (Player.checkLeft.distance <= Player.radius || Player.checkRight.distance <= Player.radius) {
                    Player.move(Cos(Player.angle) * movement.moveSpeed * delta * runModifier, 0);
                }
                Player.move(0, Sin(Player.angle) * movement.moveSpeed * delta * runModifier);
                Player.checkUp.cast(walls.ToArray());
                Player.checkDown.cast(walls.ToArray());
                if (Player.checkUp.distance <= Player.radius || Player.checkDown.distance <= Player.radius) {
                    Player.move(0, -Sin(Player.angle) * movement.moveSpeed * delta * runModifier);
                }
            }
        }
        public override void OnKeyDown(Key k)
        {   //controlls
            //ccw rotation
            if ((k == Key.A || k == Key.Num4) && !movement.counterclockwise) {
                movement.counterclockwise = true;
            }
            //cw rotation
            if ((k == Key.D || k == Key.Num6) && !movement.clockwise) {
                movement.clockwise = true;
            }
            //left
            if ((k == Key.Q || k == Key.Num7) && !movement.left) {
                movement.left = true;
            }
            //right
            if ((k == Key.E || k == Key.Num9) && !movement.right) {
                movement.right = true;
            }
            //forwards
            if ((k == Key.W || k == Key.Num8) && !movement.forwards) {
                movement.forwards = true;
            }
            //backwards
            if ((k == Key.S || k == Key.Num5) && !movement.backwards) {
                movement.backwards = true;
            }
            //run
            if ((k == Key.LShift || k == Key.RShift || k == Key.Shift || k == Key.Separator) && !movement.run) {
                movement.run = true;
            }

            //player to screen distance
            if (k == Key.Plus) { screenDistance += 0.1f; }
            if (k == Key.Minus) { screenDistance -= 0.1f; }
            //player height
            if (k == Key.K1) { playerHeight += 0.1f; }
            if (k == Key.K2) { playerHeight -= 0.1f; }
        }
        public override void OnKeyRelease(Key k)
        {   //controlls
            //ccw rotation
            if ((k == Key.A || k == Key.Num4) && movement.counterclockwise) {
                movement.counterclockwise = false;
            }
            //cw rotation
            if ((k == Key.D || k == Key.Num6) && movement.clockwise) {
                movement.clockwise = false;
            }
            //left
            if ((k == Key.Q || k == Key.Num7) && movement.left) {
                movement.left = false;
            }
            //right
            if ((k == Key.E || k == Key.Num9) && movement.right) {
                movement.right = false;
            }
            //forwards
            if ((k == Key.W || k == Key.Num8) && movement.forwards) {
                movement.forwards = false;
            }
            //backwards
            if ((k == Key.S || k == Key.Num5) && movement.backwards) {
                movement.backwards = false;
            }
            //run
            if ((k == Key.LShift || k == Key.RShift || k == Key.Shift || k == Key.Separator) && movement.run) {
                movement.run = false;
            }
        }
        public Pixel sampleTexture(Sprite sprite, float sx, float sy)
        {
            return sprite[(int)((sx % 1) * sprite.Width), (int)((sy % 1) * sprite.Height)];
        }
    }

    class Boundry {
        public System.Windows.Point a;
        public System.Windows.Point b;
        public float length { get { return (float)Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y)); } }
        public Sprite texture { get; private set; }

        public Boundry(System.Windows.Point start, System.Windows.Point end, Sprite _texture) {
            a = start; b = end;
            texture = _texture;
        }
        public Boundry(float x1, float y1, float x2, float y2, Sprite _texture) {
            a = new System.Windows.Point(x1, y1);
            b = new System.Windows.Point(x2, y2);
            texture = _texture;
        }
        public Boundry() {
            a = new System.Windows.Point();
            b = new System.Windows.Point();
            texture = new Sprite(1,1);
            texture[0, 0] = Pixel.Presets.White;
        }
    }
    class Ray {
        private const float bigNumber = 100f;
        public float angle { get; private set; }
        public System.Windows.Point pos { get; private set; }
        public System.Windows.Vector dir { get { return new System.Windows.Vector(Math.Sin(angle), Math.Cos(angle)); } }
        public float distance { get; private set; }
        public bool hitSomething { get; private set; }
        public float DistAlongWall { get; private set; }
        public Boundry hitWall { get; private set; }
        public Ray() {
            pos = new System.Windows.Point(0,0);
            angle = 0;
        }
        public Ray(float x, float y, float a) {
            pos = new System.Windows.Point(x,y);
            angle = a;
        }
        public Ray(System.Windows.Point point , float a) {
            pos = point;
            angle = a;
        }
        public void pointAt(double x, double y) {
            angle = (float)Math.Atan2(x - pos.X, y - pos.Y);
        }
        public void pointAt(float a) {
            angle = a;
        }
        public void turn(float a) {
            angle += a;
        }
        public void moveTo(float x, float y) {
            pos = new System.Windows.Point(x, y);
        }
        public void move(float x, float y) {
            pos = new System.Windows.Point(pos.X + x, pos.Y + y);
        }
        public float distanceTo(Boundry wall) {
            //https://en.wikipedia.org/wiki/Line%E2%80%93line_intersection#Given_two_points_on_each_line_segment
            double x1 = wall.a.X;
            double y1 = wall.a.Y;
            double x2 = wall.b.X;
            double y2 = wall.b.Y;

            double x3 = pos.X;
            double y3 = pos.Y;
            double x4 = pos.X+dir.X;
            double y4 = pos.Y+dir.Y;

            double denominator = (x1-x2)*(y3-y4)-(y1-y2)*(x3-x4);
            if (denominator == 0) {//they are parallel
                return bigNumber;
            }

            double t = ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / denominator;
            double u = -((x1 - x2) * (y1 - y3) - (y1 - y2) * (x1 - x3)) / denominator;

            if ((0 < t && t < 1) && u > 0) {
                double endX = x1 + t * (x2-x1); 
                double endY = y1 + t * (y2-y1);
                hitWall = wall;
                DistAlongWall = (float)t*wall.length;
                float dist = (float)Math.Sqrt((endX-x3)*(endX-x3)+(endY-y3)*(endY-y3));
                return dist;
            } else {
                return bigNumber;
            }

        }
        public void cast(Boundry[] walls) {
            distance = bigNumber;
            hitSomething = false;
            float wallProportion = 0;
            Boundry wallhit = null;
            foreach (Boundry wall in walls) {
                float d = distanceTo(wall);
                if (d < distance) {
                    hitSomething = true;
                    distance = d;
                    wallhit = hitWall;
                    wallProportion = DistAlongWall;
                }
            }
            hitWall = wallhit;
            DistAlongWall = wallProportion;
        }
    }
    class Player {
        public System.Windows.Point Pos { get; private set; }
        public System.Windows.Vector dir { get { return new System.Windows.Vector(Math.Sin(angle), Math.Cos(angle)); } }
        public float angle { get; private set; }
        public readonly float radius = 0.25f;
        public readonly float FOV = (float)(45*(Math.PI/180)); //in radians
        private readonly float RaysPerRadian = 300;
        public Ray[] rays;
        public Ray checkUp;
        public Ray checkDown;
        public Ray checkLeft;
        public Ray checkRight;

        public Player(System.Windows.Point position, float ang) {
            angle = ang;
            Pos = position;
            rays = new Ray[(int)(FOV*RaysPerRadian)];
            
            float cwEdge  = ang + FOV / 2;
            float rayA;
            for (int index = 0; index < rays.Length; index++) {
                float progress = (index / (float)rays.Length) - 0.5f;
                rayA = ang+(progress*cwEdge);
                rays[index] = new Ray(Pos, rayA);
            }

            checkUp = new Ray(Pos, 0);
            checkLeft = new Ray(Pos, Game.PI/2);
            checkDown = new Ray(Pos, Game.PI);
            checkRight = new Ray(Pos, 3*Game.PI/2);
        }

        public void move(float x, float y) {
            Pos = new System.Windows.Point(Pos.X + x, Pos.Y + y);
            foreach (Ray r in rays) {
                r.move(x,y);
            }
            checkUp.moveTo((float)Pos.X, (float)Pos.Y);
            checkLeft.moveTo((float)Pos.X, (float)Pos.Y);
            checkDown.moveTo((float)Pos.X, (float)Pos.Y);
            checkRight.moveTo((float)Pos.X, (float)Pos.Y);
        }
        public void moveTo(float x, float y) {
            Pos = new System.Windows.Point(x, y);
            foreach (Ray r in rays) {
                r.move(x,y);
            }
            checkUp.moveTo((float)Pos.X, (float)Pos.Y);
            checkLeft.moveTo((float)Pos.X, (float)Pos.Y);
            checkDown.moveTo((float)Pos.X, (float)Pos.Y);
            checkRight.moveTo((float)Pos.X, (float)Pos.Y);
        }
        public void turn(float a) {
            pointAt(angle+a);
        }
        public void pointAt(double x, double y) {
            angle = (float)Math.Atan2(x-Pos.X, y-Pos.Y);
            for (int index = 0; index < rays.Length; index++) {
                float turnAmt = -(index-rays.Length/2)/(rays.Length*0.5f)/(2f*FOV);
                rays[index].pointAt(angle+turnAmt);
            }
        }
        public void pointAt(float a) {
            angle = a;
            for (int index = 0; index < rays.Length; index++) {
                float turnAmt = -(index-rays.Length/2)/(rays.Length*0.5f)/(2f*FOV);
                rays[index].pointAt(angle+turnAmt);
            }
        }
    }
}