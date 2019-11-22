using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nysa.Logics
{

    public static class OptionExtensions
    {

        public static Option<U> Select<T, U>(this Option<T> option, Func<T, U> selector)
            =>   option is Some<T> some ? selector(some.Value)
               :                          Option<U>.None;

        public static Option<U> Select<T, U>(this Option<T> option, Func<T, Option<U>> selector)
            =>   option is Some<T> some ? selector(some.Value)
               :                          Option<U>.None;

        public static U Select<T, U>(this Option<T> option, Func<T, U> someSelector, Func<U> noneSelector)
            =>   option is Some<T> some ? someSelector(some.Value)
               :                          noneSelector();

        public static U Select<T, U>(this Option<T> option, Func<T, U> someSelector, U noneValue)
            =>   option is Some<T> some ? someSelector(some.Value)
               :                          noneValue;

        public static Option<V> SelectMany<T, U, V>(this Option<T> first, Option<U> second, Func<T, U, V> selector)
            =>     first  is Some<T> someFirst
                && second is Some<U> someSecond ? selector(someFirst.Value, someSecond.Value)
              :                                   Option<V>.None;

        public static Option<T> OrOther<T>(this Option<T> option, Option<T> @default)
            => option is Some<T> some ? option : @default;

        public static T OrValue<T>(this Option<T> option, T @default)
            => option is Some<T> some ? some.Value : @default;

        public static Suspect<T> OrError<T>(this Option<T> option, Func<Exception> error)
            =>   option is Some<T> some ? some.Value.ToSuspect()
               :                          error();

        public static T OrNull<T>(this Option<T> option) where T : class
            => option is Some<T> some ? some.Value : null;

        //public static Option<T> OrOther<T>(this Option<T> first, params Option<T>[] others)
        //    =>   first is Some<T> some ? some.Value
        //       : others.Length > 0     ? others.FirstOrDefault(o => o.IsSome) ?? Option<T>.None
        //       :                         Option<T>.None;

        public static Boolean Affect<T>(this Option<T> option, Action<T> onSome)
        {
            if (option is Some<T> some)
            {
                onSome(some.Value);
                return true;
            }
            else
                return false;
        }

        public static void Affect<T>(this Option<T> option, Action<T> onSome, Action onNone)
        {
            if (option is Some<T> some)
                onSome(some.Value);
            else
                onNone();
        }

        public static Option<T> AsOption<T>(this T thing, Boolean nullAsNone = true)
            where T : class
            => thing == null && nullAsNone ? Option<T>.None : thing;

        public static Option<T> AsOption<T>(this T thing)
            where T : struct
            => thing;

        public static Option<T> AsOption<T>(this Nullable<T> thing, Boolean nullAsNone = true)
            where T : struct
            => !thing.HasValue && nullAsNone ? Option<T>.None : thing;

        public static Option<T> AsOptionIf<T>(this T thing, Func<T, Boolean> isNone)
            => isNone(thing) ? Option<T>.None : thing;


        public static Suspect<Option<T>> Inverted<T>(this Option<Suspect<T>> option)
            => option is Some<Suspect<T>> some
               ?   some.Value is Confirmed<T> confirmed ? (Suspect<Option<T>>)((Option<T>)confirmed.Value)
                 : some.Value is Failed<T>    failed    ? (Suspect<Option<T>>)failed.Value
                 :                                        throw new InvalidCastException()
               : (Suspect<Option<T>>)Option<T>.None;

        public static Option<T> FirstOrNone<T>(this IEnumerable<T> items, Func<T, Boolean> predicate)
        {
            foreach (var item in items)
                if (predicate(item))
                    return item;

            return Option<T>.None;
        }
    }

}
