using DiscriminatedUnionsGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscriminatedUnionsConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var a = RoslynAnalyzer.ReadUnions(@"C:\Projects\DiscriminatedUnions\DiscriminatedUnions\DiscriminatedUnions.csproj");
            Console.WriteLine(TextGenerators.GenerateUnion(a[0]));

//            var res =
//TextGenerators.GenerateUnion(new UnionInfo(new ClassInfo("PaymentMethod", new Dictionary<string, string>()),
//                                           new List<ClassInfo>
//                                           {
//                                                           new ClassInfo("Cash", new Dictionary<string, string> { ["Amount"] = "int" }),
//                                                           new ClassInfo("Wire", new Dictionary<string, string>{
//                                                               ["Amount"] = "int",
//                                                               ["Bank"] = "string",
//                                                               ["ISIN"] = "long",
//                                                           }),
//                                                           new ClassInfo("CreditCard", new Dictionary<string, string>()),
//                                           }));
            //Console.WriteLine(res);
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }
    }
}
