using System;
namespace MetaQuotes.Services.Exceptions
{
    public class ConvertBinaryToClassException : Exception
    {
        public ConvertBinaryToClassException(string message, Exception exception) : base(message, exception)
        {
        }
    }
}
