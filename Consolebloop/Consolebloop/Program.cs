using System;

namespace Consolebloop {
    class Program {
        static void Main(string[] args) {
            for (float i = 5.3f; i <= 10; i += 0.1f) {
                Console.Beep((int)MathF.Pow(2,i), 50);
            }
            Console.WriteLine("done");
            Console.Read();
        }
    }
}
