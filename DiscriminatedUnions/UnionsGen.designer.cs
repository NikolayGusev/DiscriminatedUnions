 
namespace DiscriminatedUnions
{
    using System;
    using System.Threading.Tasks;
    using DiscriminatedUnionsAttributes;
    using ABC = System.Threading.Tasks.Task<int>;

    public abstract partial class Payment2
    {
        public Payment2()
        {

        }


        public TV Match<TV>(Func<Cash, TV> cash, Func<Cheque, TV> cheque, Func<CreditCard, TV> creditCard)
        {
            if (this.GetType() == typeof(Cash))
            {
                return cash((Cash)this);
            }
            if (this.GetType() == typeof(Cheque))
            {
                return cheque((Cheque)this);
            }
            if (this.GetType() == typeof(CreditCard))
            {
                return creditCard((CreditCard)this);
            }

            throw new NotSupportedException();
        }


        public void Do(Action<Cash> cash, Action<Cheque> cheque, Action<CreditCard> creditCard)
        {
            if (this.GetType() == typeof(Cash))
            {
                cash((Cash)this);
                return;
            }
            if (this.GetType() == typeof(Cheque))
            {
                cheque((Cheque)this);
                return;
            }
            if (this.GetType() == typeof(CreditCard))
            {
                creditCard((CreditCard)this);
                return;
            }

            throw new NotSupportedException();
        }


        public static Cash Cash(int amount)
        {
            return new Cash(amount);
        }
        public static Cheque Cheque(string signee, string country)
        {
            return new Cheque(signee, country);
        }
        public static CreditCard CreditCard(string type, Guid number)
        {
            return new CreditCard(type, number);
        }


        public override bool Equals(object obj) => obj is Payment2 && Equals((Payment2)obj);

        public bool Equals(Payment2 other)
            => true;


        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = base.GetHashCode();

                return hashCode;
            }
        }


        public static bool operator ==(Payment2 left, Payment2 right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Payment2 left, Payment2 right)
        {
            return !Equals(left, right);
        }

        internal abstract void Seal();
    }
}
namespace DiscriminatedUnions
{
    using System;
    using System.Threading.Tasks;
    using DiscriminatedUnionsAttributes;
    using ABC = System.Threading.Tasks.Task<int>;

    public sealed partial class Cash
    {
        public Cash(int amount)
        {
            Amount = amount;
        }


        public override bool Equals(object obj) => obj is Cash && Equals((Cash)obj);

        public bool Equals(Cash other)
            => base.Equals(other) &&
        this.Amount.Equals(other.Amount);


        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ Amount.GetHashCode();
                return hashCode;
            }
        }


        public static bool operator ==(Cash left, Cash right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Cash left, Cash right)
        {
            return !Equals(left, right);
        }

        internal override void Seal() { /* Do nothing */ }
    }
}
namespace DiscriminatedUnions
{
    using System;
    using System.Threading.Tasks;
    using DiscriminatedUnionsAttributes;
    using ABC = System.Threading.Tasks.Task<int>;

    public sealed partial class Cheque
    {
        public Cheque(string signee, string country)
        {
            Signee = signee;
            Country = country;
        }


        public override bool Equals(object obj) => obj is Cheque && Equals((Cheque)obj);

        public bool Equals(Cheque other)
            => base.Equals(other) &&
        this.Signee.Equals(other.Signee) &&
        this.Country.Equals(other.Country);


        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (Signee != null ? Signee.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Country != null ? Country.GetHashCode() : 0);
                return hashCode;
            }
        }


        public static bool operator ==(Cheque left, Cheque right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Cheque left, Cheque right)
        {
            return !Equals(left, right);
        }

        internal override void Seal() { /* Do nothing */ }
    }
}
namespace DiscriminatedUnions
{
    using System;
    using System.Threading.Tasks;
    using DiscriminatedUnionsAttributes;
    using ABC = System.Threading.Tasks.Task<int>;

    public sealed partial class CreditCard
    {
        public CreditCard(string type, Guid number)
        {
            Type = type;
            Number = number;
        }


        public override bool Equals(object obj) => obj is CreditCard && Equals((CreditCard)obj);

        public bool Equals(CreditCard other)
            => base.Equals(other) &&
        this.Type.Equals(other.Type) &&
        this.Number.Equals(other.Number);


        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (Type != null ? Type.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Number.GetHashCode();
                return hashCode;
            }
        }


        public static bool operator ==(CreditCard left, CreditCard right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CreditCard left, CreditCard right)
        {
            return !Equals(left, right);
        }

        internal override void Seal() { /* Do nothing */ }
    }
}

namespace DiscriminatedUnions
{
    using System;
    using System.Threading.Tasks;
    using DiscriminatedUnionsAttributes;
    using ABC = System.Threading.Tasks.Task<int>;

    public abstract partial class MyOption<T, T2>
    {
        public MyOption(Task<T> baseObjectValue, int baseIntValue, string baseStringValue, ABC aliasedType)
        {
            BaseObjectValue = baseObjectValue;
            BaseIntValue = baseIntValue;
            BaseStringValue = baseStringValue;
            AliasedType = aliasedType;
        }


