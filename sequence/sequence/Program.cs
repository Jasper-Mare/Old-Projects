using System;

namespace sequence {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("Set console full screen"); Console.ReadLine();
            for (int i = 0; i < 100; i++) {
                int total = i;
                Console.Write("  | ");
                for (int a = 0; a < i; a++) {
                    total += a;
                    Console.BackgroundColor = (ConsoleColor)((a+8)%16);
                    Console.ForegroundColor = (ConsoleColor)(a%16);
                    Console.Write(a+"+");
                }
                Console.Write(i);
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.WriteLine(total);
            }
            Console.WriteLine(Math.Pow(0,0).ToString());
            Console.ReadLine();
        }
    }
}
