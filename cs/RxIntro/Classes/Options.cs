using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommandLine;

namespace RxIntro
{
    class Options
    {
        [Option('p', "part", Required = false, Default = 1)]
        public int Part { get; set; }

        [Option('e', "example", Required = false, Default = 1)]
        public int Example { get; set; }
    }

    public abstract class Part
    {
        public abstract void Example1();
        public abstract void Example2();
        public abstract void Example3();
        public abstract void Example4();
        public abstract void Example5();
        public abstract void Example6();
        public abstract void Example7();
        public abstract void Example8();
        public abstract void Example9();

        public async Task Exec(int exampleId)
        {            
            var t = this.GetType().GetMethod($"Example{exampleId}").Invoke(this, null) as Task;            
            if (t != null)
            {                
                await t;
            }
        }
    }
}

