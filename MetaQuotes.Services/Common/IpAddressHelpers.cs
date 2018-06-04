using System;
using System.Net;
using MetaQuotes.Services.Exceptions;

namespace MetaQuotes.Services.Common
{
    public static class IpAddressHelpers
    {
        public static uint IpToUint (string ipString)
        {
            IPAddress ipAddress;
            if (IPAddress.TryParse(ipString, out ipAddress))
            {
                var ipBytes = ipAddress.GetAddressBytes();
                var ip = (uint)ipBytes[0] << 24;
                ip += (uint)ipBytes[1] << 16;
                ip += (uint)ipBytes[2] << 8;
                ip += (uint)ipBytes[3];
                return ip;
            }
            throw new IpAddressConvertToUIntException($"Ошибка при конвертировании IP: {ipString} в UInt");
        }

        public static string IpUintToString(uint ipUint)
        {
            try
            {
                var ipBytes = BitConverter.GetBytes(ipUint);
                var ipBytesRevert = new byte[4];
                ipBytesRevert[0] = ipBytes[3];
                ipBytesRevert[1] = ipBytes[2];
                ipBytesRevert[2] = ipBytes[1];
                ipBytesRevert[3] = ipBytes[0];

                return new IPAddress(ipBytesRevert).ToString();
            } catch (Exception exception){
                throw new UIntIpAddressConvertStringException($"Ошибка при конвертировании UInt: {ipUint} в IP", exception);
            }
        }
    }
}
