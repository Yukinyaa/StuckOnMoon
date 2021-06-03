using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceGameObjectPrefabs : Singleton<SurfaceGameObjectPrefabs>
{
    public GameObject this[int idx] { get => Instance.prefabs[idx]; }
    [SerializeField]
    private List<GameObject> prefabs;
}