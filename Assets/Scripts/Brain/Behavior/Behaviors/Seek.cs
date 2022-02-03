using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Steering
{
    public class Seek : Behaviour
    {
        public GameObject m_Target;
        public override void start(BehaviorContext context)
        {
            base.start(context);
            //initialize things for behavior here
        }
        public Seek(GameObject target)
        {
            m_Target = target;
        }

        override public Vector3 CalculateSteeringForce(float dt, BehaviorContext context)
        {
            
            //update target position plus desired velocity, and returning steering force
            positionTarget = m_Target.transform.position;
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