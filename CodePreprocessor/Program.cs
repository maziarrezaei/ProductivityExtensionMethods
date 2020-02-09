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
            string code;

            if (args.Length == 0)
            {
                code = new StreamReader(Console.OpenStandardInput()).ReadToEnd();
            }
            else
            {
                string path = args[0];

                if (!File.Exists(path))
                {
                    Console.Error.Write("File not found: {0}", path);
                    return 2;
                }

                code = File.ReadAllText(path);
            }

            // Commenting out conditional compile blocks, so they will be processed as well regardless of which symbol is defined.
            var ifPreprocessorRegX = new Regex("^#(if|elif|endif|else)", RegexOptions.Multiline | RegexOptions.ExplicitCapture);

            code = ifPreprocessorRegX.Replace(code, uniqueComment + @"$0");

            string processedCode = AnalyzerRunner.RunCodeFix("C#", new NullableReferenceTypeAnalyzer(), new NullableReferenceTypeCodeFixProvider(), code, true);

            Console.Write(processedCode.Replace(uniqueComment, ""));

            return 0;
        }
    }
}
