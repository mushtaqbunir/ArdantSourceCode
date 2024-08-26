using System;

namespace ArdantOffical.Helpers
{
    public class UnAuthorizedException : Exception
    {
        public UnAuthorizedException()
        {

        }
        public UnAuthorizedException(string message) : base(message)
        {

        }
    }
}
