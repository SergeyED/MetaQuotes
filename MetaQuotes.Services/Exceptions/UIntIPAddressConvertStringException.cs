using System;
namespace MetaQuotes.Services.Exceptions
{
    public class UIntIPAddressConvertStringException : Exception
    {
        public UIntIPAddressConvertStringException(string message, Exception exception) : base(message, exception)
        {
        }
    }
}
