using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Steering
{

    public class Keyboard : Behaviour
    {
       override public Vector3 CalculateSteeringForce(float dt, BehaviorContext context)
        {
            //get requested direction from input
            Vector3 requested_direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

            if (requested_direction != Vector3.zero)
                positionTarget = context.m_position + requested_direction.normalized * context.m_settings.maxDesiredVelocity;
            else
                positionTarget = context.m_position;

            //calculate desired velocity and return the steering force
            velocityDesired = (positionTarget - context.m_position).normalized * context.m_settings.maxDesiredVelocity;
            return velocityDesired - context.m_velocity;
        }
    }
}