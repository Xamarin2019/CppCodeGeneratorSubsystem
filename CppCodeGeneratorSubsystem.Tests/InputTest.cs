using System;
using System.Linq;
using Xunit;

namespace CppCodeGeneratorSubsystem.Tests
{
    public class InputTest
    {
        [Fact]
        public void Test_Element()
        {
            // Arrange
            string[] inputNames = {"ClassName",  "my_library::callback1", "std::string", "my_library::struct1<T1,T2,T3>" };
            string[] namespaces = { "",          "my_library",            "std",         "my_library"                    };
            string[] names =      { "ClassName", "callback1",             "string",      "struct1"                       };
            var elements = new Element[inputNames.Length];

            // Act
            var results = elements.Zip(inputNames, (element, inputName) => new Class(inputName)).ToArray();

            // Assert
            for (int i = 0; i < inputNames.Length; i++)
            {
                Assert.Equal(namespaces[i], results[i].Namespace);
                Assert.Equal(names[i], results[i].Name);
                Assert.Equal(inputNames[i], results[i].QualifiedName);
            }
 
        }

        [Fact]
        public void Test_Alias()
        {
            // Arrange
            string[] inputNames = { "ClassName", "my_library::callback1", "std::string", "my_library::struct1<T1,T2,T3>" };
            string[] namespaces = { "", "my_library", "std", "my_library" };
            string[] names = { "ClassName", "callback1", "string", "struct1" };
            var elements = new Element[inputNames.Length];

            // Act
            var inputType = new Class(inputNames[2]);
            var outputType = new Class(inputNames[0]);
            var result = new Alias(inputNames[1], inputNames[0], inputNames[2]);

            // Assert
            Assert.Equal(result.QualifiedName, inputNames[1]);
            Assert.Equal(result.NestedTypes[0].QualifiedName, outputType.QualifiedName);
            Assert.Equal(result.NestedTypes[1].QualifiedName, inputType.QualifiedName);
            Assert.Equal(result.NestedTypes[1].Namespace, inputType.Namespace);
            Assert.Throws<FormatException>(() => new Alias(inputNames[1], inputNames[0], " "));
        }

        [Fact]
        public void Test_Repo_GetType()
        {
            // Arrange
            string[] inputNames = { "ClassName", "my_library::callback1", "std::string", "my_library::struct1<T1,T2,T3>" };
            var elements = new Element[inputNames.Length];

            Repository Repository = new Repository();
            Repository.AvailableTypes["<string>"].Add(new Class("std::string"));
            Repository.AvailableTypes["\"my_class.h\""].Add(new Class("my_library::my_class"));
            Repository.AvailableTypes["\"my_library.h\""].Add(new Alias("my_library::callback", "my_library::my_class", "std::string"));
            Repository.AvailableTypes["\"my_library.h\""].Add(new Struct("my_library::struct1<T1,T2,T3>"));

            // Act
            //var element = Repository.GetType("std::string");
            elements = elements.Zip(inputNames, (element, inputName) => Repository.GetType(inputName)).ToArray();
            

            // Assert
            Assert.True(elements[0] == null);
            Assert.True(elements[1] == null);
            Assert.Equal(elements[2], new Class("std::string"));
            Assert.Equal(elements[3], new Struct("my_library::struct1<T1,T2,T3>"));
            //Assert.Throws<FormatException>(() => new Alias(inputNames[1], inputNames[0], " "));
        }

        [Fact]
        public void Test_Repo_GetFilename()
        {
            // Arrange
            string[] inputNames = { "ClassName", "my_library::callback1", "std::string", "my_library::struct1<T1,T2,T3>" };
            var fileNames = new string[inputNames.Length];

            Repository Repository = new Repository();
            Repository.AvailableTypes["<string>"].Add(new Class("std::string"));
            Repository.AvailableTypes["\"my_class.h\""].Add(new Class("my_library::my_class"));
            Repository.AvailableTypes["\"my_library.h\""].Add(new Alias("my_library::callback", "my_library::my_class", "std::string"));
            Repository.AvailableTypes["\"my_library.h\""].Add(new Struct("my_library::struct1<T1,T2,T3>"));

            // Act
            //var element = Repository.GetType("std::string");
            fileNames = fileNames.Zip(inputNames, (_, inputName) => Repository.GetFilename(inputName)).ToArray();


            // Assert
            Assert.True(fileNames[0] == null);
            Assert.True(fileNames[1] == null);
            Assert.Equal("<string>", fileNames[2]);
            Assert.Equal("\"my_library.h\"", fileNames[3]);
            //Assert.Throws<FormatException>(() => new Alias(inputNames[1], inputNames[0], " "));
        }
    }
}
