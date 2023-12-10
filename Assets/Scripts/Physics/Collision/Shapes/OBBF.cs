using Unity.Mathematics;

namespace TWY.Physics
{
    public class OBBF
    {
        private float3 center;
        private float3x3 rm;
        private float3 extents;

        public float3 Center
        {
            get => center;
            set => center = value;
        }

        public float3x3 RM
        {
            get => rm;
            set => rm = value;
        }

        public float3 Extents
        {
            get => extents;
            set => extents = value;
        }

        #region 相交测试

        public bool IntersectOBB(OBBF obb)
        {
            return false;
        }

        #endregion

    }
}
