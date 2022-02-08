using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "sturen", menuName = "Stuurdata", order =2)]
public class GenericSteering : ScriptableObject
{
    [Header("Steering Settings")]
    public float mass = 30f; //Mass in kg's
    public float maxDesiredVelocity = 3f; // max desired velocity in m/s
    public float maxSteeringforce = 3f; //max steering force in m/s
    public float maxSpeed = 3f; //max speed in m/s

    [Header("Arrive")]
    public float arriveDistance = 1f; // distance from target when you reach zero velocity
    public float slowingDistance = 2.0f; //distance from object where we Start slowing down

    [Header("Pursue and Evade")]
    public float lookAheadTime = 1.0f;
    public float pursueRadius = 7;

    [Header("Approach")]
    public float approachRadius = 10;

    [Header("Attack")]
    public float attackRadius = 4;

    [Header("Wander")]
    public float wanderCircleDistance = 5f; //distance
    public float wanderCircleRadius = 5f;  // circle radius
    public float wanderNoiseAngle = 10f;  // how much it can differ from the last one

    [Header("Avoiding")]
    public float avoidDistance = 10;
    public float avoidMaxForce = 20;
    public string wallLayer;
    public string obstacleLayer;

    [Header("Flocking")]
    public string flockLayer;
    public float flockAlignmentWeight;
    public float flockCohesionWeight;
    public float flockAlignmentRadius;
    public float flockCohesionRadius;
    public float flockSeparationWeight;
    public float flockSeparationRadius;
    public float flockVisibilityAngle;

    [Header("Hiding")]
    public string hideLayer;
    public float hideOffset;
}
