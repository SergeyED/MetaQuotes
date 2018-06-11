using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Взял стандартный SequenceEqual https://github.com/Microsoft/referencesource/blob/master/System.Core/System/Linq/Enumerable.cs 
        /// и изменил его под свою задачу. Теперь он мне возвращает смещение
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static int SequenceEqual(this IEnumerable<byte> first, IEnumerable<byte> second)
        {
            var comparer = EqualityComparer<byte>.Default;
            if (first == null) throw new ArgumentNullException("first");
            if (second == null) throw new ArgumentNullException("second");
            using (IEnumerator<byte> e1 = first.GetEnumerator())
            using (IEnumerator<byte> e2 = second.GetEnumerator())
            {
                while (e1.MoveNext())
                {
                    if (!(e2.MoveNext() && comparer.Equals(e1.Current, e2.Current))) return e1.Current - e2.Current;
                }
                if (e2.MoveNext()) return e1.Current - e2.Current;
            }
            return 0;
        }
    }

}
