using System;
using System.Collections;
using System.Collections.Generic;
using TWY.Physics;
using UnityEngine;

public class TestCapsule : MonoBehaviour
{
    public CapsuleF shape;

    private Vector3 lenUp;
    private float radius;

    private Transform trans;
    // Start is called before the first frame update
    void Start()
    {
        trans = transform;
        var cap = gameObject.GetComponent<CapsuleCollider>();
        var len = cap.height / 2 - cap.radius;
        var pA = cap.center + len*Vector3.up;
        lenUp = len * Vector3.up;
        var pB = cap.center - len*Vector3.up;
        shape = new CapsuleF(pA, pB, cap.radius);
        radius = cap.radius;
    }

    private void OnDrawGizmos()
    {
        shape.Draw();
    }

    // Update is called once per frame
    void Update()
    {
        var pA = trans.position + lenUp;
        var pB = trans.position - lenUp;
        shape = new CapsuleF(pA, pB, radius);
    }
}
