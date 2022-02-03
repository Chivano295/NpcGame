using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Steering
{
    public abstract class Behaviour : IBehavior
    {

        [Header("Behaviour runtime")]
        public Vector3 positionTarget = Vector3.zero; // target position
        public Vector3 velocityDesired = Vector3.zero; // desired velocity


        public virtual void start(BehaviorContext context)
        {
            positionTarget = context.m_position;
        }
        //used to transfer variables to other script
        public abstract Vector3 CalculateSteeringForce(float dt, BehaviorContext context);
        public virtual void OnDrawGizmos(BehaviorContext context)
        {
            Support.DrawRay(context.m_position, velocityDesired, Color.red);
        }
    }
}