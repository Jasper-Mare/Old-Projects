using System;
using System.Diagnostics;
using System.Collections.Generic;
using static System.Console;

namespace SortingAlgoritms
{
    class Program
    {
        static void Main()
        {
            while (true) {
                Write("Length of list: ");
                int Numb;
                if (!int.TryParse(ReadLine(), out Numb)) { Numb = 10; }

                Stopwatch watch = new Stopwatch();
                //mysort
                WriteLine("Mysort");
                List<int> Values = GenerateList(Numb, 0, 9);

                watch.Start();
                Values = Mysort(Values);
                watch.Stop();
                WriteLine(watch.Elapsed);

                watch.Reset();
                //Bubble sort
                WriteLine("Bubble sort");
                Values = GenerateList(Numb, 0, 9);

                watch.Start();
                Values = BubbleSort(Values);
                watch.Stop();

                WriteLine(watch.Elapsed);
                watch.Reset();
            }
        }

        static void WriteList(List<int> list) {
            foreach (int i in list) {
                Write("| {0, 1} ", i);
            }
            WriteLine("|");
        }

        static List<int> GenerateList(int length, int min, int max) {
            List<int> Generated = new List<int>();
            Random rand = new Random();

            for (int i = 0; i < length; i++) {
                Generated.Add(rand.Next(min, max + 1));
            }

            return Generated;
        }

        //----------------------------------------------------- sorts ------------------------------------------
        static List<int> Mysort(List<int> original) {
            List<int> NewList = new List<int>();
            
            for (int index_i = 0; index_i < original.Count; index_i++) {
                int i = original[index_i];
                if (NewList.Count == 0) { NewList.Add(i); }
                else {
                    bool iAdded = false;
                    for (int index_j = 0; index_j < NewList.Count; index_j++) {
                        int j = NewList[index_j];
                        if (i <= j) {
                            NewList.Insert(index_j, i); //insert i one space before index_j
                            iAdded = true;
                            break;
                        }   
                    }
                    if (iAdded == false) {
                        NewList.Add(i); //add to end
                    }
                }
                //WriteList(NewList);
            }

            return NewList;
        }

        static List<int> BubbleSort(List<int> a) {
            int t;
            for (int p = 0; p <= a.Count - 2; p++)
            {
                for (int i = 0; i <= a.Count - 2; i++)
                {
                    if (a[i] > a[i + 1])
                    {
                        t = a[i + 1];
                        a[i + 1] = a[i];
                        a[i] = t;
                    }
                }
            }

            return a;
        }
    }
}
