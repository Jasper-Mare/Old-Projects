using Raylib_cs;

namespace FighterPilotSim {
    static class Options {
        //public static
        public static int Width = 1600;
        public static int Height = 900;
        public static string playerName = "";

    }

    static class Resources { 
        public static Image Icon;
        public static Model Plane;

        public static void Setup() {
            Icon = Raylib.LoadImage(@"Resources\icon.png");
            Plane = Raylib.LoadModel(@"Resources\plane.obj");
        }
    }
}
