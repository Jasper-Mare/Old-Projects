using System;
using System.Collections.Generic;
using PixelEngine;

namespace pseudo3d
{
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("1) Parallax scrolling");
            Console.WriteLine("2) Scaling along the Z axis (road)");
            Console.WriteLine("3) 1 point perspective");

            switch (Console.ReadLine()) {
                case "1":
                    parallax parallax = new parallax();
                    parallax.Construct(300,300,3,3);
                    parallax.Start();
                    break;

                case "2":
                    road3d road = new road3d();
                    road.Construct(160,100,8,8);
                    road.PixelMode = Pixel.Mode.Alpha;
                    road.Start();
                    break;

                case "3":
                    perspective1 perspective1 = new perspective1();
                    perspective1.Construct(300,300,3,3);
                    perspective1.Start();
                    break;
            }
        }
    }
}
