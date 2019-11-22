using System;

namespace Nysa.Logics
{

    public sealed class Option
    {
        public static readonly Option None = new Option();

        public static Option<T> Create<T>(T some) { return some; }

        private Option() { }

    } // class Option

}
