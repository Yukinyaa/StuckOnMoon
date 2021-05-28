using FixMath.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;


namespace SoM.SurfaceGen
{
    public class SurfaceGenClampNode : Node
    {
        [Input] public SurfaceGenAbstractNode<Fix64> input;
        [Output] public SurfaceGenAbstractNode<bool> output;
        public long clampValueRaw64;

        public override object GetValue(NodePort port)
        {
            SurfaceGenAbstractNode<Fix64> input = GetInputValue<SurfaceGenAbstractNode<Fix64>>("input", this.input);
            return new SurfaceGenClamp() { prevNode = input, clampValue = Fix64.FromRaw(clampValueRaw64) };
        }
    }
}