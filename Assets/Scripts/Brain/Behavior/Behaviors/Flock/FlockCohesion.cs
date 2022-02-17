using UnityEngine;

namespace Steering
{
    /// <summary>
    /// Support class to calculate the desired velocity to steer towards 'center of mass' aka the average position of the neighbor agents.
    /// </summary>
    class FlockCohesion
    {
        private float   sqrRadius;
        private int     neighborCount = 0;
        private Vector3 total         = Vector3.zero;

        public FlockCohesion(float radius)
        {
            sqrRadius = radius * radius;
        }

        public void AddPosition(float sqrDistance, Vector3 neighborPosition)
        {
            if (sqrDistance <= sqrRadius)
            {
                total += neighborPosition;
                neighborCount++;
            }
        }

        public Vector3 DesiredVelocity(Vector3 position)
        {
            if (neighborCount > 0)
                return (total / neighborCount - position).normalized;
            else
                return Vector3.zero;
        }
    }
}
