
using System.Collections.Generic;
using UnityEngine;

namespace Steering
{
    using BehaviorList = List<IBehavior>;
    public class Steering : MonoBehaviour
    {
        [Header("Steering settings")]
        public string m_label; // label to show when running
        public GenericSteering settings; // de steering settings for all behaviour

        [Header("Steering runtime")]
        public Vector3 position = Vector3.zero;// current position       
        public Vector3 velocity = Vector3.zero; // current velocity      
        public Vector3 steering = Vector3.zero; //steering force
        public BehaviorList m_behaviors = new BehaviorList(); // all behaviors for this steering object
        void Start()
        {
            position = transform.position;
        }

        private void FixedUpdate()
        {
            //Steering general: calculate steering force
            steering = Vector3.zero;
           foreach(IBehavior behavior in m_behaviors)
            {
                steering += behavior.CalculateSteeringForce(Time.fixedDeltaTime, new BehaviorContext(position, velocity, settings));
            }
            // make sure Y is fixxed its only done on the XZ axis now
            steering.y = 0f;
            //Steering general: clamp steering force to maximum steering force and apply mass
            steering = Vector3.ClampMagnitude(steering, settings.maxSteeringforce);
            steering /= settings.mass;
            //Steering general: Update velocity with steering force, and update position
            velocity = Vector3.ClampMagnitude(velocity + steering, settings.maxSpeed);
            position += velocity * Time.fixedDeltaTime;
            //update object with new position
            transform.position = position;
            transform.LookAt(position + Time.fixedDeltaTime * velocity);
        }

        private void OnDrawGizmos()
        {
            Support.DrawRay(transform.position, velocity, Color.red);
            Support.DrawLabel(transform.position, m_label, Color.white);

            foreach(IBehavior behavior in m_behaviors)
            {
                behavior.OnDrawGizmos(new BehaviorContext(position, velocity, settings));
            }
        }

        public void SetBehaviors(BehaviorList behaviors, string label = "")
        {
            //remember new settings
            m_label = label;
            m_behaviors = behaviors;

            //start all behaviours
            foreach(IBehavior behavior in m_behaviors)
            {
                behavior.start(new BehaviorContext(position, velocity, settings));
            }

                
        }

    }
}