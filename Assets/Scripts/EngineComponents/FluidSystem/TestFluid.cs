using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using static UpdateManager;

public class TestPipe : MonoBehaviour
{
    public Int32 amount, maxamount; // in m^2
    public Int32 width, height, elevation; //in m
    public Int16 length, diameter;

    void Awake()
    {
        var pipe = GetComponent<Fluidbody>();
        pipe.maxamount = maxamount;
        pipe.width = width;
        pipe.height = height;
        pipe.elevation = elevation;
    }
    private void LateUpdate()
    {
        if (amount != 0)
        {
            GetComponent<Fluidbody>().dataBuffer[CurrentBuffer].amount = amount;
            amount = 0;
        }
        
    }
}
