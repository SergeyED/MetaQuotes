using System.Runtime.InteropServices;

namespace MetaQuotes.Models
{
    [StructLayout(LayoutKind.Sequential, Size = Constants.IndexesSize)]
    public struct Location
    {
        public uint index;
    }


}
