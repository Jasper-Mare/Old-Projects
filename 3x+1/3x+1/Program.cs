using System;
using System.Diagnostics;

namespace _3x_1 {
    class Program {
        static void Main(string[] args) {
            Stopwatch watch = new Stopwatch();
            bool end = false;
            int seed = 1;
            watch.Start();
            while (!end) {
                Console.WriteLine(seed);
                int number = seed;
                while (number != 1) {
                    if (number % 2 == 0) { //even
                        number /= 2;
                    } else { //odd
                        number *= 3;
                        number++;
                    }
                }

                if (seed % 1000 == 0) { 
                    watch.Stop(); 
                    Console.WriteLine(watch.Elapsed.ToString()); 
                    Console.Write(":"); 
                    end = (Console.ReadLine() != ""); 
                    watch.Restart(); 
                }
                seed++;
            }
            
            Console.ReadLine();
        }
    }
}
