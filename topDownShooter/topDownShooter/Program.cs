using static System.Console;

namespace topDownShooter {
    class Program {
        static void Main() {
            bool exit = false;

            Title = "Top-Down Shooter";
            CursorVisible = false;

            while (!exit) {
                //MainMenu

                switch (Menu("Top-Down Shooter", new string[] { "Start","Options","Exit" })) {
                    case 0: Game.Game game = new Game.Game(new Game.GameOptions()); game.Run(); break; //Start
                    case 1: break; //Options
                    case 2: exit = true; break; //Exit
                }

            }

            Clear();
            WriteLine("Goodbye...");
            System.Threading.Thread.Sleep(1000);
        }

        static int Menu(string title,string[] options) {
            System.ConsoleKey key = System.ConsoleKey.NoName;
            int currentOption = 0;

            //write out options
            Clear();
            WriteLine(title);
            foreach(string option in options) {
                WriteLine("  "+option);
            }

            while(key != System.ConsoleKey.Enter) {
                
                SetCursorPosition(0,currentOption+1);
                Write(">");

                key = ReadKey(true).Key;
                SetCursorPosition(0,currentOption+1);
                Write(" ");
                if (key == System.ConsoleKey.UpArrow)   { currentOption--; }
                if (key == System.ConsoleKey.DownArrow) { currentOption++; }
                currentOption = (currentOption+options.Length) % options.Length;

            }

            return currentOption;
        }
    }
}
