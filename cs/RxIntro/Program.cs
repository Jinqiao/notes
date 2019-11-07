using System;
using CommandLine;

namespace RxIntro
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {                       
                       Part part = (Part)System.Reflection.Assembly.GetExecutingAssembly().CreateInstance($"RxIntro.Part{o.Part}");                       
                       part.Exec(o.Example).Wait();
                   });
        }
    }
}
