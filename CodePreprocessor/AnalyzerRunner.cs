using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Text;

namespace CodePreprocessor
{
    /// <summary>
    ///     Class for turning strings into documents and getting the diagnostics on them
    ///     All methods are static
    /// </summary>
    public static class AnalyzerRunner
    {
        #region Fields

        private static readonly MetadataReference CodeAnalysisReference = MetadataReference.CreateFromFile(typeof(Compilation).Assembly.Location);
        private static readonly MetadataReference CorlibReference = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
        private static readonly MetadataReference CSharpSymbolsReference = MetadataReference.CreateFromFile(typeof(CSharpCompilation).Assembly.Location);
        private static readonly MetadataReference SystemCoreReference = MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location);

        internal static string CSharpDefaultFileExt = "cs";
        internal static string DefaultFilePathPrefix = "Test";
        internal static string TestProjectName = "TestProject";
        internal static string VisualBasicDefaultExt = "vb";

        #endregion

        #region  Public Methods

        /// <summary>
        ///     Given classes in the form of strings, their language, and an IDiagnosticAnalyzer to apply to it, return the
        ///     diagnostics found in the string after converting it to a document.
        /// </summary>
        /// <param name="sources">Classes in the form of strings</param>
        /// <param name="language">The language the source classes are in</param>
        /// <param name="analyzer">The analyzer to be run on the sources</param>
        /// <returns>An IEnumerable of Diagnostics that surfaced in the source code, sorted by Location</returns>
        public static Diagnostic[] GetSortedDiagnostics(string[] sources, string language, DiagnosticAnalyzer analyzer)
        {
            if (language != LanguageNames.CSharp && language != LanguageNames.VisualBasic)
                throw new ArgumentException("Unsupported Language");

            Project project = CreateProject(sources, language);
            Document[] documents = project.Documents.ToArray();

            if (sources.Length != documents.Length)
                throw new InvalidOperationException("Amount of sources did not match amount of Documents created");

            return GetSortedDiagnosticsFromDocuments(analyzer, documents);
        }

