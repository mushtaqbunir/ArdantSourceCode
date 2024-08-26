using System;

namespace ArdantOffical.Helpers
{

    public class BadRequestException : Exception
    {
        public BadRequestException()
        {

        }
        public BadRequestException(string message) : base(message)
        {

        }
    }
}
