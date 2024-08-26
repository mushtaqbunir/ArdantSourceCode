using System;

namespace ArdantOffical.Helpers
{
    public class NotFoundException : Exception
    {
        public NotFoundException()
        {

        }
        public NotFoundException(string message) : base(message)
        {

        }
    }
}
