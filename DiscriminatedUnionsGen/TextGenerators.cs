using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.Formatting;

namespace DiscriminatedUnionsGen
{
    public static class Templates
    {
        public const string UNION_BASE_CLASS =
@"namespace [Namespace]
{
    [Usings]

    public abstract partial class [UnionNameG]
    {
        [Constructor]

        [MatchMethod]

        [DoMethod]

        [StaticConstructors]

        [Equals]

        [GetHashCode]

        [EqualsOperators]

        internal abstract void Seal();
    }
}
";

        public const string MATCH_METHOD =
@"public TV Match<TV>([MatchFuncs])
{
    [MatchCases]
    throw new NotSupportedException();
}
";

        public const string MATCH_FUNC = "Func<[CaseNameG], TV> [CaseNameLowerCase]";
        public const string MATCH_CASE =
@"if (this.GetType() == typeof([CaseNameG]))
{
    return [CaseNameLowerCase](([CaseNameG])this);
}
";

        public const string DO_METHOD =
@"public void Do([DoActions])
{
    [DoCases]
    throw new NotSupportedException();
}
";

        public const string DO_ACTION = "Action<[CaseNameG]> [CaseNameLowerCase]";
        public const string DO_CASE =
@"if (this.GetType() == typeof([CaseNameG]))
{
    [CaseNameLowerCase](([CaseNameG])this);
    return;
}
";

        public const string CASE_CLASS =
@"namespace [Namespace]
{
    [Usings]

    public sealed partial class [CaseNameG]
    {
        [Constructor]

        [Equals]

        [GetHashCode]

        [EqualsOperators]

        internal override void Seal() { /* Do nothing */ }
    }
}
";

        public const string BASE_CONSTRUCTOR =
@"public [UnionName]([ConstructorArguments])
{
    [ConstructorAssignments]
}
";

        public const string CASE_CONSTRUCTOR =
@"public [CaseName]([ConstructorArguments])[BaseConstructorCall]
{
    [ConstructorAssignments]
}
";

        public const string STATIC_CONSTRUCTOR =
@"public static [CaseNameG] [CaseName]([ConstructorArguments])
{
    return new [CaseNameG]([ConstructorParameters]);
}
";
        public const string BASE_EQUALS =
@"public override bool Equals(object obj) => obj is [UnionNameG] && Equals(([UnionNameG]) obj);

public bool Equals([UnionNameG] other)
    => [EqualsComparisons];
";

        public const string CASE_EQUALS =
@"public override bool Equals(object obj) => obj is [CaseNameG] && Equals(([CaseNameG]) obj);

public bool Equals([CaseNameG] other)
    => [EqualsComparisons];
";

        public const string GET_HASH_CODE =
@"public override int GetHashCode()
{
    unchecked
    {
        var hashCode = base.GetHashCode();
        [AdditionalGetHashCodeLines]
        return hashCode;
    }
}
";

        public const string BASE_EQUALS_OPERATORS =
@"public static bool operator ==([UnionNameG] left, [UnionNameG] right)
{
    return Equals(left, right);
}

public static bool operator !=([UnionNameG] left, [UnionNameG] right)
{
    return !Equals(left, right);
}";
        public const string CASE_EQUALS_OPERATORS =
@"public static bool operator ==([CaseNameG] left, [CaseNameG] right)
{
    return Equals(left, right);
}

public static bool operator !=([CaseNameG] left, [CaseNameG] right)
{
    return !Equals(left, right);
}";

    }

