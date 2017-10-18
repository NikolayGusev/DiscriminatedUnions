using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiscriminatedUnionsGen
{
    public class RoslynAnalyzer
    {
        Type _ = typeof(Microsoft.CodeAnalysis.CSharp.Formatting.CSharpFormattingOptions);

        public static List<UnionInfo> ReadUnions(string projectPath)
        {
            var compilation = MSBuildWorkspace.Create().OpenProjectAsync(projectPath).Result.GetCompilationAsync().Result;
            var trees = compilation.SyntaxTrees
                .Cast<CSharpSyntaxTree>()
                .Where(x => x.FilePath.EndsWith("designer.cs") == false);

            List<Class> classes = new List<Class>();


            foreach (var tree in trees)
            {
                var model = compilation.GetSemanticModel(tree);
                var c = 
                    tree
                    .GetRoot()
                    .DescendantNodes().OfType<ClassDeclarationSyntax>()
                    .Select(x => new Class(x, model.GetDeclaredSymbol(x), model)).ToList();
                classes.AddRange(c);
            }

            var unions = classes.Where(IsUnionBase);

            return unions.Select(u =>
                          {
                              var cases = 
                              classes
                                .Where(c =>
                                  {
                                      return c.Symbol.BaseType.ConstructedFrom == u.Symbol;
                                  });
                              return new UnionInfo(GetClassInfo(u, false), cases.Select(x => GetClassInfo(x, true)).ToList());
                          })
                          .ToList();
        }

        private static List<string> GetCaseTypeParameters(Class c)
        {
            var unionBaseParams = c.Symbol.BaseType.ConstructedFrom.TypeParameters.Select(x => x.ToString()).ToList();
            var passedToBase = c.Symbol.BaseType.TypeArguments.Select(x => x.ToString()).ToList();
            var caseParams = c.Syntax.TypeParameterList?.Parameters.Select(x => x.ToString()).ToList() ?? new List<string>();

            if (passedToBase.Count != caseParams.Count)
            {
                throw new Exception($"Case class must have the same number of generic type parameters as union base. Case class name: {c.Symbol.Name}");
            }

            if (caseParams.Count == 0) return new List<string>();

            var resultParams = passedToBase.Select(x => caseParams.IndexOf(x)).Select(index => unionBaseParams[index]);
            return resultParams.ToList();
        }

        private static ClassInfo GetClassInfo(Class c, bool isCase)
        {
            var properties = GetProperties(c);
            var ns = GetNamespace(c.Syntax);
            var nsName = ns.Name.ToString();

            var types = c.Syntax.TypeParameterList?.Parameters.Select(x => x.ToString()).ToList() ?? new List<string>();
            var typesInTermsOfUnionBase = isCase ? GetCaseTypeParameters(c) : types;
            var usings = GetUsings(ns);

            return new ClassInfo(c.Symbol.Name, 
                properties, 
                nsName, 
                usings, 
                types.Select((b,i) => new TypeParameter(b.Trim(), typesInTermsOfUnionBase[i].Trim())).ToList());
        }


        private static NamespaceDeclarationSyntax GetNamespace(SyntaxNode syntax)
        {
            var ns = syntax as NamespaceDeclarationSyntax;
            return ns ?? GetNamespace(syntax.Parent);
        }

        private static List<string> GetUsings(NamespaceDeclarationSyntax ns)
        {
            var result = new List<string>();
            result.AddRange(GetUsings(ns.Usings));
            if (ns.Parent is NamespaceDeclarationSyntax)
            {
                result.AddRange(GetUsings((NamespaceDeclarationSyntax)ns.Parent));
            }
            if (ns.Parent is CompilationUnitSyntax)
            {
                result.AddRange(GetUsings(((CompilationUnitSyntax)ns.Parent).Usings));
            }
            return result;
        }

        private static List<string> GetUsings(SyntaxList<UsingDirectiveSyntax> usings)
        {
            return usings.Select(u => u.ToString()).ToList();
        }

        private static Dictionary<string, PropertyInfo> GetProperties(Class c)
        {
            var props =
                c.Syntax
                    .Members.OfType<PropertyDeclarationSyntax>()
                    .Where(p => p.Modifiers != null && p.Modifiers.Any(m => m.ToString() == "public")
                                && p.AccessorList != null && p.AccessorList.Accessors.All(a => a.Body == null));
            return props
                .ToDictionary(x => x.Identifier.ToString(), 
                              x =>
                              {
                                  var typeInfo = c.Model.GetTypeInfo(x.Type);
                                  return new PropertyInfo(x.Type.ToString(), typeInfo.Type.IsValueType);
                              });
        }

        private static bool IsUnionBase(Class c)
        {
            return
                new List<string> { "public", "abstract", "partial" }.All(x => c.Syntax.Modifiers.ToString().Contains(x))
                && 
                c.Syntax.AttributeLists.Any()
                && 
                c.Syntax.AttributeLists.Any(a => a.Attributes.Any(at => at.Name.ToString() == "UnionBase"));
        }
    }

    class Class
    {
        public Class(ClassDeclarationSyntax syntax, INamedTypeSymbol symbol, SemanticModel model)
        {
            Syntax = syntax;
            Symbol = symbol;
            Model = model;
        }
        public ClassDeclarationSyntax Syntax { get; }
        public INamedTypeSymbol Symbol { get; }
        public SemanticModel Model { get; }
    }
}
