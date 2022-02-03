
using UnityEngine;

namespace Steering
{
    public class Evade : Behaviour
    {
        public GameObject m_target;
        public Vector3 prevTargetPos;
        public Vector3 currentTargetPos;

        public override void start(BehaviorContext context)
        {
            base.start(context);
            //initialize things for behavior here
            
        }

        public Evade(GameObject target)
        {
            m_target = target;
            
        }


        override public Vector3 CalculateSteeringForce(float dt, BehaviorContext context)
        {
            prevTargetPos = currentTargetPos;
            currentTargetPos = m_target.transform.position;

            // calculate speed
           Vector3 speed =  (currentTargetPos - prevTargetPos) / dt;
            //the calculation for the steering force
            positionTarget = (currentTargetPos + speed) * context.m_settings.lookAheadTime;
            velocityDesired = -(positionTarget - context.m_position) * context.m_settings.maxDesiredVelocity;
            return velocityDesired - context.m_velocity;
           
        }

        public override void OnDrawGizmos(BehaviorContext context)
        {
            base.OnDrawGizmos(context);
            // draw things for behavior
        }
    }
}