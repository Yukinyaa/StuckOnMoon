//using UnityEngine;
//using UnityEditor;
//using System.Collections;

//[CustomEditor(typeof(Fluidbody))]
//public class FluidbodyDebugEditor : Editor
//{

//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();

        
//        if (GUILayout.Button("Fill Fluid"))
//        {
//            Fluidbody body = (Fluidbody)target;
//            if (body != null)
//            {
//                body.dataBuffer[UpdateManager.CurrentBuffer].amount = body.maxamount;
//                body.dataBuffer[UpdateManager.LastBuffer].amount = body.maxamount;
//            }
                
//        }
//    }
//}