    public static class TextGenerators 
    {
        public static string GenerateUnion(UnionInfo unionInfo)
        {
            //TODO: pass one stringbuilder around

            var result = new StringBuilder(Templates.UNION_BASE_CLASS);

            result.Replace("[Namespace]", unionInfo.BaseClassInfo.Namespace);
            result.Replace("[Usings]", GetUsings(unionInfo.BaseClassInfo));
            result.Replace("[MatchMethod]", Templates.MATCH_METHOD);
            result.Replace("[MatchFuncs]", string.Join(",", unionInfo.CaseClassInfos.Select(GetMatchFunc)));
            result.Replace("[MatchCases]", string.Join("", unionInfo.CaseClassInfos.Select(GetMatchCase)));

            result.Replace("[DoMethod]", Templates.DO_METHOD);
            result.Replace("[DoActions]", string.Join(",", unionInfo.CaseClassInfos.Select(GetDoAction)));
            result.Replace("[DoCases]", string.Join("", unionInfo.CaseClassInfos.Select(GetDoCase)));

            result.Replace("[StaticConstructors]", string.Join("", unionInfo.CaseClassInfos.Select(c => GetStaticConstructor(c, unionInfo.BaseClassInfo))));
            result.Replace("[Equals]", GetBaseEquals(unionInfo.BaseClassInfo));
            result.Replace("[GetHashCode]", GetHashCode(unionInfo.BaseClassInfo));
            result.Replace("[EqualsOperators]", Templates.BASE_EQUALS_OPERATORS);
            FillBaseConstructor(unionInfo.BaseClassInfo, result);


            result.Replace("[UnionNameG]", unionInfo.BaseClassInfo.NameWithGenerics);
            result.Replace("[UnionName]", unionInfo.BaseClassInfo.Name);
            result.Replace("[UnionNameLowerCase]", unionInfo.BaseClassInfo.LowerCaseName);

            foreach(var c in unionInfo.CaseClassInfos)
            {
                result.Append(Templates.CASE_CLASS);
                result.Replace("[Namespace]", c.Namespace);
                result.Replace("[Usings]", GetUsings(c));
                FillCaseConstructor(unionInfo, result, c);

                result.Replace("[Equals]", GetCaseEquals(c));
                result.Replace("[GetHashCode]", GetHashCode(c));
                result.Replace("[EqualsOperators]", Templates.CASE_EQUALS_OPERATORS);

                result.Replace("[CaseNameG]", c.NameWithGenerics);
                result.Replace("[CaseName]", c.Name);
            }

            return CodeFormatter.Format(result.ToString());
        }

        private static void FillCaseConstructor(UnionInfo unionInfo, StringBuilder result, ClassInfo c)
        {
            result.Replace("[Constructor]", Templates.CASE_CONSTRUCTOR);
            result.Replace("[ConstructorArguments]", GetCaseConstructorArguments(c, unionInfo.BaseClassInfo, useBaseGenericTypes: false));
            result.Replace("[BaseConstructorCall]", GetBaseConstructorCall(unionInfo.BaseClassInfo));
            result.Replace("[ConstructorAssignments]",
                string.Join(Environment.NewLine,
                    c.PublicProperties.Select(p => p.Key + " = " + SafeLowerCase.ToLowerCase(p.Key) + ";")));
        }

        private static void FillBaseConstructor(ClassInfo baseInfo, StringBuilder result)
        {
            result.Replace("[Constructor]", Templates.BASE_CONSTRUCTOR);
            result.Replace("[ConstructorArguments]", GetBaseConstructorArguments(baseInfo));
            result.Replace("[BaseConstructorCall]", "");
            result.Replace("[ConstructorAssignments]",
                string.Join(Environment.NewLine,
                    baseInfo.PublicProperties.Select(p => p.Key + " = " + SafeLowerCase.ToLowerCase(p.Key) + ";")));
        }

        private static string GetUsings(ClassInfo a)
        {
            var usings = a.Usings.Concat(new List<string> {"using System;"}).Distinct();
            return string.Join(Environment.NewLine, usings);
        }

        private static string GetBaseConstructorArguments(ClassInfo baseInfo)
        {
            return string.Join(",", baseInfo.PublicProperties.Select(p => p.Value.Type + " " + SafeLowerCase.ToLowerCase(p.Key)));
        }
        private static string GetCaseConstructorArguments(ClassInfo c, ClassInfo baseInfo, bool useBaseGenericTypes)
        {
            var caseArgs = c.PublicProperties.Select(p =>
            {
                var valueType = useBaseGenericTypes ? ReplaceTypeWithBaseType(c, p.Value.Type) : p.Value.Type;
                return valueType + " " + SafeLowerCase.ToLowerCase(p.Key);
            });
            var baseArgs = baseInfo.PublicProperties.Select(p =>
            {
                var valueType = useBaseGenericTypes ? p.Value.Type : ReplaceTypeWithCaseType(c, p.Value.Type);
                return valueType + " " + SafeLowerCase.ToLowerCase(p.Key);
            });
            return string.Join(",", caseArgs.Concat(baseArgs));
        }

        private static string GetBaseConstructorCall(ClassInfo unionBase)
        {
            if (unionBase.PublicProperties.Count == 0) return string.Empty;
            return ":base(" + string.Join(",",unionBase.PublicProperties.Select(p => SafeLowerCase.ToLowerCase(p.Key))) + ")";
        }

