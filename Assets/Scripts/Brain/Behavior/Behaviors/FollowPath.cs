using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Steering
{
    public class FollowPath : Behavior
    {
        private GameObject[] waypointList;
        private int waypointIndex = 0;
        private float waypointRadius = 2.5f;

        public FollowPath(GameObject[] waypoints)
        {
            waypointList = waypoints;
        }
        public override void Start(BehaviorContext context)
        {
            base.Start(context);
            //initialize things for behavior here
        }


        override public Vector3 CalculateSteeringForce(float dt, BehaviorContext context)
        {
           

            // calculate distance to waypoint
            float _distanceToWaypoint = (waypointList[waypointIndex].transform.position - context.position).magnitude;

            // update waypoint index if object is within radius
            if (_distanceToWaypoint < waypointRadius)
            {
                if (waypointIndex < waypointList.Length - 1)
                {
                    ++waypointIndex;
                }
                else
                {
                    waypointIndex = 0;
                }
            }

            positionTarget = waypointList[waypointIndex].transform.position;
            //update target position plus desired velocity, and returning steering force

            velocityDesired = (positionTarget - context.position).normalized * context.settings.maxDesiredVelocity;
            return velocityDesired - context.velocity;
        }

        public override void OnDrawGizmos(BehaviorContext context)
        {
            base.OnDrawGizmos(context);
            // draw things for behavior
            // draw lines between waypoints
            GameObject _previousWP = null;
            foreach (var _currentWP in waypointList)
            {
                // draw circle
                Support.DrawWireDisc(_currentWP.transform.position, waypointRadius, Color.black);

                // draw line between waypoints
                if (_previousWP != null)
                    Debug.DrawLine(_previousWP.transform.position, _currentWP.transform.position, Color.black);

                // update previous waypoint with the current one
                _previousWP = _currentWP;
            }
            //met grote hulp van milos
        }
    }
}