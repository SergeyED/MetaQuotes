using System;
namespace MetaQuotes.Services.Exceptions
{
    public class IPAddressConvertToUIntException : Exception
    {
        public IPAddressConvertToUIntException(string message) : base(message)
        {
        }
    }
}