        public TV Match<TV>(Func<Some<T2, T>, TV> some, Func<None<T, T2>, TV> none)
        {
            if (this.GetType() == typeof(Some<T2, T>))
            {
                return some((Some<T2, T>)this);
            }
            if (this.GetType() == typeof(None<T, T2>))
            {
                return none((None<T, T2>)this);
            }

            throw new NotSupportedException();
        }


        public void Do(Action<Some<T2, T>> some, Action<None<T, T2>> none)
        {
            if (this.GetType() == typeof(Some<T2, T>))
            {
                some((Some<T2, T>)this);
                return;
            }
            if (this.GetType() == typeof(None<T, T2>))
            {
                none((None<T, T2>)this);
                return;
            }

            throw new NotSupportedException();
        }


        public static Some<T2, T> Some(T objectValue, int intValue, string stringValue, int doubleValue, MyOption<T2, T> myOptionValue, Task<T> baseObjectValue, int baseIntValue, string baseStringValue, ABC aliasedType)
        {
            return new Some<T2, T>(objectValue, intValue, stringValue, doubleValue, myOptionValue, baseObjectValue, baseIntValue, baseStringValue, aliasedType);
        }
        public static None<T, T2> None(None<T, int> optionValue, Task<T> baseObjectValue, int baseIntValue, string baseStringValue, ABC aliasedType)
        {
            return new None<T, T2>(optionValue, baseObjectValue, baseIntValue, baseStringValue, aliasedType);
        }


        public override bool Equals(object obj) => obj is MyOption<T, T2> && Equals((MyOption<T, T2>)obj);

        public bool Equals(MyOption<T, T2> other)
            => this.BaseObjectValue.Equals(other.BaseObjectValue) &&
        this.BaseIntValue.Equals(other.BaseIntValue) &&
        this.BaseStringValue.Equals(other.BaseStringValue) &&
        this.AliasedType.Equals(other.AliasedType);


        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (BaseObjectValue != null ? BaseObjectValue.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ BaseIntValue.GetHashCode();
                hashCode = (hashCode * 397) ^ (BaseStringValue != null ? BaseStringValue.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (AliasedType != null ? AliasedType.GetHashCode() : 0);
                return hashCode;
            }
        }


        public static bool operator ==(MyOption<T, T2> left, MyOption<T, T2> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MyOption<T, T2> left, MyOption<T, T2> right)
        {
            return !Equals(left, right);
        }

        internal abstract void Seal();
    }
}
namespace DiscriminatedUnions
{
    using System;
    using System.Threading.Tasks;
    using DiscriminatedUnionsAttributes;
    using ABC = System.Threading.Tasks.Task<int>;

    public sealed partial class Some<T, T3>
    {
        public Some(T3 objectValue, int intValue, string stringValue, int doubleValue, MyOption<T, T3> myOptionValue, Task<T3> baseObjectValue, int baseIntValue, string baseStringValue, ABC aliasedType) : base(baseObjectValue, baseIntValue, baseStringValue, aliasedType)
        {
            ObjectValue = objectValue;
            IntValue = intValue;
            StringValue = stringValue;
            DoubleValue = doubleValue;
            MyOptionValue = myOptionValue;
        }


        public override bool Equals(object obj) => obj is Some<T, T3> && Equals((Some<T, T3>)obj);

        public bool Equals(Some<T, T3> other)
            => base.Equals(other) &&
        this.ObjectValue.Equals(other.ObjectValue) &&
        this.IntValue.Equals(other.IntValue) &&
        this.StringValue.Equals(other.StringValue) &&
        this.DoubleValue.Equals(other.DoubleValue) &&
        this.MyOptionValue.Equals(other.MyOptionValue);


        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (ObjectValue != null ? ObjectValue.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ IntValue.GetHashCode();
                hashCode = (hashCode * 397) ^ (StringValue != null ? StringValue.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ DoubleValue.GetHashCode();
                hashCode = (hashCode * 397) ^ (MyOptionValue != null ? MyOptionValue.GetHashCode() : 0);
                return hashCode;
            }
        }


        public static bool operator ==(Some<T, T3> left, Some<T, T3> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Some<T, T3> left, Some<T, T3> right)
        {
            return !Equals(left, right);
        }

        internal override void Seal() { /* Do nothing */ }
    }
}
namespace DiscriminatedUnions
{
    using System;
    using System.Threading.Tasks;
    using DiscriminatedUnionsAttributes;
    using ABC = System.Threading.Tasks.Task<int>;

    public sealed partial class None<T, T2>
    {
        public None(None<T, int> optionValue, Task<T> baseObjectValue, int baseIntValue, string baseStringValue, ABC aliasedType) : base(baseObjectValue, baseIntValue, baseStringValue, aliasedType)
        {
            OptionValue = optionValue;
        }


        public override bool Equals(object obj) => obj is None<T, T2> && Equals((None<T, T2>)obj);

        public bool Equals(None<T, T2> other)
            => base.Equals(other) &&
        this.OptionValue.Equals(other.OptionValue);


        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (OptionValue != null ? OptionValue.GetHashCode() : 0);
                return hashCode;
            }
        }


        public static bool operator ==(None<T, T2> left, None<T, T2> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(None<T, T2> left, None<T, T2> right)
        {
            return !Equals(left, right);
        }

        internal override void Seal() { /* Do nothing */ }
    }
}
