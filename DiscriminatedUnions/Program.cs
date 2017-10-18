
using System;
using System.Threading.Tasks;
using DiscriminatedUnionsAttributes;
using ABC = System.Threading.Tasks.Task<int>;

namespace DiscriminatedUnions
{
    class Program
    {
        static void Main(string[] args)
        {
        }

    }

    [UnionBase]
    public abstract partial class Payment2 { }
    public partial class Cash : Payment2 { public int Amount { get; } }
    public partial class Cheque : Payment2
    {
        public string Signee { get; }
        public string Country { get; }
    }
    public partial class CreditCard : Payment2
    {
        public string Type { get; }
        public Guid Number { get; }
    }

    [UnionBase]
    public  abstract partial class MyOption<T, T2>
    {
        public Task<T> BaseObjectValue { get; }
        public int BaseIntValue { get; }
        public string BaseStringValue { get; }
        public ABC AliasedType { get; }
    }

    public partial class Some<T, T3>: MyOption<T3, T>
    {
        public T3 ObjectValue { get; }
        public int IntValue { get; }
        public string StringValue { get; }
        public int DoubleValue { get; }
        public MyOption<T, T3> MyOptionValue { get; }
    }

    public partial class None<T, T2>: MyOption<T, T2>
    {
        public None<T, int> OptionValue { get; }
    }
}
