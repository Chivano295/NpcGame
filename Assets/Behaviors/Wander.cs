
using UnityEngine;

namespace Steering
{
    public class Wander : Behaviour
    {
        public float wanderAngle;
        public override void start(BehaviorContext context)
        {
            base.start(context);
            //initialize things for behavior here
            
        }
      

        override public Vector3 CalculateSteeringForce(float dt, BehaviorContext context)
        {
            wanderAngle += Random.Range(-0.5f * context.m_settings.wanderNoiseAngle * Mathf.Deg2Rad, 0.5f * context.m_settings.wanderNoiseAngle * Mathf.Deg2Rad);

            //calculate center of the circle
            Vector3 centerOfCircle = context.m_position + context.m_velocity.normalized * context.m_settings.wanderCircleDistance;

            //calculate circle offset
            Vector3 offset = new Vector3(context.m_settings.wanderCircleRadius * Mathf.Cos(wanderAngle),
                                         0.0f,
                                         context.m_settings.wanderCircleRadius * Mathf.Sin(wanderAngle));

            //update target position plus desired velocity and return steering force
            positionTarget = centerOfCircle + offset;

            velocityDesired = (positionTarget - context.m_position) * context.m_settings.maxDesiredVelocity;


            return velocityDesired - context.m_velocity;
        }

        public override void OnDrawGizmos(BehaviorContext context)
        {
            base.OnDrawGizmos(context);
            // draw things for behavior

            //draw circle
            Vector3 centerOffCircle = context.m_position + context.m_velocity.normalized * context.m_settings.wanderCircleDistance;
            Support.DrawWireDisc(centerOffCircle, context.m_settings.wanderCircleDistance, Color.black);

            //draw noise lines
            float a = context.m_settings.wanderNoiseAngle * Mathf.Deg2Rad;
            Vector3 rangeMin = new Vector3(context.m_settings.wanderCircleRadius * Mathf.Cos(wanderAngle - 4),
                                           0.0f,
                                           context.m_settings.wanderCircleRadius * Mathf.Sin(wanderAngle - a));

            Vector3 rangeMax = new Vector3(context.m_settings.wanderCircleRadius * Mathf.Cos(wanderAngle + a),
                                           0.0f,
                                           context.m_settings.wanderCircleRadius * Mathf.Sin(wanderAngle + a));

            Debug.DrawLine(centerOffCircle, centerOffCircle + rangeMin, Color.black);
            Debug.DrawLine(centerOffCircle, centerOffCircle + rangeMax, Color.black);
        }
    }
}