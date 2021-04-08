using UnityEngine;
using System.Collections;

public class UITestFluid : MonoBehaviour
{

    // Use this for initialization
    void Awake()
    {
        Vector3[] v = new Vector3[4];
        GetComponent<RectTransform>().GetWorldCorners(v);
            //It starts bottom left and rotates to top left, then top right, and finally bottom right

        Vector3 size = v[2] - v[0];
        Debug.DrawLine(v[2], v[0], Color.red, 3);


        Fluidbody f = GetComponent<Fluidbody>();

        f.maxamount = (int)(size.x * size.y)/100;

        f.width = (int)size.x;
        f.height = (int)size.y;
        f.elevation = (int)v[0].y;

        f.length = Mathf.Max(f.width, f.height);
        f.radius = Mathf.Min(f.width, f.height);

    }

    // Update is called once per frame
    void Update()
    {

    }
}
