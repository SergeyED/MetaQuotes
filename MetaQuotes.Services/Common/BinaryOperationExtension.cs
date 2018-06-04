using System;
using System.IO;
using System.Runtime.InteropServices;

namespace MetaQuotes.Services.Common
{
    public static class BinaryOperationExtension
    {
        /// <summary>
        /// Данный метод может использоваться для конвертирования массива байт в структуру. 
        /// Использовал его изначально, но при тестах он давал не очень хорошие результаты.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static T ReadStruct<T>(this byte[] bytes, int offset) where T : struct
        {
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(T)));

            Marshal.Copy(bytes, offset, ptr, Marshal.SizeOf(typeof(T)));
            var returnObject = (T)Marshal.PtrToStructure(ptr, typeof(T));
            Marshal.FreeHGlobal(ptr);
            
            return returnObject;
        }
    }

}
