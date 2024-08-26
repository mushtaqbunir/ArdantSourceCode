using System;

namespace ArdantOffical.Helpers
{
    public class Ok : Exception
    {
        public Ok()
        {

        }
        public Ok(string message) : base(message)
        {

        }
    }
}
