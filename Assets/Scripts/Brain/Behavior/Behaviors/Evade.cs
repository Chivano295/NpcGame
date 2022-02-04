
using UnityEngine;

namespace Steering
{
    public class Evade : Behavior
    {
        public GameObject target;
        public Vector3 prevTargetPos;
        public Vector3 currentTargetPos;

        public override void Start(BehaviorContext context)
        {
            base.Start(context);
            //initialize things for behavior here
            
        }

        public Evade(GameObject target)
        {
            target = target;
            
        }


        override public Vector3 CalculateSteeringForce(float dt, BehaviorContext context)
        {
            prevTargetPos = currentTargetPos;
            currentTargetPos = target.transform.position;

            // calculate speed
           Vector3 speed =  (currentTargetPos - prevTargetPos) / dt;
            //the calculation for the steering force
            positionTarget = (currentTargetPos + speed) * context.settings.lookAheadTime;
            velocityDesired = -(positionTarget - context.position) * context.settings.maxDesiredVelocity;
            return velocityDesired - context.velocity;
           
        }

        public override void OnDrawGizmos(BehaviorContext context)
        {
            base.OnDrawGizmos(context);
            // draw things for behavior
        }
    }
}