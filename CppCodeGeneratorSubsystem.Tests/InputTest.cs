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
    }
}
