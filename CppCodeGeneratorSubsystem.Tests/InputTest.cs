using System;
using System.Linq;
using Xunit;
using Moq;
using Xunit.Abstractions;

namespace CppCodeGeneratorSubsystem.Tests
{
    public class InputTest : RedirectConsoleOutput
    {
        public InputTest(ITestOutputHelper output) : base(output) { }
 
        [Fact]
        public void Test_ElementList()
        {
            // Arrange
            Repository Repository = new Repository();
            Repository.AvailableTypes["<string>"].AddClass("std", "string");
            Repository.AvailableTypes["\"my_library.h\""].AddAlias("my_library1", "callback1", "std::string", "std::string");
            Repository.AvailableTypes["\"my_library.h\""].AddAlias("my_library1", "callback4", "std::string", "std::string");
            string[] Declarations = { "my_library1::callback1", "my_library2::callback1", "my_library1::callback2", "my_library2::callback2", "my_library1::callback3", "my_library2::callback3" };
            ElementList DeclarationsOutput = new ElementList(Repository.AvailableTypes);

            // Act
            Element element = Repository.AvailableTypes.FindElement("my_library1::callback1");
            DeclarationsOutput.Add(element);


            // Assert


        }

        [Fact]
        public void Test_ElementCopy()
        {
            // Arrange
            Element element = new Namespace("TestNamespace_1");
            element.AddNested(new Class("TestClass_1"));
            element.AddNested(new Class("TestClass_2"));

            Element nested = element.Nested[0];
            //Repository Repository = new Repository();
            //Repository.AvailableTypes["<string>"].AddClass("std", "string");
            //Repository.AvailableTypes["\"my_library.h\""].AddAlias("my_library1", "callback1", "std::string", "std::string");
            //Repository.AvailableTypes["\"my_library.h\""].AddAlias("my_library1", "callback4", "std::string", "std::string");
            //string[] Declarations = { "my_library1::callback1", "my_library2::callback1", "my_library1::callback2", "my_library2::callback2", "my_library1::callback3", "my_library2::callback3" };
            //ElementList DeclarationsOutput = new ElementList(Repository.AvailableTypes);

            // Act
            Element result = nested.Copy();


            Console.WriteLine(element);
            // Assert


        }

        //[Fact]
        //public void Test_Element()
        //{
        //    // Arrange
        //    string[] inputNames = {"ClassName",  "my_library::callback1", "std::string", "my_library::struct1<T1,T2,T3>" };
        //    string[] namespaces = { "",          "my_library",            "std",         "my_library"                    };
        //    string[] names =      { "ClassName", "callback1",             "string",      "struct1"                       };
        //    var elements = new Element[inputNames.Length];

        //    // Act
        //    var results = elements.Zip(inputNames, (element, inputName) => new Class(inputName)).ToArray();

        //    // Assert
        //    for (int i = 0; i < inputNames.Length; i++)
        //    {
        //        Assert.Equal(namespaces[i], results[i].Namespace);
        //        Assert.Equal(names[i], results[i].Name);
        //        Assert.Equal(inputNames[i], results[i].QualifiedName);
        //    }

        //}

        //[Fact]
        //public void Test_Alias()
        //{
        //    // Arrange
        //    string[] inputNames = { "ClassName", "my_library::callback1", "std::string", "my_library::struct1<T1,T2,T3>" };
        //    string[] namespaces = { "",          "my_library",            "std",         "my_library" };
        //    string[] names =      { "ClassName", "callback1",             "string",      "struct1" };
        //    var elements = new Element[inputNames.Length];

        //    // Act
        //    var inputType = new Class(inputNames[2]);
        //    var outputType = new Class(inputNames[0]);
        //    var result = new Alias(inputNames[1], inputNames[0], inputNames[2]);

        //    // Assert
        //    Assert.Equal(result.QualifiedName, inputNames[1]);
        //    Assert.Equal(result.Nested[0].QualifiedName, outputType.QualifiedName);
        //    Assert.Equal(result.Nested[1].QualifiedName, inputType.QualifiedName);
        //    Assert.Equal(result.Nested[1].Namespace, inputType.Namespace);
        //    Assert.Throws<FormatException>(() => new Alias(inputNames[1], inputNames[0], " "));
        //}

        //[Fact]
        //public void Test_Repo_GetType()
        //{
        //    // Arrange
        //    string[] inputNames = { "ClassName", "my_library::callback1", "std::string", "my_library::struct1<T1,T2,T3>" };
        //    var elements = new Element[inputNames.Length];

        //    Repository Repository = new Repository();
        //    Repository.AvailableTypes["<string>"].Add(new Class("std::string"));
        //    Repository.AvailableTypes["\"my_class.h\""].Add(new Class("my_library::my_class"));
        //    Repository.AvailableTypes["\"my_library.h\""].Add(new Alias("my_library::callback", "my_library::my_class", "std::string"));
        //    Repository.AvailableTypes["\"my_library.h\""].Add(new Struct("my_library::struct1<T1,T2,T3>"));

        //    // Act
        //    //var element = Repository.GetType("std::string");
        //    elements = elements.Zip(inputNames, (element, inputName) => Repository.GetElement(inputName)).ToArray();


