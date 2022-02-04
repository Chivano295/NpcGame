
using UnityEngine;

namespace Steering
{
    public class Arrive : Behavior
    {
        public Vector3 Target;

        public override void Start(BehaviorContext context)
        {
            base.Start(context);
            //initialize things for behavior here
        }

        public Arrive(Vector3 position)
        {
            Target = position;
        }

        override public Vector3 CalculateSteeringForce(float dt, BehaviorContext context)
        {
            //update target position, e.g.
            positionTarget = Target;
            positionTarget.y = context.position.y;

            //calculate stop offset
            Vector3 stopVector = (context.position - positionTarget).normalized * context.settings.arriveDistance;
            Vector3 stopPosition = positionTarget + stopVector;

            //how many meters to go until stopping
            Vector3 targetOffset = stopPosition - context.position;
            float distance = targetOffset.magnitude;

            // calculate ramped speed ramped/clipped speed
            float rampedSpeed = context.settings.maxDesiredVelocity * (distance / context.settings.slowingDistance);
            float clippedSpeed = Mathf.Min(rampedSpeed, context.settings.maxDesiredVelocity);

            //update desired velocity and return force
            if (distance > 0.001f)
                velocityDesired = (clippedSpeed / distance) * targetOffset;
            else
                velocityDesired = Vector3.zero;

            return velocityDesired - context.velocity;
            
        }

        public override void OnDrawGizmos(BehaviorContext context)
        {
            base.OnDrawGizmos(context);
            // draw things for behavior
            Support.DrawWireDisc(Target, context.settings.arriveDistance, Color.yellow);
            Support.DrawWireDisc(Target, context.settings.slowingDistance, Color.yellow);
        }
    }
}