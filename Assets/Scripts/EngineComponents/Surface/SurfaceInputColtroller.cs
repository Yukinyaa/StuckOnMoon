using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;


public class SurfaceInputColtroller : MonoBehaviour
{
    [SerializeField]
    public KeyCode placeKeyCode;

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
            for (int x = -10; x <= 10; x++)
                for (int y = -10; y <= 10; y++)
 

            DebugExtension.DrawArrow(Vector3.zero, new Vector3(gridSize.x, 0), Color.red);
            DebugExtension.DrawArrow(Vector3.zero, new Vector3(0, gridSize.y), Color.green);

        }
#endif
    }
}
