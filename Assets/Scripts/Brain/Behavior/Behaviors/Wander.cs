
using UnityEngine;

namespace Steering
{
    public class Wander : Behavior
    {
        public float wanderAngle;
        public override void Start(BehaviorContext context)
        {
            base.Start(context);
            //initialize things for behavior here
        }

        public Wander(Transform transform)
        {
            wanderAngle = Vector3.SignedAngle(transform.forward, Vector3.right, Vector3.up) * Mathf.Deg2Rad;
        }
      

        override public Vector3 CalculateSteeringForce(float dt, BehaviorContext context)
        {
            wanderAngle += Random.Range(-0.5f * context.settings.wanderNoiseAngle * Mathf.Deg2Rad, 0.5f * context.settings.wanderNoiseAngle * Mathf.Deg2Rad);

            //calculate center of the circle
            Vector3 centerOfCircle = context.position + context.velocity.normalized * context.settings.wanderCircleDistance;

            //calculate circle offset
            Vector3 offset = new Vector3(context.settings.wanderCircleRadius * Mathf.Cos(wanderAngle),
                                         0.0f,
                                         context.settings.wanderCircleRadius * Mathf.Sin(wanderAngle));

            //update target position plus desired velocity and return steering force
            positionTarget = centerOfCircle + offset;

            velocityDesired = (positionTarget - context.position) * context.settings.maxDesiredVelocity;


            return velocityDesired - context.velocity;
        }

        public override void OnDrawGizmos(BehaviorContext context)
        {
            base.OnDrawGizmos(context);
            // draw things for behavior

            GenericSteering settings = context.settings; // saves settings for optimazation

            //draw circle
            float a = settings.wanderNoiseAngle * Mathf.Deg2Rad;
            // Geen idee wat hier gebeurt :/ (is van video)
            Vector3 rangeMin = new Vector3(settings.wanderCircleRadius * Mathf.Cos(wanderAngle - a), 0, settings.wanderCircleRadius * Mathf.Sin(wanderAngle - a));
            Vector3 rangeMax = new Vector3(settings.wanderCircleRadius * Mathf.Cos(wanderAngle + a), 0, settings.wanderCircleRadius * Mathf.Sin(wanderAngle + a));
            Vector3 centerOfCircle = context.position + context.velocity.normalized * settings.wanderCircleDistance;

            // Draws outter circle & targeted position
            Support.DrawWireDisc(centerOfCircle, settings.wanderCircleRadius, Color.black);
            Support.DrawSolidDisc(positionTarget, 0.25f, Color.red);

            // Draws inner 2 lines
            Support.DrawLine(centerOfCircle, centerOfCircle + rangeMin, Color.black);
            Support.DrawLine(centerOfCircle, centerOfCircle + rangeMax, Color.black);

            // Drwas from ai position to center of circle / towards velocity
            Support.DrawLine(context.position, centerOfCircle, Color.white);
            Support.DrawRay(context.position, velocityDesired, Color.blue);
        }
    }
}