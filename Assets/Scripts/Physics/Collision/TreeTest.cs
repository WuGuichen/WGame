using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TWY.Physics;

public class TreeTest : MonoBehaviour
{
    // Start is called before the first frame update
    public WPointOctree<GameObject> pointTree = new WPointOctree<GameObject>(15, Vector3.zero, 1);

    // private PointOctree<GameObject> pointTree = new PointOctree<GameObject>(15, Vector3.zero, 1);
    public GameObject[] objs;

    void Start()
    {
        // pointTree = new WPointOctree<GameObject>(15, this.transform.position, 1);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnGUI()
    {
        for (int i = 0; i < objs.Length; i++)
        {
            if (GUILayout.Button("AddBtn" + i, GUILayout.Width(100)))
                OnClickAdd(i);
            if (GUILayout.Button("RemoveBtn" + i, GUILayout.Width(100)))
                OnClickRemove(i);
        }
    }

    void OnClickAdd(int index)
    {
        pointTree.Add(objs[index], objs[index].transform.position);
    }

    void OnClickRemove(int index)
    {
        pointTree.Remove(objs[index]);
    }

    private void OnDrawGizmos()
    {
        pointTree.DrawAllBounds();
        pointTree.DrawAllObjects();
    }
}