        /// <summary>
        ///     General verifier for codefixes.
        ///     Creates a Document from the source string, then gets diagnostics on it and applies the relevant codefixes.
        ///     Then gets the string after the codefix is applied and compares it with the expected result.
        ///     Note: If any codefix causes new diagnostics to show up, the test fails unless allowNewCompilerDiagnostics is set to
        ///     true.
        /// </summary>
        /// <param name="language">The language the source code is in</param>
        /// <param name="analyzer">The analyzer to be applied to the source code</param>
        /// <param name="codeFixProvider">The codefix to be applied to the code wherever the relevant Diagnostic is found</param>
        /// <param name="oldSource">A class in the form of a string before the CodeFix was applied to it</param>
        /// <param name="allowNewCompilerDiagnostics">
        ///     A bool controlling whether or not the test will fail if the CodeFix
        ///     introduces other warnings after being applied
        /// </param>
        public static string RunCodeFix(string language, DiagnosticAnalyzer analyzer, CodeFixProvider codeFixProvider, string oldSource, bool allowNewCompilerDiagnostics)
        {
            Document document = CreateProject(new[] { oldSource }, language).Documents.First();
            Diagnostic[] analyzerDiagnostics = GetSortedDiagnosticsFromDocuments(analyzer, new[] { document });
            IEnumerable<Diagnostic> compilerDiagnostics = GetCompilerDiagnostics(document);

            while (analyzerDiagnostics.Length > 0)
            {
                var actions = new List<CodeAction>();

                for (int i = 0; i < analyzerDiagnostics.Length && actions.Count == 0; i++)
                {
                    if (!codeFixProvider.FixableDiagnosticIds.Contains(analyzerDiagnostics[i].Id))
                        continue;

                    var context = new CodeFixContext(document, analyzerDiagnostics[i], (action, diagnostics) => actions.Add(action), CancellationToken.None);
                    codeFixProvider.RegisterCodeFixesAsync(context).Wait();
                }

                if (actions.Count == 0)
                    break;

                document = ApplyFix(document, actions[0]);

                analyzerDiagnostics = GetSortedDiagnosticsFromDocuments(analyzer, new[] { document });

                IEnumerable<Diagnostic> newCompilerDiagnostics = GetNewDiagnostics(compilerDiagnostics, GetCompilerDiagnostics(document));

                //check if applying the code fix introduced any new compiler diagnostics
                if (!allowNewCompilerDiagnostics && newCompilerDiagnostics.Any())
                {
                    // Format and get the compiler diagnostics again so that the locations make sense in the output
                    document = document.WithSyntaxRoot(Formatter.Format(document.GetSyntaxRootAsync().Result, Formatter.Annotation, document.Project.Solution.Workspace));
                    newCompilerDiagnostics = GetNewDiagnostics(compilerDiagnostics, GetCompilerDiagnostics(document));

                    throw new ApplicationException(string.Format("Fix introduced new compiler diagnostics:\r\n{0}\r\n\r\nNew document:\r\n{1}\r\n",
                                                                 string.Join("\r\n", newCompilerDiagnostics.Select(d => d.ToString())), document.GetSyntaxRootAsync().Result.ToFullString()));
                }
            }

            //after applying all of the code fixes, compare the resulting string to the inputted one
            return document.GetSyntaxRootAsync().Result.GetText().ToString();
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Given an analyzer and a document to apply it to, run the analyzer and gather an array of diagnostics found in it.
        ///     The returned diagnostics are then ordered by location in the source document.
        /// </summary>
        /// <param name="analyzer">The analyzer to run on the documents</param>
        /// <param name="documents">The Documents that the analyzer will be run on</param>
        /// <returns>An IEnumerable of Diagnostics that surfaced in the source code, sorted by Location</returns>
        private static Diagnostic[] GetSortedDiagnosticsFromDocuments(DiagnosticAnalyzer analyzer, Document[] documents)
        {
            var projects = new HashSet<Project>();
            foreach (Document document in documents)
                projects.Add(document.Project);

            var diagnostics = new List<Diagnostic>();

            foreach (Project project in projects)
            {
                CompilationWithAnalyzers compilationWithAnalyzers = project.GetCompilationAsync().Result.WithAnalyzers(ImmutableArray.Create(analyzer));
                ImmutableArray<Diagnostic> diags = compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync().Result;

                foreach (Diagnostic diag in diags)
                {
                    if (diag.Location == Location.None || diag.Location.IsInMetadata)
                        diagnostics.Add(diag);
                    else
                        for (int i = 0; i < documents.Length; i++)
                        {
                            Document document = documents[i];
                            SyntaxTree tree = document.GetSyntaxTreeAsync().Result;
                            if (tree == diag.Location.SourceTree)
                                diagnostics.Add(diag);
                        }
                }
            }

            Diagnostic[] results = diagnostics.OrderBy(d => d.Location.SourceSpan.Start).ToArray();
            diagnostics.Clear();
            return results;
        }

        /// <summary>
        ///     Create a project using the inputted strings as sources.
        /// </summary>
        /// <param name="sources">Classes in the form of strings</param>
        /// <param name="language">The language the source code is in</param>
        /// <returns>A Project created out of the Documents created from the source strings</returns>
        private static Project CreateProject(string[] sources, string language = LanguageNames.CSharp)
        {
            string fileNamePrefix = DefaultFilePathPrefix;
            string fileExt = language == LanguageNames.CSharp ? CSharpDefaultFileExt : VisualBasicDefaultExt;

            ProjectId projectId = ProjectId.CreateNewId(TestProjectName);

            Solution solution = new AdhocWorkspace()
                               .CurrentSolution
                               .AddProject(projectId, TestProjectName, TestProjectName, language)
                               .AddMetadataReference(projectId, CorlibReference)
                               .AddMetadataReference(projectId, SystemCoreReference)
                               .AddMetadataReference(projectId, CSharpSymbolsReference)
                               .AddMetadataReference(projectId, CodeAnalysisReference);

            int count = 0;

            foreach (string source in sources)
            {
                string newFileName = fileNamePrefix + count + "." + fileExt;
                DocumentId documentId = DocumentId.CreateNewId(projectId, newFileName);
                solution = solution.AddDocument(documentId, newFileName, SourceText.From(source));
                count++;
            }

            return solution.GetProject(projectId);
        }

        /// <summary>
        ///     Get the existing compiler diagnostics on the inputted document.
        /// </summary>
        /// <param name="document">The Document to run the compiler diagnostic analyzers on</param>
        /// <returns>The compiler diagnostics that were found in the code</returns>
        private static IEnumerable<Diagnostic> GetCompilerDiagnostics(Document document)
        {
            return document.GetSemanticModelAsync().Result.GetDiagnostics();
        }

        /// <summary>
        ///     Apply the inputted CodeAction to the inputted document.
        ///     Meant to be used to apply codefixes.
        /// </summary>
        /// <param name="document">The Document to apply the fix on</param>
        /// <param name="codeAction">A CodeAction that will be applied to the Document.</param>
        /// <returns>A Document with the changes from the CodeAction</returns>
        private static Document ApplyFix(Document document, CodeAction codeAction)
        {
            ImmutableArray<CodeActionOperation> operations = codeAction.GetOperationsAsync(CancellationToken.None).Result;
            Solution solution = operations.OfType<ApplyChangesOperation>().Single().ChangedSolution;
            return solution.GetDocument(document.Id);
        }

        /// <summary>
        ///     Compare two collections of Diagnostics,and return a list of any new diagnostics that appear only in the second
        ///     collection.
        ///     Note: Considers Diagnostics to be the same if they have the same Ids.  In the case of multiple diagnostics with the
        ///     same Id in a row,
        ///     this method may not necessarily return the new one.
        /// </summary>
        /// <param name="diagnostics">The Diagnostics that existed in the code before the CodeFix was applied</param>
        /// <param name="newDiagnostics">The Diagnostics that exist in the code after the CodeFix was applied</param>
        /// <returns>A list of Diagnostics that only surfaced in the code after the CodeFix was applied</returns>
        private static IEnumerable<Diagnostic> GetNewDiagnostics(IEnumerable<Diagnostic> diagnostics, IEnumerable<Diagnostic> newDiagnostics)
        {
            Diagnostic[] oldArray = diagnostics.OrderBy(d => d.Location.SourceSpan.Start).ToArray();
            Diagnostic[] newArray = newDiagnostics.OrderBy(d => d.Location.SourceSpan.Start).ToArray();

            int oldIndex = 0;
            int newIndex = 0;

            while (newIndex < newArray.Length)
            {
                if (oldIndex < oldArray.Length && oldArray[oldIndex].Id == newArray[newIndex].Id)
                {
                    ++oldIndex;
                    ++newIndex;
                }
                else
                {
                    yield return newArray[newIndex++];
                }
            }
        }

        #endregion
    }
}