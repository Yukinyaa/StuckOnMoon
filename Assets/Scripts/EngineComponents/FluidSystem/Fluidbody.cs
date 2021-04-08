using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public struct FluidDataBuffer
{
    public int amount; // in liter =  (m^2)/100
    public int pressure; // in mH2O * g(gravity const)
    public int flowRate; // in liter/frame
    public int avgflowRate; // in abs(liter/frame)
    public byte finalizedFrameno;
}//Energy = elevation + amount/maxamount*height + pressure

unsafe public class Fluidbody : MonoBehaviour
{
    public int maxamount; // in liter, (m^2)/100
    public int width, height, elevation; //in cm, pressure data
    public int length, radius;// in cm, friction data
    public uint fluidType;
    
    public List<Fluidbody> neighboringPipes = new List<Fluidbody>();
    //[MarshalAs(UnmanagedType.ByValArray, SizeConst = UpdateManager.BufferCount)]
    public FluidDataBuffer[] dataBuffer = new FluidDataBuffer[UpdateManager.BufferCount];

    
}
