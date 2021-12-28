using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

class InputManager : Singleton<InputManager>
{
    [SerializeField]
    public KeyCode placeKeyCode;
    [SerializeField]
    int objectOnHand = 0;

    public void DoInput()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            objectOnHand = -1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            objectOnHand = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            objectOnHand = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            objectOnHand = 3;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            objectOnHand = 4;
        }

        if (objectOnHand == -1)
        {
            return;
            //deconstruct? idk
        }


        int2 pos = SurfaceManager.Instance.MousePositionAsGridPosition();
        var placingObjectShape = SObjectTypes.sObjectTypes[objectOnHand].shape;

        int2 posArgumented = pos - (new int2(placingObjectShape.size.x, placingObjectShape.size.y) / 2);
        
        //render ghost


        if (Input.GetKey(placeKeyCode))
        {
            int2 posArgumentedInt2 = new int2(posArgumented);

            Debug.Assert(SurfaceManager.Instance.ViewingSurface.MapWidth != 0);

            while (posArgumentedInt2.x < 0)
                posArgumentedInt2.x += SurfaceManager.Instance.ViewingSurface.MapWidth;

            SurfaceEvent placeObjEvent = 
                new SurfacePlaceObjectEvent(SurfaceManager.Instance.ViewingSurfaceNo ?? SurfaceManager.Instance.CurrentSurfaceNo, objectOnHand, posArgumentedInt2);

            EventManager.Instance.RegisterLocalEvent(placeObjEvent);
        }


    }
}