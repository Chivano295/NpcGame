using UnityEngine;
using System.Collections;

using SimpleBehaviorTree.Examples;

namespace Steering
{
    public class Flock : Behavior 
    {

        private readonly GameObject myObj;

        private int               flockLayer;
        private float             largestRadius;

        private Vector3 SteerForce = Vector3.zero;

        private BehaviorContext cont;

        private Coroutine loop;

        private AgentBrain megaBrain;

        public Flock(GameObject obj, AgentBrain brain)
        {
            myObj = obj;
            megaBrain = brain;
        }

        public override void Stop()
        {
            megaBrain.StopCoroutine(loop);
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        override public void Start(BehaviorContext context)
        {
            base.Start(context);

            cont = context;


            // get layer mask using the flock layer name
            flockLayer = LayerMask.GetMask(cont.settings.flockLayer);

            // determine largest radius
            largestRadius = 0.0f;
            if (cont.settings.flockAlignmentWeight > 0.0f)
                largestRadius = Mathf.Max(largestRadius, cont.settings.flockAlignmentRadius);
            if (cont.settings.flockCohesionWeight > 0.0f)
                largestRadius = Mathf.Max(largestRadius, cont.settings.flockCohesionRadius);
            if (cont.settings.flockSeparationWeight > 0.0f)
                largestRadius = Mathf.Max(largestRadius, cont.settings.flockSeparationRadius);

            loop = megaBrain.StartCoroutine(Epic());
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        private Vector3 CalculateDesiredVelocity(BehaviorContext context)
        {
            Vector3 pos = context.position;

            // find all neighbors
            Collider[] neighbors = Physics.OverlapSphere(pos, largestRadius, flockLayer, QueryTriggerInteraction.Ignore);

            if (neighbors.Length == 0)
                return Vector3.zero;

            // prepare to calculate the three flock calculation forces
            FlockAlignment  alignment  = new FlockAlignment (cont.settings.flockAlignmentRadius );
            FlockCohesion   cohesion   = new FlockCohesion  (cont.settings.flockCohesionRadius  );
            FlockSeparation separation = new FlockSeparation(cont.settings.flockSeparationRadius);
            
            // process all neigbors
            foreach (Collider neighbor in neighbors)
            {
                // get AgentBrain component from neighbor (if any)
                AgentBrain neighborSteering = neighbor.gameObject.GetComponent<AgentBrain>();

                if (neighborSteering == null)
                    continue;

                // calcute direction and squared distance to neighbor
                Vector3 neighborDirection = neighborSteering.position - pos;
                float   sqrDistance       = neighborDirection.sqrMagnitude;

                // skip neigbor if not in sight
                if (Vector3.Angle(myObj.transform.forward, neighborDirection) > cont.settings.flockVisibilityAngle)
                    continue;

                // update calculations
                alignment .AddVelocity (sqrDistance, neighborSteering.velocity);
                cohesion  .AddPosition (sqrDistance, neighborSteering.position);
                separation.AddDirection(sqrDistance, neighborDirection);
            }

            // calculate desired velocity
            Vector3 desiredVelocity = alignment .DesiredVelocity()                   * cont.settings.flockAlignmentWeight +
                                      cohesion  .DesiredVelocity(pos)                * cont.settings.flockCohesionWeight  +
                                      separation.DesiredVelocity()                   * cont.settings.flockSeparationWeight;

            return desiredVelocity.normalized * cont.settings.maxDesiredVelocity;
        }

        IEnumerator Epic()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.2f);

                SteerForce = CalculateDesiredVelocity(cont);
            }
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        public override Vector3 CalculateSteeringForce(float dt, BehaviorContext context)
        {
            // update target position plus desired velocity, and return steering force
            velocityDesired = SteerForce;
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
