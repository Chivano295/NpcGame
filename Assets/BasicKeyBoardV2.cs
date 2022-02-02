
using UnityEngine;

namespace SimpleSteering
{
    public class BasicKeyBoardV2 : MonoBehaviour
    {
        [Header("Steering settings")]
        public SteeringSettings settings; // de steering settings for this behaviour

        [Header("Steering runtime")]
        public Vector3 position = Vector3.zero;// current position
        public Vector3 positionTarget = Vector3.zero; // Position of target
        public Vector3 velocity = Vector3.zero; // current velocity
        public Vector3 velocityDesired = Vector3.zero; // The desired velocity
        public Vector3 steering = Vector3.zero; //steering force
                                                // Start is called before the first frame update
        void Start()
        {
            position = transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            //keyboard specific: get requested direction from input
            Vector3 Requested_Direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical"));

            //keyboard specific: determine target position
            if (Requested_Direction != Vector3.zero)
                positionTarget = position + Requested_Direction.normalized * settings.maxDesiredVelocity;
            else
                positionTarget = position;
        }
        private void FixedUpdate()
        {
            //steering: calculate desired velocity and steering force (normaal: Bereken de gap van waar je nu kijkt en wilt gaan kijken, vervolgens verminder je die gap steeds)
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
            //draws gizmos
            Support.DrawRay(transform.position, velocity, Color.red);
            Support.DrawRay(transform.position, velocityDesired, Color.blue);
            Support.DrawLabel(transform.position, name, Color.white);
        }

    }
}