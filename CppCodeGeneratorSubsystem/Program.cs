using System;
using System.Collections.Generic;

namespace CppCodeGeneratorSubsystem
{
    class Program
    {
        static void Main(string[] args)
        {
            Repository Repository = Repository.GetRepository();

            //Repository.AvailableTypes["<string>"].Add(new Element(Format.Class, "std::string"));
            //Repository.AvailableTypes["\"my_class.h\""].Add(new Element(Format.Class, "my_class"));
            //Repository.AvailableTypes["\"my_library.h\""].Add(new Element(Format.Alias, "my_library::callback", "my_class", "std::string"));
            //Repository.AvailableTypes["\"my_library.h\""].Add(new Element(Format.Struct, "my_library::struct1<T1,T2,T3>"));

            //Сompiler compiler = new Сompiler()
            //{
            //    Declarations = { "my_library::struct1<T1,T2,T3>", "my_library::callback" }
            //};

            //Repository.AvailableTypes["<string>"].Add(new Element(Format.Class, "std::string"));
            //Repository.AvailableTypes["\"my_class.h\""].Add(new Element(Format.Class, "my_library::my_class"));
            //Repository.AvailableTypes["\"my_library.h\""].Add(new Element(Format.Alias, "my_library::callback", "my_library::my_class", "std::string"));
            //Repository.AvailableTypes["\"my_library.h\""].Add(new Element(Format.Struct, "my_library::struct1<T1,T2,T3>"));

            //Сompiler compiler = new Сompiler()
            //{
            //    Declarations = { "my_library::struct1<T1,T2,T3>", "my_library::callback" }
            //};


            //Repository.AvailableTypes["<string>"].Add(new Element(Format.Class, "std::string"));
            //Repository.AvailableTypes["\"my_library.h\""].Add(new Element(Format.Alias, "my_library1::callback1", "std::string", "std::string"));
            //Repository.AvailableTypes["\"my_library.h\""].Add(new Element(Format.Alias, "my_library2::callback1", "my_library1::callback1", "std::string"));
            //Repository.AvailableTypes["\"my_library.h\""].Add(new Element(Format.Alias, "my_library1::callback2", "my_library2::callback1", "std::string"));
            //Repository.AvailableTypes["\"my_library.h\""].Add(new Element(Format.Alias, "my_library2::callback2", "my_library1::callback2", "std::string"));
            //Repository.AvailableTypes["\"my_library.h\""].Add(new Element(Format.Alias, "my_library1::callback3", "my_library2::callback2", "std::string"));
            //Repository.AvailableTypes["\"my_library.h\""].Add(new Element(Format.Alias, "my_library2::callback3", "my_library1::callback3", "std::string"));

            Сompiler compiler = new Сompiler(Repository)
            {
                Declarations = { "my_library1::callback1", "my_library2::callback1", "my_library1::callback2", "my_library2::callback2", "my_library1::callback3", "my_library2::callback3" }
            };


            Console.WriteLine(compiler.BuildOutput());
        }
    }
}
