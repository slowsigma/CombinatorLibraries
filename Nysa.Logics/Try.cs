using System;
using System.Threading.Tasks;

namespace Nysa.Logics
{

    public static class Try
    {

        public static Suspect<T> This<T>(Func<T> func, String failMessage = null)
        {
            try
            {
                return func();
            }
            catch (Exception except)
            {
                return failMessage == null
                       ? except
                       : new Exception(failMessage, except);
            }
        }


        public static async Task<Suspect<T>> ThisAsync<T>(Func<Task<T>> func, String failMessage = null)
        {
            try
            {
                return await func();
            }
            catch (Exception except)
            {
                return failMessage == null
                       ? except
                       : new Exception(failMessage, except);
            }
        }

        public static Option<Exception> This(Action action, String failMessage = null)
        {
            try
            {
                action();

                return Option<Exception>.None;
            }
            catch (Exception except)
            {
                return failMessage == null
                       ? except
                       : (new Exception(failMessage, null));
            }
        }

    }

}
