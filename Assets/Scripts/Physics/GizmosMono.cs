using System;
using System.Collections;
using System.Collections.Generic;
using TWY.Physics;
using UnityEngine;

public class GizmosMono : MonoBehaviour
{
    public WBVH<GameObject> bvh;
    public List<CapsuleF> capsule = new List<CapsuleF>();

    private void OnDrawGizmos()
    {
        if (bvh != null)
        {
            bvh.DrawAllBounds();
        }

        for (int i = 0; i < capsule.Count; i++)
        {
            capsule[i].Draw();
        }
    }
}
