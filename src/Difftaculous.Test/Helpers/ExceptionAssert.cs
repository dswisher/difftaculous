
using System;
using NUnit.Framework;


namespace Difftaculous.Test.Helpers
{
    // From Json.Net
    public static class ExceptionAssert
    {
        public static void Throws<TException>(string message, Action action)
            where TException : Exception
        {
            try
            {
                action();

                Assert.Fail("Exception of type {0} expected; got none exception", typeof(TException).Name);
            }
            catch (TException ex)
            {
                if (message != null)
                {
                    Assert.AreEqual(message, ex.Message, "Unexpected exception message." + Environment.NewLine + "Expected: " + message + Environment.NewLine + "Got: " + ex.Message + Environment.NewLine + Environment.NewLine + ex);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Exception of type {0} expected; got exception of type {1}.", typeof(TException).Name, ex.GetType().Name), ex);
            }
        }
    }
}
