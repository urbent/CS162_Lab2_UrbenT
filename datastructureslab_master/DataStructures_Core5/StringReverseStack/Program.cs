using System;
using System.Collections.Generic;

namespace StringReverseStack
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter a string to reverse: ");
            string input = Console.ReadLine();

            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("You entered an empty string.");
                return;
            }

            Stack<char> stack = new Stack<char>();
            foreach (char c in input)
            {
                stack.Push(c);
            }

            string reversed = "";
            while (stack.Count > 0)
            {
                reversed += stack.Pop();
            }

            Console.WriteLine($"Original: {input}");
            Console.WriteLine($"Reversed: {reversed}");
        }
    }
}