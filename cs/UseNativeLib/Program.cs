using System;
using System.Runtime.InteropServices;

namespace UseNativeLib
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2) {
                Console.WriteLine("need at least 2 arguments");
                return;
            }

            double a = double.Parse(args[0]);
            double b = double.Parse(args[1]);
            Console.WriteLine($"a: {a}");
            Console.WriteLine($"b: {b}");
            Console.WriteLine($"my_add: {MyAdd(a, b)}");
            Console.WriteLine($"my_minus: {MyMinus(a, b)}");
            Console.WriteLine($"Finished");
        }

        // this will looks for libmylib.so on Linux
        [DllImport("mylib", EntryPoint = "my_add")]
        private static extern double MyAdd(double a, double b);

        [DllImport("mylib", EntryPoint = "my_minus")]
        private static extern double MyMinus(double a, double b);
    }
}