        //    // Assert
        //    Assert.True(elements[0] == null);
        //    Assert.True(elements[1] == null);
        //    Assert.Equal(elements[2], new Class("std::string"));
        //    Assert.Equal(elements[3], new Struct("my_library::struct1<T1,T2,T3>"));
        //    Assert.Throws<FormatException>(() => Repository.GetElement(" "));
        //}

        //[Fact]
        //public void Test_Repo_GetFilename()
        //{
        //    // Arrange
        //    string[] inputNames = { "ClassName", "my_library::callback1", "std::string", "my_library::struct1<T1,T2,T3>" };
        //    var fileNames = new string[inputNames.Length];

        //    Repository Repository = new Repository();
        //    Repository.AvailableTypes["<string>"].Add(new Class("std::string"));
        //    Repository.AvailableTypes["\"my_class.h\""].Add(new Class("my_library::my_class"));
        //    Repository.AvailableTypes["\"my_library.h\""].Add(new Alias("my_library::callback", "my_library::my_class", "std::string"));
        //    Repository.AvailableTypes["\"my_library.h\""].Add(new Struct("my_library::struct1<T1,T2,T3>"));

        //    // Act
        //    fileNames = fileNames.Zip(inputNames, (_, inputName) => Repository.GetFilename(inputName)).ToArray();


        //    // Assert
        //    Assert.True(fileNames[0] == null);
        //    Assert.True(fileNames[1] == null);
        //    Assert.Equal("<string>", fileNames[2]);
        //    Assert.Equal("\"my_library.h\"", fileNames[3]);
        //    Assert.Throws<FormatException>(() =>  Repository.GetFilename(" "));
        //}

        //[Fact]
        //public void Test_Repo_GetFilenameType()
        //{
        //    // Arrange
        //    string[] inputNames = { "ClassName", "my_library::callback1", "std::string", "my_library::struct1<T1,T2,T3>" };

        //    Repository Repository = new Repository();
        //    Repository.AvailableTypes["<string>"].Add(new Class("std::string"));
        //    Repository.AvailableTypes["\"my_class.h\""].Add(new Class("my_library::my_class"));
        //    Repository.AvailableTypes["\"my_library.h\""].Add(new Alias("my_library::callback", "my_library::my_class", "std::string"));
        //    Repository.AvailableTypes["\"my_library.h\""].Add(new Struct("my_library::struct1<T1,T2,T3>"));

        //    // Act
        //    Element element1 = Repository.GetFilenameElement("<string>", "std::string");
        //    Element element2 = Repository.GetFilenameElement("<string>", "std::string1");

        //    // Assert
        //    Assert.Equal("std::string", element1.QualifiedName);
        //    Assert.Null(element2);
        //    Assert.Throws<NullReferenceException>(() => Repository.GetFilenameElement("<string>1", "std::string"));
        //}

        //[Fact]
        //public void Test_Repo_Indexer()
        //{
        //    // Arrange
        //    string[] inputNames = { "ClassName", "my_library::callback1", "std::string", "my_library::struct1<T1,T2,T3>" };
        //    var fileNames = new string[inputNames.Length];
        //    var elements = new Element[inputNames.Length];
        //    int filesDictionaryIndexCounter = 0;
        //    int availableTypesCounter = 0;

        //    var FilesDictionaryFake = new Mock<FilesDictionary>() { CallBase = true };
        //    FilesDictionaryFake.Setup(r => r[It.IsAny<string>()]).Callback(() => filesDictionaryIndexCounter++);

        //    var RepositoryFake = new Mock<Repository>() { CallBase = true };
        //    RepositoryFake.SetupGet(r => r.AvailableTypes).Returns(FilesDictionaryFake.Object).Callback(() => availableTypesCounter++);

        //    Repository Repository = RepositoryFake.Object;
        //    Repository.AvailableTypes["<string>"].Add(new Class("std::string"));
        //    Repository.AvailableTypes["\"my_class.h\""].Add(new Class("my_library::my_class"));
        //    Repository.AvailableTypes["\"my_library.h\""].Add(new Alias("my_library::callback", "my_library::my_class", "std::string"));
        //    Repository.AvailableTypes["\"my_library.h\""].Add(new Struct("my_library::struct1<T1,T2,T3>"));

        //    // Act
        //    fileNames = fileNames.Zip(inputNames, (_, inputName) => Repository.GetFilename(inputName)).ToArray();
        //    elements = elements.Zip(inputNames, (_, inputName) => Repository.GetElement(inputName)).ToArray();

        //    // Assert
        //    Assert.True(fileNames[0] == null);
        //    Assert.True(fileNames[1] == null);
        //    Assert.Equal("<string>", fileNames[2]);
        //    Assert.Equal("\"my_library.h\"", fileNames[3]);
        //    Assert.Throws<FormatException>(() => Repository.GetFilename(" "));
        //    Assert.True(filesDictionaryIndexCounter == inputNames.Length);
        //    Assert.True(availableTypesCounter == inputNames.Length * 3);

        //}
    }
}
