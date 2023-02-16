using System;
using System.Collections.Generic;
using PixelEngine;

namespace pseudo3d
{
    class perspective1 : Game {
        float length = 1;
        float squareY = 1;
        public override void OnCreate() {
            PixelMode = Pixel.Mode.Alpha;
        }

        public override void OnUpdate(float delta) {
            if (GetKey(Key.Plus ).Down) { length+=length*delta; }
            if (GetKey(Key.Minus).Down) { length-=length*delta; }
            if (GetKey(Key.Up   ).Down) { squareY += delta; }
            if (GetKey(Key.Down ).Down) { squareY -= delta; }
            draw();
        }

        void draw() {
            Clear(Pixel.Presets.Black);
            DrawLine(new Point(0, 150), new Point(300, 150), Pixel.Presets.Cyan);
            //for (int x = -150; x <= 150; x+=10) {
            //    DrawLine(GetScreenPoint(x, 150, 0.001f), GetScreenPoint(x, 150, length), Pixel.Presets.White);
            //}
            DrawLine(GetScreenPoint(1, squareY, 1), GetScreenPoint(1, squareY, 2), Pixel.Presets.White);
            DrawLine(GetScreenPoint(1, squareY, 1), GetScreenPoint(2, squareY, 1), Pixel.Presets.White);
            DrawLine(GetScreenPoint(2, squareY, 2), GetScreenPoint(1, squareY, 2), Pixel.Presets.White);
            DrawLine(GetScreenPoint(2, squareY, 2), GetScreenPoint(2, squareY, 1), Pixel.Presets.White);
            DrawText(Point.Origin, squareY.ToString(), Pixel.Presets.Red);
        }

        Point GetScreenPoint(Point3d pnt) {
            return GetScreenPoint(pnt.X, pnt.Y, pnt.Z);
        }
        Point GetScreenPoint(float X, float Y, float Z) {
            Z *= 100;
            return new Point(150+(int)(X/Z), 150+(int)(Y/Z));
            /*f(x,y,z) = (x/z, y/z)
            then z is the depth in the scene and you can move back and forward
            with f(x,y,z,t) = (x/(z-t), y/(z-t))
            where t is the camera z position

            generally, if you have a camera transformation, you apply that before the projection transformation
            f(x-a,y-b,z-c) = ((x-a)/(z-c),(y-b)/(z-c)) to translate the camera so that (a,b,c) is the origin
            later you might want to apply scaling or rotation to the camera
            but notice that you subtract rather than add to translate the camera.
            the same rule applies to scaling and rotation. 
            generally you transform by the inverse of the camera matrix before you project
             */
        }

    }
    class Point3d { 
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public Point3d(float _X, float _Y, float _Z) {
            X = _X; Y = _Y; Z = _Z; 
        }
    }
}