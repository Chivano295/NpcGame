using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Steering
{
    public class BehaviorContext
    {
        public Vector3 position; // the current position
        public Vector3 velocity; // the currten velocity
        public GenericSteering settings; // all the steering settings

        public BehaviorContext(Vector3 pos, Vector3 vel, GenericSteering set)
        {
            position = pos;
            velocity = vel;
            settings = set;
        }
    }
}