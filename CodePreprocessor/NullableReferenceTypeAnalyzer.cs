using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodePreprocessor
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NullableReferenceTypeAnalyzer : DiagnosticAnalyzer
    {
        #region Fields

        public const string DiagnosticId = "NullableReference";
        public static readonly (string Start, string End) NullableSyntaxMarker = ("/*Start:nullableref*/", "/*End:nullableref*/");

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, "RuleTitle", "Found non-tagged {1}: {0}", "NullableUsage", DiagnosticSeverity.Info, true, "Nullable syntax rule.");

        #endregion

        #region Properties

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        #endregion

        #region  Public Methods

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);

            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.NullableType, SyntaxKind.TypeConstraint, SyntaxKind.NullableDirectiveTrivia);
        }

        #endregion

        #region Private Methods

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            switch (context.Node.Kind())
            {
                case SyntaxKind.NullableType:

                    var nullableTypeNode = (NullableTypeSyntax)context.Node;

                    TypeInfo semantics = context.SemanticModel.GetTypeInfo(nullableTypeNode);

                    if (!semantics.Type.IsValueType && !HasNullableRefMarker(nullableTypeNode))
                    {
                        Diagnostic diagnostic = Diagnostic.Create(Rule, nullableTypeNode.GetLocation(), nullableTypeNode.GetText(), "reference type");

                        context.ReportDiagnostic(diagnostic);
                    }

                    break;
                case SyntaxKind.TypeConstraint:
                    var constraintNode = (TypeConstraintSyntax)context.Node;
                    if (HasNullableRefMarker((TypeParameterConstraintClauseSyntax)constraintNode.Parent))
                        break;

                    foreach (IdentifierNameSyntax it in constraintNode.ChildNodes().OfType<IdentifierNameSyntax>().Where(it => it.Identifier.ValueText == "notnull"))
                    {
                        if (HasNullableRefMarker(it))
                            continue;

                        Diagnostic diagnostic = Diagnostic.Create(Rule, it.GetLocation(), it.Identifier.ValueText, "keyword");

                        context.ReportDiagnostic(diagnostic);
                    }

                    break;
                case SyntaxKind.NullableDirectiveTrivia:
                    var nullableDirective = (NullableDirectiveTriviaSyntax)context.Node;

                    if (HasNullableRefMarker(nullableDirective))
                        break;

                    Diagnostic diagnostic2 = Diagnostic.Create(Rule, context.Node.GetLocation(), nullableDirective.GetText(), "directive");

                    context.ReportDiagnostic(diagnostic2);
                    break;
            }
        }

        private static bool HasNullableRefMarker(NullableDirectiveTriviaSyntax nullableDirective)
        {
            return nullableDirective.GetLeadingTrivia().Any(it => it.IsKind(SyntaxKind.MultiLineCommentTrivia) && it.ToFullString() == NullableSyntaxMarker.Start)
                   && nullableDirective.GetTrailingTrivia().Any(it => it.IsKind(SyntaxKind.MultiLineCommentTrivia) && it.ToFullString() == NullableSyntaxMarker.End);
        }
        private static bool HasNullableRefMarker(IdentifierNameSyntax notNullIdentifier)
        {
            SyntaxNode constraintClause = notNullIdentifier.Parent.Parent;

            string test = constraintClause.ToFullString();

            SyntaxTrivia leadingTrivia = constraintClause.FindTrivia(notNullIdentifier.SpanStart - 1);

            if (!leadingTrivia.IsKind(SyntaxKind.MultiLineCommentTrivia) || leadingTrivia.ToFullString() != NullableSyntaxMarker.Start)
                return false;

            SyntaxToken commaToken = constraintClause.ChildTokens()
                                                     .First(it => it.GetLocation().SourceSpan.Start > notNullIdentifier.GetLocation().SourceSpan.Start
                                                                  && it.IsKind(SyntaxKind.CommaToken));

            SyntaxTrivia firstTrailingTrv = commaToken.TrailingTrivia.FirstOrDefault();

            return firstTrailingTrv.IsKind(SyntaxKind.MultiLineCommentTrivia) && firstTrailingTrv.ToFullString() == NullableSyntaxMarker.End;
        }

        private static bool HasNullableRefMarker(TypeParameterConstraintClauseSyntax typeParameterConstraint)
        {
            SyntaxTrivia leadingTrivia = typeParameterConstraint.Parent.FindTrivia(typeParameterConstraint.SpanStart - 1);
            if (!leadingTrivia.IsKind(SyntaxKind.MultiLineCommentTrivia) || leadingTrivia.ToFullString() != NullableSyntaxMarker.Start)
                return false;

            List<TypeConstraintSyntax> typeConstraintSyntaxes = typeParameterConstraint.ChildNodes().OfType<TypeConstraintSyntax>().ToList();

            if (typeConstraintSyntaxes.Count != 1)
                return false;

            SyntaxTrivia firstTrailingTrv = typeConstraintSyntaxes[0].GetTrailingTrivia().FirstOrDefault();

            return firstTrailingTrv.IsKind(SyntaxKind.MultiLineCommentTrivia) && firstTrailingTrv.ToFullString() == NullableSyntaxMarker.End;
        }

        private static bool HasNullableRefMarker(NullableTypeSyntax nullableTypeNode)
        {
            SyntaxToken questionMarkToken = nullableTypeNode.ChildTokens().First(it => it.IsKind(SyntaxKind.QuestionToken));

            SyntaxTriviaList trailingTrivia = questionMarkToken.TrailingTrivia;
            SyntaxTrivia leadingTrivia = nullableTypeNode.FindTrivia(questionMarkToken.SpanStart - 1);

            if (trailingTrivia.Count == 0 || !leadingTrivia.IsKind(SyntaxKind.MultiLineCommentTrivia))
                return false;

            SyntaxTrivia firstTrailingTrivia = trailingTrivia.First();

            bool result = firstTrailingTrivia.Kind() == SyntaxKind.MultiLineCommentTrivia
                          && firstTrailingTrivia.ToFullString() == NullableSyntaxMarker.End
                          && leadingTrivia.ToFullString() == NullableSyntaxMarker.Start;

            return result;
        }

        #endregion
    }
}