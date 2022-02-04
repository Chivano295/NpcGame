using UnityEngine;

namespace Steering
{
    public class Flock : Behavior 
    {
        private readonly Collider myCollider;

        private int               flockLayer;
        private float             largestRadius;

        public Flock(GameObject obj)
        {
            myCollider = obj.GetComponent<Collider>();
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        override public void Start(BehaviorContext context)
        {
            base.Start(context);

            // get layer mask using the flock layer name
            flockLayer = LayerMask.GetMask(context.settings.flockLayer);

            // determine largest radius
            largestRadius = 0.0f;
            if (context.settings.flockAlignmentWeight > 0.0f)
                largestRadius = Mathf.Max(largestRadius, context.settings.flockAlignmentRadius);
            if (context.settings.flockCohesionWeight > 0.0f)
                largestRadius = Mathf.Max(largestRadius, context.settings.flockCohesionRadius);
            if (context.settings.flockSeparationWeight > 0.0f)
                largestRadius = Mathf.Max(largestRadius, context.settings.flockSeparationRadius);
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        private Vector3 CalculateDesiredVelocity(BehaviorContext context)
        {
            // find all neighbors
            Collider[] neighbors = Physics.OverlapSphere(context.position, largestRadius, flockLayer, QueryTriggerInteraction.Ignore);
            if (neighbors.Length == 0)
                return Vector3.zero;

            // prepare to calculate the three flock calculation forces
            FlockAlignment  alignment  = new FlockAlignment (context.settings.flockAlignmentRadius );
            FlockCohesion   cohesion   = new FlockCohesion  (context.settings.flockCohesionRadius  );
            FlockSeparation separation = new FlockSeparation(context.settings.flockSeparationRadius);
            
            // process all neigbors
            foreach (Collider neighbor in neighbors)
            {
                // skip this agent
                if (neighbor == myCollider)
                    continue;

                // get steering component from neighbor (if any)
                Steering neighborSteering = neighbor.gameObject.GetComponent<Steering>();
                if (neighborSteering == null)
                {
                    Debug.LogError($"ERROR: Flock Behavior found neighbor in layer {context.settings.flockLayer} without Steering script!");
                    continue;
                }

                // calcute direction and squared distance to neighbor
                Vector3 neighborDirection = neighborSteering.position - context.position;
                float   sqrDistance       = neighborDirection.sqrMagnitude;

                // skip neigbor if not in sight
                if (Vector3.Angle(myCollider.transform.forward, neighborDirection) > context.settings.flockVisibilityAngle)
                    continue;

                // update calculations
                alignment .AddVelocity (sqrDistance, neighborSteering.velocity);
                cohesion  .AddPosition (sqrDistance, neighborSteering.position);
                separation.AddDirection(sqrDistance, neighborDirection);
            }

            // calculate desired velocity
            Vector3 desiredVelocity = alignment .DesiredVelocity()                   * context.settings.flockAlignmentWeight +
                                      cohesion  .DesiredVelocity(context.position) * context.settings.flockCohesionWeight  +
                                      separation.DesiredVelocity()                   * context.settings.flockSeparationWeight;
            return desiredVelocity.normalized * context.settings.maxDesiredVelocity;
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        public override Vector3 CalculateSteeringForce(float dt, BehaviorContext context)
        {
            // update target position plus desired velocity, and return steering force 
            velocityDesired = CalculateDesiredVelocity(context);
            positionTarget  = context.position + velocityDesired * dt;
            return velocityDesired - context.velocity;
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        override public void OnDrawGizmos(BehaviorContext context)
        {
            base.OnDrawGizmos(context);
#if false
            DrawGizmos.DrawWireDisc(context.position, largestRadius, Color.black);
#endif
#if false
            // calculate the world rotation (the angle the velocity makes in with the vector to the right)
            float worldRotation = Vector3.SignedAngle(Vector3.right, context.velocity, -Vector3.up) * Mathf.Deg2Rad;

            // draw view lines
            float a = context.settings.flockVisibilityAngle * Mathf.Deg2Rad;
            Vector3 rangeMin = new Vector3(largestRadius * Mathf.Cos(worldRotation - a),
                                           0.0f,
                                           largestRadius * Mathf.Sin(worldRotation - a));

            Vector3 rangeMax = new Vector3(largestRadius * Mathf.Cos(worldRotation + a),
                                           0.0f,
                                           largestRadius * Mathf.Sin(worldRotation + a));

            Debug.DrawLine(context.position, context.position + rangeMin, Color.black);
            Debug.DrawLine(context.position, context.position + rangeMax, Color.black);
#endif
        }
    }
}
