
using UnityEngine;

namespace SimpleSteering
{
    public class SimpleSeek : MonoBehaviour
    {
        [Header("Steering settings")]
        public SteeringSettings settings; // de steering settings for this behaviour

        [Header("Steering runtime")]
        public Vector3 position = Vector3.zero;// current position
        public Vector3 positionTarget = Vector3.zero; // Position of target
        public Vector3 velocity = Vector3.zero; // current velocity
        public Vector3 velocityDesired = Vector3.zero; // The desired velocity
        public Vector3 steering = Vector3.zero; //steering force

        public GameObject target;
        void Start()
        {
            position = transform.position;
            positionTarget = position;
        }

        // Update is called once per frame
        void Update()
        {                                    
            positionTarget = target.transform.position;
            positionTarget.y = position.y; // make sure to stay at the same level                           
        }
        private void FixedUpdate()
        {
            //steering: calculate desired velocity and steering force 
            velocityDesired = (positionTarget - position).normalized * settings.maxDesiredVelocity;
            Vector3 steering_force = velocityDesired - velocity;

            //Steering general: calculate steering force
            steering = Vector3.zero;
            steering += steering_force;
            //Steering general: clamp steering force to maximum steering force and apply mass
            steering = Vector3.ClampMagnitude(steering_force, settings.maxSteeringForce);
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
            Support.DrawRay(transform.position, velocityDesired, Color.blue);
            Support.DrawLabel(transform.position, name, Color.white);

            UnityEditor.Handles.color = Color.cyan;
            UnityEditor.Handles.DrawSolidDisc(positionTarget, Vector3.up, 0.25f);
        }

    }
}