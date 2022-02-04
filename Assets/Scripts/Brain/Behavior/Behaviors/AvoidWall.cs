using UnityEngine;

namespace Steering
{
    public class AvoidWall : Behavior 
    {
        private int     wallLayer; // the active obstacle layer mask

        // info used to draw gizmos (or not)
        private bool    doAvoidObject; // true if we are walling an obstacle
        private Vector3 hitPoint;      // raycast results: the current hit point

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        override public void Start(BehaviorContext context)
        {
            base.Start(context);          

            // get layer mask using the wall layer name
            wallLayer = LayerMask.GetMask(context.settings.wallLayer);
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        override public Vector3 CalculateSteeringForce(float dt, BehaviorContext context)
        {
            // construct ray from position and velocity and check for the nearest collider
            Ray ray         = new Ray(context.position, context.velocity);
            doAvoidObject = Physics.Raycast(ray, out RaycastHit hit, context.settings.avoidDistance, wallLayer, QueryTriggerInteraction.Ignore);

            // return zero steering force if no collider found
            if (!doAvoidObject)
                return Vector3.zero;

            // remember hit point for drawing gizmos
            hitPoint = hit.point;

            // calculate desired velocity: hit normal * wallMaxForce
            velocityDesired = hit.normal * context.settings.avoidMaxForce;

            // make sure desired velocity and velocity are not aligned
            float angle = Vector3.Angle(velocityDesired.With(y:0), context.velocity.With(y:0));

            if (angle > 179.0f)
                velocityDesired = Vector3.Cross(Vector3.up, context.velocity).normalized * context.settings.avoidMaxForce;

            // update target position and return steering force  
            positionTarget = context.position + velocityDesired; // fake target just used to draw gizmos!
            return velocityDesired - context.velocity;
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        override public void OnDrawGizmos(BehaviorContext context)
        {
            // do nothing if we are disabled
            if (context.settings.avoidMaxForce <= 0.0f)
                return;

            // draw ray from current position to sensor position
            Support.DrawRay(context.position, 
                               context.velocity.normalized * context.settings.avoidDistance, 
                               doAvoidObject ? Color.black : Color.grey);

            // draw feedback on collision in own colors
            if (doAvoidObject)
            {
                Support.DrawWireDisc(hitPoint, 0.25f            , Color.black);
                Support.DrawRay     (hitPoint, velocityDesired, Color.green);
            }
        }
    }
}
