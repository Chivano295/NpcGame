using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Steering
{
    public class Flee : Behavior
    {
        private GameObject target;

        public override void Start(BehaviorContext context)
        {
            base.Start(context);
            //initialize things for behavior here
        }

        public Flee(GameObject targ)
        {
            target = targ;
        }

        override public Vector3 CalculateSteeringForce(float dt, BehaviorContext context)
        {
            //update target position plus desired velocity, and returning steering force
            if (!target)
                return velocityDesired - context.velocity;

            positionTarget = target.transform.position;
            velocityDesired = -(positionTarget - context.position).normalized * context.settings.maxDesiredVelocity;
            return velocityDesired - context.velocity;
        }

        public override void OnDrawGizmos(BehaviorContext context)
        {
            base.OnDrawGizmos(context);
            // draw things for behavior
        }
    }
}