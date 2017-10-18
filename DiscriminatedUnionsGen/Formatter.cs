using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Formatting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Options;

namespace DiscriminatedUnionsGen
{
    public static class CodeFormatter
    {
        public static string Format(string code)
        {
            var workspace = new AdhocWorkspace();

            OptionSet options = workspace.Options;
            options = options.WithChangedOption(CSharpFormattingOptions.NewLinesForBracesInMethods, true);
            options = options.WithChangedOption(CSharpFormattingOptions.NewLinesForBracesInProperties, true);

            var parsed = CSharpSyntaxTree.ParseText(code).GetRoot();

            SyntaxNode formattedNode = Formatter.Format(parsed, workspace, options);

            var sb = new StringBuilder();

            using (var writer = new StringWriter(sb))
            {
                formattedNode.WriteTo(writer);
            }

            return sb.ToString();
        }
    }
}
