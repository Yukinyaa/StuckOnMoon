using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;



/// <summary>
/// Triple or quad-buffered datas.
/// </summary>
/// <typeparam name="T">is recommended to be size over 64 bytes(google: cache line size)</typeparam>
[StructLayout(LayoutKind.Sequential)]
struct BufferedData<T> where T : unmanaged
{
    /// <summary>
    /// The datas
    /// </summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = UpdateManager.BufferCount)]
    T[] array;
    public T Current => array[UpdateManager.CurrentBuffer];
    public T Last { get => array[UpdateManager.LastBuffer]; }
}
