using System;
using System.Collections.Generic;

namespace CppCodeGeneratorSubsystem
{
    class Program
    {
        static void Main(string[] args)
        {
            Repository Repository = Repository.GetRepository();
 
            Сompiler compiler = new Сompiler(Repository)
            {
                Declarations = { "my_library1::callback1", "my_library2::callback1", "my_library1::callback2", "my_library2::callback2", "my_library1::callback3", "my_library2::callback3" }
            };


            Console.WriteLine(compiler.BuildOutput());
        }
    }
}
