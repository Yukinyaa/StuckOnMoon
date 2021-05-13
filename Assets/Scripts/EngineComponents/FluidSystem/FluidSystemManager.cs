#define DEBUG_FLUID_VERBOSE


using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using static UpdateManager;

public class FluidSystem
{
    BufferedFixedIndexArray<Fluidbody> fBodies;
    public const int MaxNeghbors = 32;
    
    
    public int fluidDensity = 10; // water = 10
    public float fluidViscocity = 1f;//todo : to int
    public float flowConstant = 1f; // todo: to int
    
    public int flowRateDecayRate = 5;
    // Update is called once per frame



    public void CopyToNextFrame()
    {
        fBodies.CopyToNext();
    }
    // Use this for initialization


    public void Update()
    {
        int minPressureBody = -1;
        int minPressure = -1;
        for (int i = 0; i < fBodies.Current.MaxIndex; i++)
        {
            if (!fBodies.SafeGetCurrent(i, out Fluidbody body))
                continue;

            fBodies.SafeGetLast(i, out Fluidbody lastBody);

            body.pressure = fluidDensity * lastBody.amount * body.height / body.maxamount;//head pressure
            body.flowAmount = 0;
            body.avgflowRate = (lastBody.avgflowRate * (flowRateDecayRate - 1) + lastBody.flowAmount) / flowRateDecayRate;

            if (body.amount >= body.maxamount)
            {
                for (int neg = 0; neg < MaxNeghbors; neg++)//todo: (주변 파이프압 - 마찰)중 최댓값 도입
                {
                    if (body.neighboringPipes[neg] == -1) continue;
                    Fluidbody neighbor = fBodies.GetCurrent(body.neighboringPipes[neg]);
                    // 파이프 마찰 계산
                    int pressure = (int)(neighbor.pressure
                        + fluidDensity * (neighbor.elevation - body.elevation)// reflect elevation delta
                        - fluidViscocity * neighbor.length * lastBody.avgflowRate / body.radius
                        - fluidViscocity * body.length * lastBody.avgflowRate / body.radius);//todo: viscocity to int

                    if (pressure > body.pressure)
                    {
                        body.pressure = pressure;
                    }
                }
            }
            if (minPressureBody == -1)
            {
                minPressureBody = i;
            }
            else if (body.pressure < minPressure)
            {
                minPressureBody = i;
                minPressure = body.pressure;
            }
            fBodies.Current.Set(i, body); // ★Important★★★★★★★★★★★★
        }


        if (minPressureBody == -1) return;//no entry, nothing to update


#if DEBUG_FLUID_VERBOSE
        string s = "";
        int sum = 0;
        for (int i = 0; i < fBodies.Current.MaxIndex; i++)
        {
            if (!fBodies.SafeGetCurrent(i, out Fluidbody f))
                continue;
            s += $"\n:({f.amount}/{f.maxamount}), p: {f.pressure}, fr: {f.flowAmount}";
            sum += f.amount;
        }
        Debug.Log(s);
        Debug.Log($"sum of fluid: {sum}");
#endif

        FlowFluid_Cascade(minPressureBody);
    }

