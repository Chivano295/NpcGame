using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Steering
{
    [RequireComponent(typeof(Steering))]
    public class HunterBrain : MonoBehaviour
    {
        public enum HunterState { Idle, Approach, pursue, evade }

        [Header("Target and state")]
        public GameObject target;
        public HunterState state;
        public float pursueRadius = 1f;
        public float approachRadius = 1f;

        [Header("Steering settings")]
        public GenericSteering idlesettings;
        public GenericSteering approachSettings;
        public GenericSteering PursueSettings;

        [Header("Private")]
        private Steering m_steering;
        private Animator animation;

        private void Awake()
        {
            animation = GetComponent<Animator>();
        }
        private void Start()
        {
            //get steering
            m_steering = GetComponent<Steering>();

            //start idle
            ToIdle();

            

        }

        private void FixedUpdate()
        {
            float distanceToTarget = (target.transform.position - transform.position).magnitude;
            //the finite state machine, to update animation and enum state
            switch (state)
            {
                case HunterState.Idle:
                    if (distanceToTarget < approachRadius)
                    {
                        ToApproach();
                        animation.SetInteger("enum", 1);
                        PursueSettings.maxSpeed = 2;
                    }                   
                    break;
                case HunterState.Approach:
                    if (distanceToTarget > approachRadius)
                    {
                        ToIdle();
                        animation.SetInteger("enum", 0);
                        PursueSettings.maxSpeed = 2;
                    }                      
                    else if (distanceToTarget < pursueRadius)
                    {
                        ToPursue();
                        animation.SetInteger("enum", 2);
                        PursueSettings.maxSpeed = 4;
                    }                       
                    break;
                case HunterState.pursue:
                    if (distanceToTarget > pursueRadius)
                    {
                        ToApproach();
                        animation.SetInteger("enum", 1);
                        PursueSettings.maxSpeed = 2;
                    }                    
                    break;
            }
        }
        //idle state
        private void ToIdle()
        {
            state = HunterState.Idle;
            m_steering.settings = idlesettings;

            List<IBehavior> behaviors = new List<IBehavior>();
            behaviors.Add(new Idle());
            m_steering.SetBehaviors(behaviors, "Idle");
        }
        //approach state
        private void ToApproach()
        {
            state = HunterState.Approach;
            m_steering.settings = approachSettings;

            List<IBehavior> behaviors = new List<IBehavior>();
            behaviors.Add(new Seek(target));
            m_steering.SetBehaviors(behaviors, "Approach");

        }
        //pursue state
        private void ToPursue()
        {
            state = HunterState.pursue;
            m_steering.settings = PursueSettings;

            List<IBehavior> behaviors = new List<IBehavior>();
            behaviors.Add(new Pursue(target));
            m_steering.SetBehaviors(behaviors, "Pursue");
        }
        private void OnDrawGizmos()
        {
            Support.DrawWireDisc(transform.position, approachRadius, Color.cyan);
            Support.DrawWireDisc(transform.position, pursueRadius, Color.cyan);
        }
    }
  
}