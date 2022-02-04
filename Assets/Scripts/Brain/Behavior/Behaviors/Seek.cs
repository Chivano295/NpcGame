using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Steering
{
    public class Seek : Behavior
    {
        public GameObject Target;
        public override void Start(BehaviorContext context)
        {
            base.Start(context);
            //initialize things for behavior here
        }
        public Seek(GameObject target)
        {
            Target = target;
        }

        override public Vector3 CalculateSteeringForce(float dt, BehaviorContext context)
        {
            
            //update target position plus desired velocity, and returning steering force
            positionTarget = Target.transform.position;
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