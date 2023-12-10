using Unity.Mathematics;

namespace TWY.Physics
{
    public struct RayF
    {
        private float3 _origin;
        private float3 _direction;

        public RayF(float3 origin, float3 direction)
        {
            this._origin = origin;
            this._direction = math.normalize(direction);
        }

        public float3 origin
        {
            get => this._origin;
            set => this._origin = value;
        }

        public float3 direction
        {
            get => this._direction;
            set => this._direction = value;
        }

        public float3 GetPoint(float distance) => this._origin + this._direction * distance;
    }
}
