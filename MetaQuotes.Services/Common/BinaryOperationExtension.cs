using System;
using System.IO;
using System.Runtime.InteropServices;

namespace MetaQuotes.Services
{
    public static class BinaryOperationExtension
    {
        public static T ReadStruct<T>(this BinaryReader reader) where T : struct
        {
            byte[] rawData = reader.ReadBytes(Marshal.SizeOf(typeof(T)));
            GCHandle handle = GCHandle.Alloc(rawData, GCHandleType.Pinned);
            var returnObject = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            return returnObject;
        }

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
