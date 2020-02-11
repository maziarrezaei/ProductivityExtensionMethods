using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace CodePreprocessor
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NullableReferenceTypeCodeFixProvider))]
    [Shared]
    public class NullableReferenceTypeCodeFixProvider : CodeFixProvider
    {
        #region Fields

        private const string title = "Add /*nullableref*/ in around nullable syntaxs";

        #endregion

        #region Properties

        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(NullableReferenceTypeAnalyzer.DiagnosticId);

        #endregion

        #region  Public Methods

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            // TODO: Replace the following code with your own analysis, generating a CodeAction for each fix to suggest
            Diagnostic diagnostic = context.Diagnostics.First();
            TextSpan diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            IEnumerable<SyntaxNode> ancestorsAndSelf = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf();

            NullableTypeSyntax nullableTypeSyntax = ancestorsAndSelf.OfType<NullableTypeSyntax>().FirstOrDefault();
            TypeConstraintSyntax typeConstraint = ancestorsAndSelf.OfType<TypeConstraintSyntax>().FirstOrDefault();

            NullableDirectiveTriviaSyntax nullableDirective = root.FindTrivia(diagnosticSpan.Start).GetStructure() as NullableDirectiveTriviaSyntax;
            PostfixUnaryExpressionSyntax nullableSuppressExpression = ancestorsAndSelf.OfType<PostfixUnaryExpressionSyntax>().FirstOrDefault();

            if (nullableTypeSyntax == null && typeConstraint == null && nullableDirective == null && nullableSuppressExpression == null)
                return;

            // Register a code action that will invoke the fix.
            Func<CancellationToken, Task<Document>> fix;

            if (nullableTypeSyntax != null)
                fix = c => AddNullableComment(context.Document, nullableTypeSyntax, c);
            else if (typeConstraint != null)
                fix = c => AddNullableComment(context.Document, typeConstraint, c);
            else if (nullableDirective != null)
                fix = c => AddNullableComment(context.Document, nullableDirective, c);
            else
                fix = c => AddNullableComment(context.Document, nullableSuppressExpression, c);

            CodeAction codeAction = CodeAction.Create(title, fix, title);

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        #endregion

        #region Private Methods

        private async Task<Document> AddNullableComment(Document document, PostfixUnaryExpressionSyntax nullableSuppressExpression, CancellationToken cancellationToken)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync().ConfigureAwait(false);

            SyntaxToken oldOperator = nullableSuppressExpression.OperatorToken;
            var newOperator = oldOperator.WithLeadingTrivia(SyntaxFactory.Comment(NullableReferenceTypeAnalyzer.NullableSyntaxMarker.Start))
                                         .WithTrailingTrivia(SyntaxFactory.Comment(NullableReferenceTypeAnalyzer.NullableSyntaxMarker.End));

            return document.WithSyntaxRoot(root.ReplaceToken(oldOperator, newOperator));
        }
        private async Task<Document> AddNullableComment(Document document, NullableDirectiveTriviaSyntax nullableDirectiveTrivia, CancellationToken cancellationToken)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync().ConfigureAwait(false);

            SyntaxTrivia endOfLine = SyntaxFactory.EndOfLine("\r\n");

            var newDirective = nullableDirectiveTrivia.WithLeadingTrivia(SyntaxFactory.Comment(NullableReferenceTypeAnalyzer.NullableSyntaxMarker.Start), endOfLine)
                                                      .WithTrailingTrivia(endOfLine, SyntaxFactory.Comment(NullableReferenceTypeAnalyzer.NullableSyntaxMarker.End), endOfLine);
            return document.WithSyntaxRoot(root.ReplaceNode(nullableDirectiveTrivia, newDirective));
        }
        private async Task<Document> AddNullableComment(Document document, TypeConstraintSyntax typeConstraintNode, CancellationToken cancellationToken)
        {
            var constraintClause = (TypeParameterConstraintClauseSyntax)typeConstraintNode.Parent;

            TypeConstraintSyntax[] otherTypeConstraints = constraintClause.ChildNodes().OfType<TypeConstraintSyntax>().Except(new[] { typeConstraintNode }).ToArray();

            SyntaxNode root = await document.GetSyntaxRootAsync().ConfigureAwait(false);

            IdentifierNameSyntax idNameSyntax = constraintClause.ChildNodes().OfType<IdentifierNameSyntax>().First();

            SyntaxToken identifierToken = typeConstraintNode.ChildNodes().First().ChildTokens().First();

            TypeParameterConstraintClauseSyntax newConstraintClause;

            if (otherTypeConstraints.Length > 0)
            {
                // Only this type constraint must be removed, since the where clause contains others

                // Adding the starting comment before the type constraint
                SyntaxToken newIdentifierToken = SyntaxFactory.Identifier(identifierToken.LeadingTrivia.Add(SyntaxFactory.Comment(NullableReferenceTypeAnalyzer.NullableSyntaxMarker.Start)),
                                                                          SyntaxKind.IdentifierName, identifierToken.Text, identifierToken.ValueText,
                                                                          SyntaxTriviaList.Empty);

                TypeConstraintSyntax newNode = typeConstraintNode.ReplaceToken(identifierToken, newIdentifierToken);

                // Moving the new type constraint to the first of list
                SeparatedSyntaxList<TypeParameterConstraintSyntax> newConstraintList = SyntaxFactory.SeparatedList<TypeParameterConstraintSyntax>(otherTypeConstraints.Prepend(newNode));
                newConstraintClause = SyntaxFactory.TypeParameterConstraintClause(idNameSyntax.WithLeadingTrivia(SyntaxFactory.Space)).WithConstraints(newConstraintList);

                // Adding the end type constraint after the first comma.
                SyntaxToken firstCommaToken = newConstraintClause.ChildTokens().First(token => token.IsKind(SyntaxKind.CommaToken));
                SyntaxToken newCommaToken = SyntaxFactory.Token(firstCommaToken.LeadingTrivia,
                                                                SyntaxKind.CommaToken,
                                                                firstCommaToken.TrailingTrivia.Insert(0, SyntaxFactory.Comment(NullableReferenceTypeAnalyzer.NullableSyntaxMarker.End)));

                newConstraintClause = newConstraintClause.ReplaceToken(firstCommaToken, newCommaToken);
            }
            else
            {
                // the whole where clause needs to be removed, so adding the comments before and after it.

                SyntaxToken oldWhereKeyword = constraintClause.ChildTokens().First(it => it.IsKind(SyntaxKind.WhereKeyword));

                SyntaxToken newWhereKeyword = SyntaxFactory.Token(oldWhereKeyword.LeadingTrivia.Add(SyntaxFactory.Comment(NullableReferenceTypeAnalyzer.NullableSyntaxMarker.Start)),
                                                                  SyntaxKind.WhereKeyword,
                                                                  oldWhereKeyword.TrailingTrivia);

                SyntaxToken newIdentifierToken = SyntaxFactory.Identifier(identifierToken.LeadingTrivia, SyntaxKind.IdentifierName,
                                                                          identifierToken.Text, identifierToken.ValueText,
                                                                          identifierToken.TrailingTrivia.Insert(0, SyntaxFactory.Comment(NullableReferenceTypeAnalyzer.NullableSyntaxMarker.End)));

                TypeConstraintSyntax newNode = typeConstraintNode.ReplaceToken(identifierToken, newIdentifierToken);

                newConstraintClause = SyntaxFactory.TypeParameterConstraintClause(newWhereKeyword,
                                                                                  constraintClause.ChildNodes().OfType<IdentifierNameSyntax>().First(),
                                                                                  constraintClause.ChildTokens().First(it => it.IsKind(SyntaxKind.ColonToken)),
                                                                                  SyntaxFactory.SeparatedList<TypeParameterConstraintSyntax>(new[] { newNode }));
            }

            // for some reason Roslyn adds formatting annotations that results in some strange new lines being added
            // by formatter later in internal CleanupDocumentAsync method of CodeAction class.
            newConstraintClause = RemoveAnnotationFromDescendantTrivias(newConstraintClause, SyntaxAnnotation.ElasticAnnotation);

            SyntaxNode newRoot = root.ReplaceNode(constraintClause, newConstraintClause);

            Document result = document.WithSyntaxRoot(newRoot);

            return result;
        }

        private async Task<Document> AddNullableComment(Document document, NullableTypeSyntax syntaxNode, CancellationToken cancellationToken)
        {
            SyntaxToken questionToken = syntaxNode.ChildTokens().First(it => it.IsKind(SyntaxKind.QuestionToken));

            SyntaxToken newQuestionToken = SyntaxFactory.Token(questionToken.LeadingTrivia.Add(SyntaxFactory.Comment(NullableReferenceTypeAnalyzer.NullableSyntaxMarker.Start)),
                                                               SyntaxKind.QuestionToken,
                                                               questionToken.TrailingTrivia.Insert(0, SyntaxFactory.Comment(NullableReferenceTypeAnalyzer.NullableSyntaxMarker.End)));

            SyntaxNode root = await document.GetSyntaxRootAsync().ConfigureAwait(false);

            SyntaxNode newRoot = root.ReplaceToken(questionToken, newQuestionToken);

            return document.WithSyntaxRoot(newRoot);
        }

        private static T RemoveAnnotationFromDescendantTrivias<T>(T node, SyntaxAnnotation annotation) where T : SyntaxNode
        {
            IEnumerable<SyntaxTrivia> annotatedTrivias = node.GetAnnotatedTrivia(annotation);

            return node.ReplaceTrivia(annotatedTrivias, (t, trivia) => trivia.WithoutAnnotations(annotation));
        }

        #endregion
    }
}