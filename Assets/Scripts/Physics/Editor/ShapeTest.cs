#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TWY.Physics;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.TestTools;

public class ShapeTest
{
    private GizmosMono _gizmosMono;

    // A Test behaves as an ordinary method
    [Test]
    public void ShapeTestSimplePasses()
    {
        // Use the Assert class to test conditions
        
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator ShapeTestWithEnumeratorPasses()
    {
        float t = 0;
        
        GameObject cameraObj = new GameObject("Camera");
        Camera camera = cameraObj.AddComponent<Camera>();
        camera.clearFlags = CameraClearFlags.Color;
        camera.backgroundColor = new Color(77f / 255f, 77f / 255f, 77f / 255f);
        _gizmosMono = cameraObj.AddComponent<GizmosMono>();
        CapsuleF cap = new CapsuleF(float3.zero, new float3(0, 5f, 0), 2f);
        CapsuleF cap1 = new CapsuleF(float3.zero, new float3(-3f, 5f, 0), 2f);
        _gizmosMono.capsule.Add(cap);
        _gizmosMono.capsule.Add(cap1);
        while (t < 10)
        {
            t += Time.deltaTime;


            yield return null;
        }
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
    }
}

#endif