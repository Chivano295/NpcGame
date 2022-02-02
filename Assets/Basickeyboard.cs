
using UnityEngine;

namespace SimpleSteering
{
    public class Basickeyboard : MonoBehaviour
    {

        [Header("Steeringsettins")]
        public float maxSpeed = 3f;

        [Header("Steering runtime")]

        public Vector3 position = Vector3.zero;
        public Vector3 velocity = Vector3.zero;

        private void Start()
        {
            position = transform.position;
        }
        void FixedUpdate()
        {
            //get requested direction from input
            Vector3 Requested_direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical"));

            //calculate desired velocity and steering force for this behavior
            velocity = Requested_direction * maxSpeed;
            position = position + velocity * Time.fixedDeltaTime;

            //update object with position and lookdir
            transform.position = position;
            transform.LookAt(position + velocity.normalized);

            

        }

        private void OnDrawGizmos()
        {
            //draws gizmos
            Support.DrawRay(transform.position, velocity, Color.red);
            Support.DrawLabel(transform.position, name, Color.red);
        }

     
    }
}