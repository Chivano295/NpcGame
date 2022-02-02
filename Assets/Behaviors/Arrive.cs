
using UnityEngine;

namespace Steering
{
    public class Arrive : Behaviour
    {
        public GameObject m_Target;
        public override void start(BehaviorContext context)
        {
            base.start(context);
            //initialize things for behavior here
            
        }
      public Arrive(GameObject target)
        {
            m_Target = target;
        }
        override public Vector3 CalculateSteeringForce(float dt, BehaviorContext context)
        {
            //update target position, e.g.
            positionTarget = m_Target.transform.position;
            positionTarget.y = context.m_position.y;

            //calculate stop offset
            Vector3 stopVector = (context.m_position - positionTarget).normalized * context.m_settings.arriveDistance;
            Vector3 stopPosition = positionTarget + stopVector;

            //how many meters to go until stopping
            Vector3 targetOffset = stopPosition - context.m_position;
            float distance = targetOffset.magnitude;

            // calculate ramped speed ramped/clipped speed
            float rampedSpeed = context.m_settings.maxDesiredVelocity * (distance / context.m_settings.slowingDistance);
            float clippedSpeed = Mathf.Min(rampedSpeed, context.m_settings.maxDesiredVelocity);

            //update desired velocity and return force
            if (distance > 0.001f)
            {
                velocityDesired = (clippedSpeed / distance) * targetOffset;
            }
            else
                velocityDesired = Vector3.zero;
            return velocityDesired - context.m_velocity;
            
        }

        public override void OnDrawGizmos(BehaviorContext context)
        {
            base.OnDrawGizmos(context);
            // draw things for behavior
            Support.DrawWireDisc(m_Target.transform.position, context.m_settings.arriveDistance, Color.yellow);
            Support.DrawWireDisc(m_Target.transform.position, context.m_settings.slowingDistance, Color.yellow);
        }
    }
}