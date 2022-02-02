
using UnityEngine;

namespace SimpleSteering
{
    [CreateAssetMenu(fileName = "Steering settings", menuName = "Steering/Steering settings", order = 1)]
    public class SteeringSettings : ScriptableObject
    {

      
        [Header("Steering settings")]
        public float mass = 70f; // mass in kg
        public float maxDesiredVelocity = 3f; // max desired velocity in m/s
        public float maxSteeringForce = 3f;  // max Steering force  in m/s
        public float maxSpeed = 3f; // max vehicle speed

        [Header("Arrive")]
        public float arriveDistance;

        

    }
}