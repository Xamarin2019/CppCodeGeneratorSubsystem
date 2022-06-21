using System;
using Xunit;
using Xunit.Abstractions;

namespace CppCodeGeneratorSubsystem.Tests
{
    public class OutputTest : RedirectConsoleOutput
    {
        public OutputTest(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void Test1()
        {
            // Arrange
            string expected = @"#include <string>

                                class my_class;

                                namespace my_library
                                {
                                    using callback = my_class* (*) (std::string);
                                    tempalate<typename T1, typename T2, typename T3> struct struct1;
                                }";

            Repository Repository = new Repository();
            Repository.AvailableTypes["<string>"].AddClass("std", "string");
            Repository.AvailableTypes["\"my_class.h\""].AddClass("my_class");
            Repository.AvailableTypes["\"my_library.h\""].AddAlias("my_library", "callback", "my_class", "std::string");
            Repository.AvailableTypes["\"my_library.h\""].AddStruct("my_library", "struct1<T1,T2,T3>");

            Сompiler compiler = new Сompiler(Repository)
            {
                Declarations = { "my_library::struct1<T1,T2,T3>", "my_library::callback" }
            };

            // Act
            Console.WriteLine(compiler.BuildOutput());

            var result = textWriter;

            // Assert
            var actual = result.ToString().Replace(" ", string.Empty).Trim();
            Assert.True(actual == expected.Replace(" ", string.Empty).Trim());
        }

        [Fact]
        public void Test2()
        {
            // Arrange
            string expected = @"#include<string>

                                namespace my_library
                                {
                                        clas smy_class;
                                        using callback=my_library::my_class*(*)(std::string);
                                        tempalate<typenameT1,typenameT2,typenameT3> structstruct1;
                                }";

            Repository Repository = new Repository();
            Repository.AvailableTypes["<string>"].AddClass("std", "string");
            Repository.AvailableTypes["\"my_class.h\""].AddClass("my_library", "my_class");
            Repository.AvailableTypes["\"my_library.h\""].AddAlias("my_library", "callback", "my_library::my_class", "std::string");
            Repository.AvailableTypes["\"my_library.h\""].AddStruct("my_library", "struct1<T1,T2,T3>");

            Сompiler compiler = new Сompiler(Repository)
            {
                Declarations = { "my_library::struct1<T1,T2,T3>", "my_library::callback" }
            };

            // Act
            Console.WriteLine(compiler.BuildOutput());

            var result = textWriter;

            // Assert
            var actual = result.ToString().Replace(" ", string.Empty).Trim();
            Assert.True(actual == expected.Replace(" ", string.Empty).Trim());
        }

        [Fact]
        public void Test3()
        {
            // Arrange
            string expected = @"#include<string>

                                namespace my_library1
                                {
                                    using callback1=std::string*(*)(std::string);
                                }

                                namespace my_library2
                                {
                                    using callback1=my_library1::callback1*(*)(std::string);
                                }

                                namespacemy_library1
                                {
                                    using callback2=my_library2::callback1*(*)(std::string);
                                }

                                namespace my_library2
                                {
                                    using callback2=my_library1::callback2*(*)(std::string);
                                }

                                namespace my_library1
                                {
                                    using callback3=my_library2::callback2*(*)(std::string);
                                }

                                namespace my_library2
                                {
                                    using callback3=my_library1::callback3*(*)(std::string);
                                }";

            Repository Repository = new Repository();
            Repository.AvailableTypes["<string>"].AddClass("std", "string");
            Repository.AvailableTypes["\"my_library.h\""].AddAlias("my_library1", "callback1", "std::string", "std::string");
            Repository.AvailableTypes["\"my_library.h\""].AddAlias("my_library2", "callback1", "my_library1::callback1", "std::string");
            Repository.AvailableTypes["\"my_library.h\""].AddAlias("my_library1", "callback2", "my_library2::callback1", "std::string");
            Repository.AvailableTypes["\"my_library.h\""].AddAlias("my_library2", "callback2", "my_library1::callback2", "std::string");
            Repository.AvailableTypes["\"my_library.h\""].AddAlias("my_library1", "callback3", "my_library2::callback2", "std::string");
            Repository.AvailableTypes["\"my_library.h\""].AddAlias("my_library2", "callback3", "my_library1::callback3", "std::string");

            Сompiler compiler = new Сompiler(Repository)
            {
                Declarations = { "my_library1::callback1", "my_library2::callback1", "my_library1::callback2", "my_library2::callback2", "my_library1::callback3", "my_library2::callback3" }
            };

            // Act
            Console.WriteLine(compiler.BuildOutput());

            var result = textWriter;

            // Assert
            var actual = result.ToString().Replace(" ", string.Empty).Trim();
            Assert.True(actual == expected.Replace(" ", string.Empty).Trim());
        }

        [Fact]
        public void Test4()
        {
            // Arrange
            string expected = @"#include<string>

                                namespace my_library1
                                {
                                    using callback1=std::string*(*)(std::string);
                                }

                                namespace my_library2
                                {
                                    using callback1=my_library1::callback1*(*)(std::string);
                                    using callback3 = my_library1::callback1* (*)(std::string);
                                }

                                namespacemy_library1
                                {
                                    using callback2=my_library2::callback1*(*)(std::string);
                                }

                                namespace my_library2
                                {
                                    using callback2=my_library1::callback2*(*)(std::string);
                                }

                                namespace my_library1
                                {
                                    using callback3=my_library2::callback2*(*)(std::string);
                                }";

            Repository Repository = new Repository();
            Repository.AvailableTypes["<string>"].AddClass("std", "string");
            Repository.AvailableTypes["\"my_library.h\""].AddAlias("my_library1", "callback1", "std::string", "std::string");
            Repository.AvailableTypes["\"my_library.h\""].AddAlias("my_library2", "callback1", "my_library1::callback1", "std::string");
            Repository.AvailableTypes["\"my_library.h\""].AddAlias("my_library1", "callback2", "my_library2::callback1", "std::string");
            Repository.AvailableTypes["\"my_library.h\""].AddAlias("my_library2", "callback2", "my_library1::callback2", "std::string");
            Repository.AvailableTypes["\"my_library.h\""].AddAlias("my_library1", "callback3", "my_library2::callback2", "std::string");
            Repository.AvailableTypes["\"my_library.h\""].AddAlias("my_library2", "callback3", "my_library1::callback1", "std::string");

            Сompiler compiler = new Сompiler(Repository)
            {
                Declarations = { "my_library1::callback1", "my_library2::callback1", "my_library1::callback2", "my_library2::callback2", "my_library1::callback3", "my_library2::callback3" }
            };

            // Act
            Console.WriteLine(compiler.BuildOutput());

            var result = textWriter;

            // Assert
            var actual = result.ToString().Replace(" ", string.Empty).Trim();
            Assert.True(actual == expected.Replace(" ", string.Empty).Trim());
        }

        [Fact]
        public void Test5()
        {
            // Arrange

            Repository Repository = new Repository();
            Repository.AvailableTypes["<string>"].AddClass("std::string");
            Repository.AvailableTypes["\"my_library.h\""].AddAlias("my_library1::callback1", "std::string", "std::string");

            Сompiler compiler = new Сompiler(Repository)
            {
                Declarations = { "callback1", "my_library2::callback1" }
            };

            // Act

            // Assert
            Assert.Throws<NullReferenceException>(() => compiler.BuildOutput());
        }

    }
}