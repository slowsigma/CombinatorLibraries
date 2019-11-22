using System;
using System.Collections.Generic;
using System.Text;

namespace Nysa.Logics
{

    public abstract class Suspect<T>
    {
        public static implicit operator Suspect<T>(Exception exception) => new Failed<T>(exception);
        public static implicit operator Suspect<T>(T value) => new Confirmed<T>(value);
    }

    public sealed class Failed<T> : Suspect<T>
    {
        public static implicit operator Exception(Failed<T> failed) => failed.Value;

        public Exception Value { get; private set; }

        public Failed(Exception error) { this.Value = error; }

        public override String ToString() => this.Value.Message;
    }

    public sealed class Confirmed<T> : Suspect<T>
    {
        public static implicit operator T(Confirmed<T> confirmed) => confirmed.Value;

        public T Value { get; private set; }

        public Confirmed(T value) { this.Value = value; }

        public override String ToString() => this.Value.ToString();
    }

}
