using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SurfaceManager : MonoBehaviour
{
    [SerializeField]
    public KeyCode placeKeyCode;
    [SerializeField]
    Vector3 gridOffset = Vector3.zero;
    [SerializeField]
    Vector3 gridSize = Vector3.one;


#if UNITY_EDITOR
    public bool isDrawGridHintsEnabled;
#endif
    void Start()
    { 

    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(placeKeyCode))
        {
        }
    }

    Vector2Int MousePositionToGridPosition()
    {
        Vector3 input = Input.mousePosition;
        input.z = gridOffset.z;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(input);  //tip: rendering camera may not be main
        return new Vector2Int(Mathf.FloorToInt(input.x / gridSize.x), Mathf.FloorToInt(input.y / gridSize.y));

    }

    void OnDrawGizmos()
    {

#if UNITY_EDITOR
        if (isDrawGridHintsEnabled)
        {
            for (int x = -4; x <= 4; x++)
                for (int y = -4; y <= 4; y++)
                {
                    DebugExtension.DrawPoint(new Vector3(gridOffset.x + gridSize.x * x, gridOffset.y + gridSize.y * y));
                    Handles.Label(new Vector3(gridOffset.x + gridSize.x * (x + 0.5f), gridOffset.y + gridSize.y * (y + 0.5f)), $"{x},{y}", GUIStyle.none);
                }
                    

            DebugExtension.DrawArrow(Vector3.zero, new Vector3(gridSize.x, 0), Color.red);
            DebugExtension.DrawArrow(Vector3.zero, new Vector3(0, gridSize.y), Color.green);

        }
#endif
    }
}
