#define DEBUG_FLUID_VERBOSE


using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using static UpdateManager;

public class FluidSystemManager : MonoBehaviour
{
    List<Fluidbody> fBodies;
    List<Fluidbody> endPoints;
    // Use this for initialization
    void Start()
    {
        fBodies = FindObjectsOfType<Fluidbody>().ToList();
        endPoints = fBodies.FindAll(f => f.neighboringPipes.Count == 1);
    }

    int fluidDensity = 10; // water = 10
    public float fluidViscocity = 1f;//todo : to int
    public float flowConstant = 1f; //todo: to int

    [Tooltip("Higher the rate, slower the rate change")]
    public int flowRateDecayRate = 5;
    // Update is called once per frame
    void Update()
    {
        Fluidbody minPressureBody = null;
        foreach (Fluidbody body in fBodies)
        {
            body.dataBuffer[CurrentBuffer].amount = body.dataBuffer[LastBuffer].amount;
            body.dataBuffer[CurrentBuffer].pressure 
                = fluidDensity * body.dataBuffer[LastBuffer].amount * body.height / body.maxamount;//head pressure
            body.dataBuffer[CurrentBuffer].flowRate = 0;
            body.dataBuffer[CurrentBuffer].avgflowRate = (body.dataBuffer[LastBuffer].avgflowRate * (flowRateDecayRate - 1) + body.dataBuffer[LastBuffer].flowRate) / flowRateDecayRate;

            if (body.dataBuffer[CurrentBuffer].amount >= body.maxamount)
            {
                for (int i = 0; i < body.neighboringPipes.Count; i++)//(주변 파이프압 - 마찰)중 최댓값 도입
                {
                    // 파이프 마찰 계산
                    int pressure = (int)(body.neighboringPipes[i].dataBuffer[LastBuffer].pressure
                        + fluidDensity * (body.neighboringPipes[i].elevation - body.elevation)// reflect elevation delta
                        - fluidViscocity * body.neighboringPipes[i].length * body.neighboringPipes[i].dataBuffer[LastBuffer].avgflowRate / body.neighboringPipes[i].radius
                        - fluidViscocity * body.length * body.dataBuffer[LastBuffer].avgflowRate / body.radius);//todo: viscocity to int

                    /*
                    if (body.dataBuffer[CurrentBuffer].amount < body.maxamount)
                        pressure = pressure * body.dataBuffer[CurrentBuffer].amount / body.maxamount;
                        */

                    if (pressure > body.dataBuffer[CurrentBuffer].pressure)
                    {
                        body.dataBuffer[CurrentBuffer].pressure = pressure;
                    }
                }
            }
            if (minPressureBody == null)
            {
                minPressureBody = body;
            }
            else if (body.dataBuffer[CurrentBuffer].pressure < minPressureBody.dataBuffer[CurrentBuffer].pressure)
            {
                minPressureBody = body;
            }
        }


        if (minPressureBody == null) return;//no entry, nothing to update


#if DEBUG_FLUID_VERBOSE
        string s = "";

        foreach (Fluidbody f in fBodies)
        {
            s += $"\n{f.name}:({f.dataBuffer[CurrentBuffer].amount}/{f.maxamount}), p: {f.dataBuffer[CurrentBuffer].pressure}, fr: {f.dataBuffer[CurrentBuffer].flowRate}";
        }
        Debug.Log(s);
#endif

        FlowFluidTo_Cascade(minPressureBody);
    }
    void FlowFluidTo_Cascade(Fluidbody startPoint)
    {
        Stack<Fluidbody> bodyToUpdate = new Stack<Fluidbody>();
        int a = CurrentBuffer;

        Fluidbody current = startPoint;
        //bodyToUpdate.Push(startPoint);
        while (bodyToUpdate.Count != 0 || current != null )
        {
            if(current == null) current = bodyToUpdate.Pop();
            
            if(current.dataBuffer[CurrentBuffer].finalizedFrameno == frameHash)
            {
                current = null;
                continue;
            }
                


            //body.dataBuffer[CurrentBuffer].amount
            int desiredFlowSum = 0;
            if (current.dataBuffer[CurrentBuffer].amount == current.maxamount)
            {
                for (int i = 0; i < current.neighboringPipes.Count; i++)
                    bodyToUpdate.Push(current.neighboringPipes[i]);

                current.dataBuffer[CurrentBuffer].finalizedFrameno = frameHash;    
                current = null;
                continue;
            }
            bool doContinue = false;
            for (int i = 0; i < current.neighboringPipes.Count; i++)
            {
                //if delta is positive, pressure is headed towards this node.
                int pressureDelta = 
                    current.neighboringPipes[i].dataBuffer[CurrentBuffer].pressure - current.dataBuffer[CurrentBuffer].pressure
                    + fluidDensity * (current.neighboringPipes[i].elevation - current.elevation);

                //if flowing to this node
                if (pressureDelta >= 0)
                { 
                    //then add flow to sum flow
                    desiredFlowSum += Mathf.Min((int)(pressureDelta * flowConstant), current.neighboringPipes[i].dataBuffer[CurrentBuffer].amount);
                }
                else if(current.neighboringPipes[i].dataBuffer[CurrentBuffer].finalizedFrameno != frameHash)
                {
                    
                    bodyToUpdate.Push(current);
                    current = current.neighboringPipes[i];
                    doContinue = true;
                    break;
                }

            }
            if(doContinue) continue;

            if (desiredFlowSum + current.dataBuffer[CurrentBuffer].amount < current.maxamount)
            {
                current.dataBuffer[CurrentBuffer].amount += desiredFlowSum;
                current.dataBuffer[CurrentBuffer].flowRate += desiredFlowSum;

                for (int i = 0; i < current.neighboringPipes.Count; i++)
                {
                    int pressureDelta =
                        current.neighboringPipes[i].dataBuffer[CurrentBuffer].pressure - current.dataBuffer[CurrentBuffer].pressure
                        + fluidDensity * (current.neighboringPipes[i].elevation - current.elevation);
                    int flowRate = Mathf.Min((int)(pressureDelta * flowConstant), current.neighboringPipes[i].dataBuffer[CurrentBuffer].amount);
                    
                    if (pressureDelta > 0)
                    {
                        current.neighboringPipes[i].dataBuffer[CurrentBuffer].amount -= flowRate;
                        current.neighboringPipes[i].dataBuffer[CurrentBuffer].flowRate += flowRate;
                    }
                }
            }
            else
            {
                int actualFlowSum = current.maxamount - current.dataBuffer[CurrentBuffer].amount;
                current.dataBuffer[CurrentBuffer].flowRate += actualFlowSum;
                current.dataBuffer[CurrentBuffer].amount = current.maxamount;

                for (int i = 0; i < current.neighboringPipes.Count; i++)
                {
                    int pressureDelta =
                        current.neighboringPipes[i].dataBuffer[CurrentBuffer].pressure - current.dataBuffer[CurrentBuffer].pressure
                        + fluidDensity * (current.neighboringPipes[i].elevation - current.elevation);
                    int flowRate = Mathf.Min((int)(pressureDelta * flowConstant), current.neighboringPipes[i].dataBuffer[CurrentBuffer].amount) /desiredFlowSum * actualFlowSum;
                    
                    if (pressureDelta > 0)
                    {
                        current.neighboringPipes[i].dataBuffer[CurrentBuffer].amount -= flowRate;
                        current.neighboringPipes[i].dataBuffer[CurrentBuffer].flowRate += flowRate;
                    }
                }
            }

            for (int i = 0; i < current.neighboringPipes.Count; i++)
                bodyToUpdate.Push(current.neighboringPipes[i]);
            current.dataBuffer[CurrentBuffer].finalizedFrameno = frameHash;
            current = null;

        }


    }
}
