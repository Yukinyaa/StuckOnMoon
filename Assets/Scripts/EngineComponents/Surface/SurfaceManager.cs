using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SurfaceManager : Singleton<SurfaceManager>
{

    [SerializeField]
    public KeyCode placeKeyCode;
    [SerializeField]
    Vector3 gridOffset = Vector3.zero;
    [SerializeField]
    Vector3 gridSize = Vector3.one;
    [SerializeField]
    int objectOnHand = 0;

    FixedIndexArray<SurfaceController> surfaces;// = new SurfaceController();

#if UNITY_EDITOR
    [SerializeField]
    private bool isDrawGridGizmoEnabled;
    [SerializeField]
    private bool isDrawBlockGizmoEnabled;
#endif
    int currentSurface = 0;
    int viewingSurface = 0;
    private void Awake()
    {
        SObjectTypes.Init();//move elsewhere
        surfaces = FixedIndexArray<SurfaceController>();
        new SurfaceController();

    }
    public void PrepareFrame()
    {
        surface.PrepareFrame();
    }
    // Update is called once per frame
    public void DoUpdate()
    {
        DoInput();
        DoRender();
    }
    void DoInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
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
            //unimplimented
        }
        Vector2 pos = MousePositionAsGridPosition();
        var placingObjectShape = SObjectTypes.sObjectTypes[objectOnHand].shape;

        Vector2 posArgumented = pos - (new Vector2(placingObjectShape.size.x, placingObjectShape.size.y) / 2);
        //render ghost


        if (Input.GetKey(placeKeyCode))
        {
            int2 posArgumentedInt2 = new int2(posArgumented);
            while (posArgumentedInt2.x < 0) posArgumentedInt2.x += surface.chunkController.mapWidth;
            surface.chunkController.CanPlaceObject(posArgumentedInt2, objectOnHand);

            SurfaceObject newObject = new SurfaceObject(posArgumentedInt2, objectOnHand);
            int newObjectIndex = surface
            surface.chunkController.RegisterObject(posArgumentedInt2, objectOnHand);

            SurfaceEvent
        }


    }
    void DoRender()
    { 
    }
    public Vector2Int MousePositionAsGridPosition()
    {
        Vector3 input = Input.mousePosition;
        input.z = gridOffset.z;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(input);  //tip: rendering camera may not be main
        return new Vector2Int(Mathf.FloorToInt((worldPos.x - gridOffset.x) / gridSize.x), Mathf.FloorToInt(worldPos.y - gridOffset.y / gridSize.y));

    }

    void OnDrawGizmos()
    {

#if UNITY_EDITOR
        if (isDrawGridGizmoEnabled)
        {
            for (int x = -4; x <= 4; x++)
                for (int y = 0; y <= 8; y++)
                {
                    
                    DebugExtension.DrawPoint(new Vector3(gridOffset.x + gridSize.x * x, gridOffset.y + gridSize.y * y));
                    Handles.Label(new Vector3(gridOffset.x + gridSize.x * (x + 0.5f), gridOffset.y + gridSize.y * (y + 0.5f)), $"{(x<0?x+surface?.chunkController.mapWidth:x)},{y}", GUIStyle.none);
                }
            DebugExtension.DrawArrow(Vector3.zero, new Vector3(gridSize.x, 0), Color.red);
            DebugExtension.DrawArrow(Vector3.zero, new Vector3(0, gridSize.y), Color.green);
        }


        if (isDrawBlockGizmoEnabled)
        {
            //surface1?.chunkController?.ForEachObject(o => Handles.Label(new Vector3(o.postion.x + 0.5f, o.postion.y + 0.5f), SObjectTypes.sObjectTypes[o.objectType].name));// o.objectType.ToString()
            surface?.chunkController?.ForEachObject(o => Handles.Label(new Vector3(o.postion.x + 0.5f, o.postion.y + 0.5f), o.ToString()));// o.objectType.ToString()
            surface?.chunkController?.ForEachObject(o => DebugExtension.DrawBounds(
                                new Bounds(o.Middle, new Vector2(o.shape.size.x, o.shape.size.y))
                    ));// o.objectType.ToString()
        }
        

#endif
    }
}