        private static string ReplaceTypeWithBaseType(ClassInfo c, string type)
        {
            return ReplaceTypes(c, type, x => x.CaseName, x => x.BaseName);
        }

        private static string ReplaceTypeWithCaseType(ClassInfo c, string type)
        {
            return ReplaceTypes(c, type, x => x.BaseName, x => x.CaseName);
        }
        private static string ReplaceTypes(ClassInfo c, string type, Func<TypeParameter, string> from, Func<TypeParameter, string> to)
        {
            var splitted = type.Replace("<", "|<|").Replace(">", "|>|").Replace(",", "|,|").Split('|');
            return string.Join("", splitted.Select(s =>
            {
                var match = c.TypeParameters.FirstOrDefault(x => from(x) == s.Trim());
                return match != null ? to(match) : s;
            }));
        }

        private static string GetMatchFunc(ClassInfo caseInfo)
        {
            return
                Templates.MATCH_FUNC
                .Replace("[CaseNameG]", caseInfo.NameWithGenericsInTermsOfUnionBase)
                .Replace("[CaseNameLowerCase]", caseInfo.LowerCaseName);
        }

        private static string GetMatchCase(ClassInfo caseInfo)
        {
            return
                Templates.MATCH_CASE
                .Replace("[CaseNameG]", caseInfo.NameWithGenericsInTermsOfUnionBase)
                .Replace("[CaseNameLowerCase]", caseInfo.LowerCaseName);
        }

        private static string GetDoAction(ClassInfo caseInfo)
        {
            return
                Templates.DO_ACTION
                .Replace("[CaseNameG]", caseInfo.NameWithGenericsInTermsOfUnionBase)
                .Replace("[CaseNameLowerCase]", caseInfo.LowerCaseName);
        }

        private static string GetDoCase(ClassInfo caseInfo)
        {
            return
                Templates.DO_CASE
                .Replace("[CaseNameG]", caseInfo.NameWithGenericsInTermsOfUnionBase)
                .Replace("[CaseNameLowerCase]", caseInfo.LowerCaseName);
        }

        private static string GetStaticConstructor(ClassInfo caseInfo, ClassInfo baseInfo)
        {
            var keys = Enumerable.Union(caseInfo.PublicProperties, baseInfo.PublicProperties).Select(x => x.Key);
            return
                Templates.STATIC_CONSTRUCTOR
                .Replace("[CaseName]", caseInfo.Name)
                .Replace("[CaseNameG]", caseInfo.NameWithGenericsInTermsOfUnionBase)
                .Replace("[ConstructorArguments]", GetCaseConstructorArguments(caseInfo, baseInfo, useBaseGenericTypes: true))
                .Replace("[ConstructorParameters]", string.Join(",", keys.Select(SafeLowerCase.ToLowerCase)));
        }

        private static string GetBaseEquals(ClassInfo info)
        {
            return
                Templates.BASE_EQUALS
                    .Replace("[EqualsComparisons]", GetEqualsForProperties(info.PublicProperties));
        }

        private static string GetEqualsForProperties(Dictionary<string, PropertyInfo> properties)
            => properties.Any() 
               ? string.Join(" &&" + Environment.NewLine, properties.Select(x => $"this.{x.Key}.Equals(other.{x.Key})"))
               : "true";

        private static string GetCaseEquals(ClassInfo info)
        {
            return
                Templates.CASE_EQUALS
                    .Replace("[EqualsComparisons]",
                    "base.Equals(other)" + 
                     (info.PublicProperties.Any() 
                     ? " &&" + Environment.NewLine + GetEqualsForProperties(info.PublicProperties)
                     : ""));
        }


        private static string GetHashCode(ClassInfo info)
        {
            var additional = info.PublicProperties.Select(p => GetAdditionalGetHashCodeLines(p.Key, p.Value.IsValueType));
            return Templates.GET_HASH_CODE
                .Replace("[AdditionalGetHashCodeLines]", string.Join(Environment.NewLine, additional));
        }

        private static string GetAdditionalGetHashCodeLines(string propertyName, bool isValueType)
        {
            if (isValueType) return $"hashCode = (hashCode* 397) ^ {propertyName}.GetHashCode();";
            else return $"hashCode = (hashCode * 397) ^ ({propertyName} != null ? {propertyName}.GetHashCode() : 0);";
        }
    }
}
