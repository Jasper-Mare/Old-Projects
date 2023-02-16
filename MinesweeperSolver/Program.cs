using System;

namespace MinesweeperSolver {

    internal class Program {
        const byte size = 10;
        const float proportion = 0.1f;
        static bool[,] mines = new bool[size, size];
        static byte[,] mineNumbers = new byte[size, size];
        static BoardState[,] playerView = new BoardState[size, size];


        static void Main(string[] args) {

            //setup
            Setup();

            DrawPlayerView();
            DrawMines();
            DrawMineNumbers();

            while (!IsSolved()) {
                byte x, y;
                for (x = 0; x < size; x++) {
                    for (y = 0; y < size; y++) {

                        BoardState[,] snapshots = GetSnapshot(x,y);



                        DrawPlayerView();
                        DrawMines();
                        DrawMineNumbers();
                    }
                }

            }

        }

        static void Setup() {
            int numMines = (int)(size*size*proportion);
            Random random = new Random();

            while (numMines != 0) {
                int x = random.Next(size), y = random.Next(size);
                if (x == size / 2 && y == size / 2) { continue; } //leave the centre

                if (!mines[x, y]) {
                    numMines--;
                    mines[x, y] = true;
                }
            }

            for (byte x = 0; x < size; x++) {
                for (byte y = 0; y < size; y++) {
                    byte numberOfMines = 0;

                    playerView[x, y] = BoardState.notDug;
                    if (mines[x, y]) { continue; }

                    for (byte i = 0; i < 9; i++) {
                        if (i == 4) { continue; }

                        byte xpos = (byte)(x+i%3-1), ypos = (byte)(y+i/3-1);
                        if (!IsInGrid(xpos, ypos)) { continue; }

                        if (mines[xpos, ypos]) { numberOfMines++; }
                    }

                    mineNumbers[x, y] = numberOfMines;
                }
            }

            Dig(size / 2, size / 2);

        }

        static void DrawMines() {
            Console.WriteLine();
            for (byte y = 0; y < size; y++) {
                Console.Write(" ");
                for (byte x = 0; x < size; x++) {
                    Console.Write(mines[x, y] ? "M" : "·");
                }
                Console.WriteLine();
            }

        }
        static void DrawMineNumbers() {
            Console.WriteLine();
            for (byte y = 0; y < size; y++) {
                Console.Write(" ");
                for (byte x = 0; x < size; x++) {
                    if (mineNumbers[x, y] == 0) {
                        Console.Write("·");
                    } else {
                        Console.Write(mineNumbers[x, y]);
                    }
                }
                Console.WriteLine();
            }

        }
        static void DrawPlayerView() {
            Console.WriteLine();
            for (byte y = 0; y < size; y++) {
                Console.Write(" ");
                for (byte x = 0; x < size; x++) {
                    BoardState state = playerView[x, y];
                    if (1 <= (byte)state && (byte)state <= 8) {
                        Console.Write((byte)state);
                    } else if (state == BoardState.notDug) {
                        Console.Write("█");
                    } else if (state == BoardState.knownEmpty) {
                        Console.Write("·");
                    } else if (state == BoardState.flagged) {
                        Console.Write("F");
                    }


                }
                Console.WriteLine();
            }

        }

        //returns true if dead
        static bool Dig(byte x, byte y, bool recurse = true) {
            if (mines[x, y]) {
                return true;
            }

            playerView[x, y] = (BoardState)mineNumbers[x, y];

            Console.Clear();
            DrawPlayerView();
            DrawMines();
            DrawMineNumbers();
            System.Threading.Thread.Sleep(50);
            Console.SetCursorPosition(x + 1, y + 1);
            Console.Write("@");
            System.Threading.Thread.Sleep(50);
            Console.SetCursorPosition(0, 0);
            Console.Clear();

            if (!recurse) { return false; }
            if (playerView[x, y] != BoardState.knownEmpty) { recurse = false; }

            for (byte i = 0; i < 9; i++) {
                if (i % 2 == 0) { continue; }

                byte xpos = (byte)(x + i % 3 - 1), ypos = (byte)(y + i / 3 - 1);
                if (!IsInGrid(xpos, ypos)) { continue; }
                if (playerView[xpos, ypos] != BoardState.notDug) { continue; }

                if (!mines[x, y]) {
                    Dig(xpos, ypos, recurse);
                }

            }
            return false;
        }

        static bool IsInGrid(byte xpos, byte ypos) {
            if (xpos < 0 || size - 1 < xpos) { return false; }
            if (ypos < 0 || size - 1 < ypos) { return false; }
            return true;
        }
        static bool IsSolved() {
            for (byte x = 0; x < size; x++) {
                for (byte y = 0; y < size; y++) {
                    if (!mines[x,y] && playerView[x,y] == BoardState.notDug) { return false; }
                }
            }
            return true;
        }

        static BoardState[,] GetSnapshot(byte x, byte y) {
            BoardState[,] snapshot = new BoardState[3,3];

            for (byte i = 0; i < 9; i++) {
                byte xpos = (byte)(i%3), ypos = (byte)(i/3);
                if (!IsInGrid((byte)(xpos+x-1), (byte)(ypos+x-1))) { snapshot[xpos, ypos] = BoardState.knownEmpty; } 
                else { snapshot[xpos, ypos] = (BoardState)mineNumbers[(byte)(xpos+x-1), (byte)(ypos+x-1)]; }
                
            }

            return snapshot;
        }

    }

    enum BoardState : byte {
        knownEmpty = 0,
        flagged = 9,
        notDug = 10,
    }
}