    /// <summary>
    /// Calculate a frame of flud flow
    /// requires to calculate pressure of all object.
    /// 
    /// * taking recursive approach.
    ///  - calculate relative pressure to neghbors.
    ///   - if the pressure is negative(flowing *to* neghbor), calculate neghbor node first(if not already calculated).
    ///   - if the pressure is positive, take fluid from neghbors.
    ///     - if the sum of taking is greater then full tank, take from max fluid flow.
    /// 
    /// </summary>
    /// <param name="startPoint"></param>
    void FlowFluid_Cascade(int startPoint)
    {
        Stack<int> bodyToUpdate = new Stack<int>();
        int a = CurrentBuffer;
        int[] desiredFlowRate = new int[MaxNeghbors];
        int current = startPoint;
        //bodyToUpdate.Push(startPoint);
        while (bodyToUpdate.Count != 0)
        {
            if(current == -1) current = bodyToUpdate.Pop();

#if DEBUG
            if (!fBodies.SafeGetCurrent(current, out Fluidbody currentBody))
                throw new System.Exception("Fucking Idiot");
#else
            Fluidbody currentBody = fBodies.GetCurrent(current));
            
#endif

            if (currentBody.finalizedFrame == FrameHash)
            {
                current = -1;
                continue;
            }
                
            //body.dataBuffer[CurrentBuffer].amount
            int desiredFlowSum = 0;
            if (currentBody.amount == currentBody.maxamount)
            {

                for (int neg = 0; neg < MaxNeghbors; neg++)
                {
                    if (currentBody.neighboringPipes[neg] == -1) continue;
                    bodyToUpdate.Push(currentBody.neighboringPipes[neg]);
                }

                currentBody.finalizedFrame = FrameHash;
                current = -1;
                continue;
            }
            //bool doContinue = false;
            for (int neg = 0; neg < MaxNeghbors; neg++)
            {
                if (currentBody.neighboringPipes[neg] == -1) continue;

                //if delta is positive, pressure is headed towards this node.
                int pressureDelta =
                    fBodies.Current.Get(currentBody.neighboringPipes[neg]).pressure - currentBody.pressure
                    + fluidDensity * (fBodies.Current.Get(currentBody.neighboringPipes[neg]).pressure - currentBody.elevation);
                
                //if flowing to this node
                if (pressureDelta >= 0)
                { 
                    //then add flow to sum flow
                    desiredFlowSum += (desiredFlowRate[neg] = Mathf.Min((int)(pressureDelta * flowConstant), fBodies.Current.Get(currentBody.neighboringPipes[neg]).amount));
                }
                //if the pressure is negative, then calculate other body first
                else if(fBodies.Current.Get(currentBody.neighboringPipes[neg]).finalizedFrame != FrameHash)
                {   
                    //bodyToUpdate.Push(current); << will come here someday.
                    
                    current = neg;
                    //doContinue = true;
                    //break;
                    goto Cotinue;
                }
            }
            if (false)//doContinue
            {
                continue;
            }

            // case when currentbody will not be full (or be exactly full)
            if (desiredFlowSum + currentBody.amount <= currentBody.maxamount)
            {
                currentBody.amount += desiredFlowSum;
                currentBody.flowAmount += desiredFlowSum;
                

                for (int neg = 0; neg < MaxNeghbors; neg++)
                {
                    if (desiredFlowRate[neg] == 0) continue;
                    //if (currentBody.neighboringPipes[neg] == -1) continue;

                    int flowAmount = desiredFlowRate[neg];


                    if (flowAmount > 0)
                    {
                        var negbody = fBodies.Current.Get(currentBody.neighboringPipes[neg]);
                        negbody.amount -= flowAmount;
                        negbody.flowAmount += flowAmount;
                        fBodies.Current.Set(currentBody.neighboringPipes[neg], negbody);
                    }
                }
            }
            // case when currentbody will be full
            else
            {
                int actualFlowSum = currentBody.maxamount - currentBody.amount;
                currentBody.flowAmount += actualFlowSum;
                currentBody.amount = currentBody.maxamount;


                int breakCounter = 0;
                for ( ; ; )
                {
                    var (maxFlowRate, maxIndex) = desiredFlowRate.Select((n, i) => (n, i)).Max();//todo: Optimise, this is shit code;

                    if (maxFlowRate == 0)
                        throw new System.Exception("U R fucking Idiot");

                    desiredFlowRate[maxIndex] = 0;

                    maxFlowRate = Mathf.Min(actualFlowSum, maxIndex);

                    actualFlowSum -= maxFlowRate;

                    var negbody = fBodies.Current.Get(currentBody.neighboringPipes[maxIndex]);
                    negbody.amount -= maxFlowRate;
                    negbody.flowAmount += maxFlowRate;
                    fBodies.Current.Set(currentBody.neighboringPipes[maxIndex], negbody);

                    if (actualFlowSum == 0) break;

                    ++breakCounter;
                    if (breakCounter > FluidSystem.MaxNeghbors) break;
                }
            }

            for (int neg = 0; neg < MaxNeghbors; neg++)
            {
                if (currentBody.neighboringPipes[neg] == -1) continue;
                bodyToUpdate.Push(currentBody.neighboringPipes[neg]);
            }
            currentBody.finalizedFrame = FrameHash;

            fBodies.Current.Set(current, currentBody);
            current = -1;
        Cotinue: continue;
        }


    }
}
