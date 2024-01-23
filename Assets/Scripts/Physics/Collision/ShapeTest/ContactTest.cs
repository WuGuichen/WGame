using System.Collections;
using System.Collections.Generic;
using TWY.Physics;
using UnityEngine;

public class ContactTest : MonoBehaviour
{
    // Start is called before the first frame update
    public TestAABB aabb;
    public TestCapsule capsule;
    private int step = 1;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
            step++;
        if (Input.GetKeyDown(KeyCode.K))
            step--;
        if (step < 1)
            step = 1;
        if (ShapeGJK.GJK(aabb.shape, capsule.shape, out var normal, out var depth, out var point))
        {
            capsule.transform.position += (normal * depth).ToVector3();
        }
    }
}
