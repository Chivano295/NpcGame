
using UnityEngine;

namespace Steering
{
    public class Idle : Behavior
    {
        public override void Start(BehaviorContext context)
        {
            base.Start(context);
            //initialize things for behavior here
            
        }
      

        override public Vector3 CalculateSteeringForce(float dt, BehaviorContext context)
        {
            //update target position, e.g.
            positionTarget = context.position;

            //calculate desired velocity and return steering force, e.g. 
            velocityDesired = (positionTarget - context.position).normalized * context.settings.maxDesiredVelocity;
            return velocityDesired - context.velocity;
        }

        public override void OnDrawGizmos(BehaviorContext context)
        {
            base.OnDrawGizmos(context);
            // draw things for behavior
        }
    }
}