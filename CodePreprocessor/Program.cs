using System;
using System.IO;
using System.Text.RegularExpressions;

namespace CodePreprocessor
{
    class Program
    {
        static int Main(string[] args)
        {
            const string uniqueComment = "//asdfjwieowilsdkvmmweiowjaX";

            if (args.Length == 0)
            {
                Console.Error.Write("Tool must be supplied with the path to a .cs code file.");
                return 1;
            }
            string path = args[0];

            if (!File.Exists(path))
            {
                Console.Error.Write("File not found: {0}", path);
                return 2;
            }

            string code = File.ReadAllText(path);

            var ifPreprocessorRegX = new Regex("^#(if|elif|endif|else)",RegexOptions.Multiline|RegexOptions.ExplicitCapture);

            code = ifPreprocessorRegX.Replace(code, uniqueComment + @"$0");

            string processedCode = AnalyzerRunner.RunCodeFix("C#", new NullableReferenceTypeAnalyzer(), new NullableReferenceTypeCodeFixProvider(), code, true);

            Console.Write(processedCode.Replace(uniqueComment, ""));

            return 0;
        }
    }
}
