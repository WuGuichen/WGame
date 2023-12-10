using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using TWY.Physics;
using Unity.Mathematics;
using UnityEngine;

public class CamTest : MonoBehaviour
{
    // Start is called before the first frame update
    public TWY.Physics.WPointOctree<GameObject> tree;
    public TreeTest test;
    public List<GameObject> nearBy = new List<GameObject>();
    public float dis = 20;

    void Start()
    {
        tree = test.pointTree;
        foreach (var obj in test.objs)
        {
            tree.Add(obj, obj.transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += new Vector3(-1, 0, 0);
        }

        if (Input.GetKey(KeyCode.D))
            transform.position += new Vector3(1, 0, 0);
        if (Input.GetKey(KeyCode.W))
            transform.position += new Vector3(0, 1, 0);
        if (Input.GetKey(KeyCode.S))
            transform.position += new Vector3(0, -1, 0);
        if (Input.GetKey(KeyCode.L))
            transform.position += new Vector3(0, 0, 1);
        if (Input.GetKey(KeyCode.H))
            transform.position += new Vector3(0, 0, -1);
        if (Input.GetKey(KeyCode.R))
            transform.position = new Vector3(6.9200000f, 10.4899998f, -36.0699997f);

        Vector3 pos = transform.position;
        RayF ray = new RayF(new float3(pos.x, pos.y, pos.z), new float3(0, 0, 1));
        if (tree.GetNearbyNonAlloc(ray, dis, nearBy))
        {
        }

        Debug.DrawRay(transform.position, new Vector3(0, 0, 1));
    }

    private void OnGUI()
    {
        // Ray ray = new Ray(transform.position, new Vector3(0, 0, 1));
    }
}