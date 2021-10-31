using System;
using TrueSkills.APIs;

namespace TrueSkills.Exceptions
{
    public class CodeException : Exception
    {
        public CodeException()
        {

        }
        public ErrorAPI Error;
        public CodeException(ErrorAPI message) : base(message.Error)
        {
            Error = message;
        }
        public CodeException(string message) : base(message)
        {
            Error = null;
        }
    }
}
