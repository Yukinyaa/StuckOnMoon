using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

//public struct FluidDataBuffer
//{
//    public int amount; // in centilitere =  (m^2 / 10000)
//    public int pressure; // in mH2O * g(gravity const)
//    public int flowAmount; // in liter/frame
//    public int avgflowRate; // in abs(liter/frame)
//    public byte finalizedFrame;
//}//Energy = elevation + amount/maxamount*height + pressure

[StructLayout(LayoutKind.Sequential)]
public struct Fluidbody
{
    public void Init()
    {
        for (int i = 0; i < FluidSystem.MaxNeghbors; i++)
        {
            neighboringPipes[i] = -1;
        } 
    }
    public int maxamount; // in centiliter = (cm^2)
    public int height, elevation; //in cm, pressure data
    public int length, radius;// in cm, friction data
    public uint fluidType;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = FluidSystem.MaxNeghbors)]
    public int[] neighboringPipes;

    public int amount; // in centilitere =  (m^2 / 10000)
    public int pressure; // in mH2O * g(gravity const)
    public int flowAmount; // in liter/frame
    public int avgflowRate; // in abs(liter/frame)
    public byte finalizedFrame;
    
    //todo: implemnet constant pressure
    
}
