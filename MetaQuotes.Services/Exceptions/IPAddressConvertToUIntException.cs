using System;
namespace MetaQuotes.Services.Exceptions
{
    public class IpAddressConvertToUIntException : Exception
    {
        public IpAddressConvertToUIntException(string message) : base(message)
        {
        }
    }
}
