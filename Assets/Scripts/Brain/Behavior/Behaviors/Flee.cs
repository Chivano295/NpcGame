using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Steering
{
    public class Flee : Behavior
    {
        
        public override void Start(BehaviorContext context)
        {
            base.Start(context);
            //initialize things for behavior here
        }


        override public Vector3 CalculateSteeringForce(float dt, BehaviorContext context)
        {
            GameObject _player = GameObject.FindGameObjectWithTag("Player");
            //update target position plus desired velocity, and returning steering force
            positionTarget = _player.transform.position;
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