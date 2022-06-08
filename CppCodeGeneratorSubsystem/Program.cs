using System;
using System.Collections.Generic;

namespace CppCodeGeneratorSubsystem
{
    class Program
    {
        static void Main(string[] args)
        {
            Repository.AvailableTypes["<string>"]        .Add(new Element(Format.Class, "std::string"));
            Repository.AvailableTypes["\"my_class.h\""]  .Add(new Element(Format.Class, "my_class"));
            Repository.AvailableTypes["\"my_library.h\""].Add(new Element(Format.Alias, "my_library::callback", "my_class", "std::string"));
            Repository.AvailableTypes["\"my_library.h\""].Add(new Element(Format.Struct, "my_library::struct1<T1,T2,T3>"));
            Repository.AvailableTypes["\"typedefs.h\""]  .Add(new Element(Format.Struct, "my_library::struct1<T1,T2,T3>"));
            Repository.AvailableTypes["\"test.h\""].Add(new Element(Format.Class, "callback"));
            Repository.AvailableTypes["\"test.h\""].Add(new Element(Format.Struct, "struct1<T1,T2,T3>"));

            Сompiler compiler = new Сompiler() 
            { 
                //Includes =     { "std::string", "my_library::callback" },
                //Declarations = { "callback", "struct1<T1,T2,T3>" }
                  Declarations = { "my_library::callback", "my_library::struct1<T1,T2,T3>" }
                //Declarations = { "callback", "struct1<T1,T2,T3>", "my_library::callback", "my_library::struct1<T1,T2,T3>" }
            };

            Console.WriteLine(compiler.BildOutput());
        }
    }
}
