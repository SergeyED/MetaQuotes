using System;
namespace MetaQuotes.Services.Exceptions
{
    public class UIntIpAddressConvertStringException : Exception
    {
        public UIntIpAddressConvertStringException(string message, Exception exception) : base(message, exception)
        {
        }
    }
}
