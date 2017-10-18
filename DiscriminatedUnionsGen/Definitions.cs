using System;
using System.Collections.Generic;
using System.Linq;

namespace DiscriminatedUnionsGen
{
    public class UnionInfo
    {
        public UnionInfo(ClassInfo baseClassInfo, List<ClassInfo> caseClassInfos)
        {
            BaseClassInfo = baseClassInfo;
            CaseClassInfos = caseClassInfos;
        }
        public ClassInfo BaseClassInfo { get; }
        public List<ClassInfo> CaseClassInfos { get; }
    }

    public class ClassInfo
    {
        public ClassInfo(string name, 
            Dictionary<string, PropertyInfo> publicProperties, 
            string ns, 
            List<string> usings, 
            List<TypeParameter> typeParameters)
        {
            Name = name;
            PublicProperties = publicProperties;
            Namespace = ns;
            Usings = usings;
            TypeParameters = typeParameters;
            LowerCaseName = SafeLowerCase.ToLowerCase(name);
        }
        public string LowerCaseName { get; }
        public string Name { get; }
        public Dictionary<string, PropertyInfo> PublicProperties { get; }
        public string Namespace { get; }
        public List<string> Usings { get; }
        public List<TypeParameter> TypeParameters { get; }
        public string NameWithGenerics 
            => Name + FormatTypeParameters(TypeParameters.Select(x => x.CaseName).ToList());
        public string NameWithGenericsInTermsOfUnionBase 
            => Name + FormatTypeParameters(TypeParameters.Select(x => x.BaseName).ToList());

        private string FormatTypeParameters(List<string> parameters)
        {
            if (parameters.Count == 0) return string.Empty;
            return $"<{string.Join(",", parameters)}>";
        }
    }

    public class TypeParameter
    {
        public TypeParameter(string caseName, string baseName) { CaseName = caseName; BaseName = baseName; }

        public string CaseName { get; }
        public string BaseName { get; }
    }

    public class PropertyInfo
    {
        public PropertyInfo(string type, bool isValueType)
        {
            Type = type;
            IsValueType = isValueType;
        }

        public string Type { get; }
        public bool IsValueType { get; }
    }
}
