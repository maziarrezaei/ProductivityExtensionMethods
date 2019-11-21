using System;
using System.IO;

namespace CodePreprocessor
{
    class Program
    {
        static int Main(string[] args)
        {
            if(args.Length==0)
            {
                Console.Error.Write("Tool must be supplied with the path to a .cs code file.");
                return 1;
            }
            string path = args[0];

            if(!File.Exists(path))
            {
                Console.Error.Write("File not found: {0}",path);
                return 2;
            }

            string code = File.ReadAllText(path);

            string processedCode = AnalyzerRunner.RunCodeFix("C#", new NullableReferenceTypeAnalyzer(), new NullableReferenceTypeCodeFixProvider(), code, true);

            Console.Write(processedCode);

            return 0;
        }
    }
}
