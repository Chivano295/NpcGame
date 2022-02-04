using UnityEngine;

namespace Steering
{
    public abstract class Behavior: IBehavior
    {
        [Header("Behavior Runtime")]
        public Vector3 positionTarget  = Vector3.zero; // target position
        public Vector3 velocityDesired = Vector3.zero; // desired velocity

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        public virtual void Start(BehaviorContext context)
        {
            positionTarget = context.position;
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        public abstract Vector3 CalculateSteeringForce(float dt, BehaviorContext context);

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        public virtual void OnDrawGizmos(BehaviorContext context)
        {
            // draw desired velocity
            Support.DrawRay(context.position, velocityDesired, Color.blue);
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        public bool ArriveEnabled(BehaviorContext context)
        {
            return context.settings.slowingDistance > 0.0f;
        }

        public bool WithinArriveSlowingDistance(BehaviorContext context, Vector3 positionTarget)
        {
            return (positionTarget - context.position).sqrMagnitude < context.settings.slowingDistance * context.settings.slowingDistance;
        }

        public Vector3 CalculateArriveSteeringForce(BehaviorContext context, Vector3 posTarget)
        {
            // make sure we have a legal slowing distance
            if (!ArriveEnabled(context))
                return Vector3.zero;

            // update target position 
            positionTarget     = posTarget;
            positionTarget.y   = context.position.y;

            // calculate actual stop offset 
            Vector3 stopVector   = (context.position - positionTarget).normalized * context.settings.arriveDistance;
            Vector3 stopPosition = positionTarget + stopVector;

            // calculate the target offset and distance
            Vector3 targetOffset = stopPosition - context.position;
            float   distance     = targetOffset.magnitude;

            // calculate the ramped speed and clip it
            float   rampedSpeed  = context.settings.maxDesiredVelocity * (distance / context.settings.slowingDistance);
            float   clippedSpeed = Mathf.Min(rampedSpeed, context.settings.maxDesiredVelocity);

            // update desired velocity and return steering force
            if (distance > 0.001f)
                velocityDesired = (clippedSpeed / distance) * targetOffset;
            else
                velocityDesired = Vector3.zero;
            return velocityDesired - context.velocity;
        }

        public void OnDrawArriveGizmos(BehaviorContext context)
        {
            // draw a circle around the target so we can see where we Start breaking and where we must stop
            Support.DrawWireDisc(positionTarget, context.settings.arriveDistance,  Color.yellow);
            Support.DrawWireDisc(positionTarget, context.settings.slowingDistance, Color.yellow);
        }
    }
}