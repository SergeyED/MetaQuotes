using System.Runtime.InteropServices;

namespace MetaQuotes.Models.Version2
{
    /// <summary>
    /// Заголовок для экспериментального чтения с целью попробовать держать базу в массиве байт и по нему искать
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct HeaderBuffer
    {
        [FieldOffset(0)] public int Version;
        [FieldOffset(4)] public sbyte Name;
        [FieldOffset(36)] public ulong Timestamp;
        [FieldOffset(44)] public int Records;
        [FieldOffset(48)] public uint OffsetRanges;
        [FieldOffset(52)] public uint OffsetCities;
        [FieldOffset(56)] public uint OffsetLocations;
    }
}
