using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Steering
{


    public interface IBehavior
    {
        /// <summary>
        /// Allow the behaviour to initialize
        /// </summary>
        /// <param name="context">All the context information needed to perform the task at hand.</param>
        void start(BehaviorContext context);

        ///<summary>
        /// Calculate the steering force contributed by this behavior
        ///</summary>
        ///<param name="dt">The deltatime for this step.</param>
        /// <returns></returns>
        Vector3 CalculateSteeringForce(float dt, BehaviorContext context);

        /// <summary>
        /// Draw the gizmos for this behavior
        /// </summary>
        /// <param name="context"> All the context information needed to perform the task at hand.</param>
        void OnDrawGizmos(BehaviorContext context);
    }
}