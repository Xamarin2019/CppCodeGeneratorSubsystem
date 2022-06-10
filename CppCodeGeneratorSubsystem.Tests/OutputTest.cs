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

        }
    }
}