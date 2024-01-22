using TWY.Physics;
using Unity.Mathematics;
using UnityEngine;

public class TestAABB : MonoBehaviour
{
    public AABBF shape;
    // Start is called before the first frame update
    private float3 size;
    void Start()
    {
        var bound = gameObject.GetComponent<BoxCollider>();
        shape = new AABBF(bound.center.ToFloat3(), bound.size.ToFloat3());
        size = shape.Size;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(shape.Center.ToVector3(), shape.Size.ToVector3());
    }

    // Update is called once per frame
    void Update()
    {
        shape = new AABBF(transform.position.ToFloat3(), size);
    }
}
