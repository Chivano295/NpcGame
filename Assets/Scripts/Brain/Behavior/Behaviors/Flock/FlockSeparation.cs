using UnityEngine;

namespace Steering
{
    /// <summary>
    /// Support class to calculate the desired velocity to move away from the neighbor agents.
    /// </summary>
    class FlockSeparation
    {
        private float   sqrRadius;
        private int     neighborCount = 0;
        private Vector3 total         = Vector3.zero;

        public FlockSeparation(float radius)
        {
            sqrRadius = radius * radius;
        }

        public void AddDirection(float sqrDistance, Vector3 neighborDirection)
        {
            if (sqrDistance <= sqrRadius)
            {
                total += neighborDirection.normalized;
                neighborCount++;
            }
        }

        public Vector3 DesiredVelocity()
        {
            if (neighborCount > 0)
                return -(total / (float)neighborCount).normalized;
            else
                return Vector3.zero;
        }
    }
}
