using System;
using System.Collections.Generic;

namespace BinaryNumbersQueue
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter a positive number: ");
            string userInput = Console.ReadLine();

            if (!int.TryParse(userInput, out int n) || n <= 0)
            {
                Console.WriteLine("Please enter a valid positive integer.");
                return;
            }

            Queue<string> queue = new Queue<string>();
            queue.Enqueue("1");

            Console.WriteLine($"\nBinary numbers from 1 to {n}:\n");
            Console.WriteLine($"{"Decimal",-10} {"Binary",-20}");
            Console.WriteLine(new string('-', 30));

            for (int i = 1; i <= n; i++)
            {
                string binary = queue.Dequeue();
                Console.WriteLine($"{i,-10} {binary,-20}");

                queue.Enqueue(binary + "0");
                queue.Enqueue(binary + "1");
            }
        }
    }
}