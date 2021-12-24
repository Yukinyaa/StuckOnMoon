using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSurfaceGen : MonoBehaviour
{
    SurfaceGen gen = new SurfaceGen(
            new byte[] { 32,210,130 }
        );
    public SpriteRenderer sr;



    public float scale;
    public int size;
    public float z;
    

    // Start is called before the first frame update
    void Start()
    {
        sr.sprite = gen.TestPerlin(z, scale, size);
    }

    [ExecuteInEditMode]
    private void OnValidate()
    {
        sr.sprite = gen.TestPerlin(z, scale, size);
    }
}
