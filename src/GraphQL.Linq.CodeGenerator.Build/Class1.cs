using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.IO;
using System.Linq;

namespace GraphQL.Linq.CodeGenerator.Build
{
    public static class Program
    {
        public static void Main()
        {
            var filename = @"C:\Projects\external\graphql-linq\src\GraphQL.Linq.Sample\Class1.cs";
            var source = File.ReadAllText(filename);

            var result = SyntaxFactory.ParseCompilationUnit(source);

            //foreach (var n in result.DescendantNodes().OfType< QueryExpressionSyntax>())
            //{
            //    Console.WriteLine(n);
            //}

            var visitor = new GraphQLVisitor();

            visitor.Visit(result);

            Console.WriteLine(result);
        }
    }

    class GraphQLVisitor: CSharpSyntaxVisitor
    {
        public override void DefaultVisit(SyntaxNode node)
        {
            foreach (var child in node.ChildNodes())
                Visit(child);
        }

        public override void VisitQueryExpression(QueryExpressionSyntax node)
        {
            Console.WriteLine();
            Console.WriteLine(node);
            Console.WriteLine();

            base.VisitQueryExpression(node);
        }
    }
}
