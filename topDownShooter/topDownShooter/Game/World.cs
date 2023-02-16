using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;


namespace topDownShooter.Game {
    class World {
        int width = 0, height = 0;
        TileType[,] tiles;

        public World(int width, int height) {
            this.width = width; this.height = height;
            tiles = new TileType[width, height];
        }

        public void readFromByteArray(byte[,] values) {
            tiles = new TileType[values.GetLength(0), values.GetLength(1)];
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    tiles[x,y] = (TileType)values[x,y];
                }
            }
        }

        public TileType this[int x, int y] {
            get {
                if (x < 0 || width < x) { return 0; }
                if (y < 0 || height < y) { return 0; }
                return tiles[x,y];
            }
            set {
                if (x < 0 || width < x) { return; }
                if (y < 0 || height < y) { return; }
                tiles[x,y] = value;
            }
        }

    }

    enum TileType : byte { 
        Empty = 0,
        Wall = 1,
    }
}
