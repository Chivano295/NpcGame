using UnityEngine;

namespace Steering
{
    /// <summary>
    /// Support class to calculate the desired velocity to match the average velocity of the neighbor agents.
    /// </summary>
    class FlockAlignment
    {
        private float   sqrRadius;
        private int     neighborCount = 0;
        private Vector3 total         = Vector3.zero;

        public FlockAlignment(float radius)
        {
            sqrRadius = radius * radius;
        }

        public void AddVelocity(float sqrDistance, Vector3 neighborVelocity)
        {
            if (sqrDistance <= sqrRadius)
            {
                total += neighborVelocity;
                neighborCount++;
            }
        }

        public Vector3 DesiredVelocity()
        {
            if (neighborCount > 0)
                return (total / neighborCount).normalized;
            else
                return Vector3.zero;
        }
    }
}
