using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nysa.Logics
{

    public abstract class Option<T>
    {
        public static implicit operator Option<T>(T value) => new Some<T>(value);
        public static readonly Option<T> None = new None<T>();
        public static implicit operator Option<T>(Option option) => Option<T>.None;
        public abstract Boolean IsSome { get; }
        /// <summary>
        /// Return this object as Some<typeparamref name="T"/> if null if this object is None.
        /// </summary>
        /// <returns></returns>
        public Some<T> AsSome() => this as Some<T>;
    }

    public sealed class Some<T> : Option<T>
    {
        public override Boolean IsSome => true;
        public T Value { get; private set; }
        public Some(T value) { this.Value = value; }
        public override String ToString() => this.Value.ToString();
    }

    public sealed class None<T> : Option<T>
    {
        public static IEnumerable<T> Return() { yield break; }

        public override Boolean IsSome => false;
        internal None() { }
        public override String ToString() => "{Option.None}";
    }

}
