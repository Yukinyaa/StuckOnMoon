using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;


public class SimpleNode : Node
{
    [Input] public float a;
    [Output] public float b;

    public override object GetValue(NodePort port)
    {
        if (port.fieldName == "b") return GetInputValue<float>("a", a);
        else return null;
    }
}