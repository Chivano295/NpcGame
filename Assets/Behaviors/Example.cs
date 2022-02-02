
using UnityEngine;

namespace Steering
{
    public class Example : Behaviour
    {
        public override void start(BehaviorContext context)
        {
            base.start(context);
            //initialize things for behavior here
            
        }
      

        override public Vector3 CalculateSteeringForce(float dt, BehaviorContext context)
        {
            //update target position, e.g.
            positionTarget = context.m_position;

            //calculate desired velocity and return steering force, e.g. 
            velocityDesired = (positionTarget - context.m_position).normalized * context.m_settings.maxDesiredVelocity;
            return velocityDesired - context.m_velocity;
        }

        public override void OnDrawGizmos(BehaviorContext context)
        {
            base.OnDrawGizmos(context);
            // draw things for behavior
        }
    }
}