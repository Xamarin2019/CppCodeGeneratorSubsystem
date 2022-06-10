using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit.Abstractions;

namespace CppCodeGeneratorSubsystem.Tests
{
    public class RedirectConsoleOutput : IDisposable
    {
        private readonly ITestOutputHelper output;
        private readonly TextWriter originalOut;
        protected readonly TextWriter textWriter;

        public RedirectConsoleOutput(ITestOutputHelper output)
        {
            this.output = output;
            originalOut = Console.Out;
            textWriter = new StringWriter();
            Console.SetOut(textWriter);
        }

        public void Dispose()
        {
            output.WriteLine(textWriter.ToString());
            Console.SetOut(originalOut);
        }
    }
}