using System;
using System.Collections.Generic;
using System.Linq;

namespace DiscriminatedUnionsGen
{
    public static class SafeLowerCase
    {
        private static List<String> Keywords = new List<string>
        {
            "abstract",
            "as",
            "base",
            "bool",
            "break",
            "byte",
            "case",
            "catch",
            "char",
            "checked",
            "class",
            "const",
            "continue",
            "decimal",
            "default",
            "delegate",
            "do",
            "double",
            "else",
            "enum",
            "event",
            "explicit",
            "extern",
            "false",
            "finally",
            "fixed",
            "float",
            "for",
            "foreach",
            "goto",
            "if",
            "implicit",
            "in",
            "int",
            "interface",
            "internal",
            "is",
            "lock",
            "long",
            "namespace",
            "new",
            "null",
            "object",
            "operator",
            "out",
            "override",
            "params",
            "private",
            "protected",
            "public",
            "readonly",
            "ref",
            "return",
            "sbyte",
            "sealed",
            "short",
            "sizeof",
            "stackalloc",
            "static",
            "string",
            "struct",
            "switch",
            "this",
            "throw",
            "true",
            "try",
            "typeof",
            "uint",
            "ulong",
            "unchecked",
            "unsafe",
            "ushort",
            "using",
            "virtual",
            "void",
            "volatile",
            "while",
        };

        public static string ToLowerCase(string name)
        {
            if (name == null) return null;
            if (name == string.Empty) return string.Empty;

            if (!char.IsLower(name.First()))
            {
                var lowerPart = new string(name.TakeWhile(char.IsUpper).ToArray()).ToLower();
                var res = lowerPart + name.Substring(lowerPart.Length);
                return EnsureNotKeyword(res);
            }
            return EnsureNotKeyword("_" + name);
        }

        private static string EnsureNotKeyword(string name)
        {
            var result = name;
            while (Keywords.Contains(result))
            {
                result = "_" + result;
            }
            return result;
        }
    }
}
