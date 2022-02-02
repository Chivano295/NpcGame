using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Steering
{
    [RequireComponent(typeof(Steering))]
    public class SimpleBrain : MonoBehaviour
    {
        //Suported behaviours

        public enum BehaviorEnum {Keyboard, Seekclickpoint, Seek, Flee, Persue, Evade, Wander, FollowPath, Hide, Arrive, NotSet}

        [Header("Manual")]
        public BehaviorEnum behavior;
        public GameObject target; // target we are working with

        [Header("Private")]
        private Steering steering; //link to steering

        [Header("Array")]
        public GameObject[] waypoints; // waypoint list

        public SimpleBrain()
        {
            behavior = BehaviorEnum.NotSet;
            target = null;
        }

        private void Start()
        {
           if(behavior == BehaviorEnum.Keyboard || behavior == BehaviorEnum.Seekclickpoint)
            {
                target = null;
            }
           else
            {
                if (target == null)
                    target = GameObject.Find("Player");
                if (target == null)
                    target = GameObject.Find("Target");
            }

           //Get steering
            steering = GetComponent<Steering>();

            // configure steering
            List<IBehavior> behaviors = new List<IBehavior>();
            switch (behavior)
            {
                case BehaviorEnum.Keyboard:
                    behaviors.Add(new Keyboard());
                    steering.SetBehaviors(behaviors, "Keyboard");
                    break;

                

                case BehaviorEnum.Seek:
                    behaviors.Add(new Seek(target));
                    steering.SetBehaviors(behaviors, "Seek");
                    break;

                case BehaviorEnum.Flee:
                    behaviors.Add(new Flee());
                    steering.SetBehaviors(behaviors, "Flee");
                    break;

                case BehaviorEnum.FollowPath:
                    behaviors.Add(new FollowPath(waypoints));
                    steering.SetBehaviors(behaviors, "Followpath");
                    break;
                case BehaviorEnum.Arrive:
                    behaviors.Add(new Arrive(target));
                    steering.SetBehaviors(behaviors, "Arrive");
                    break;
                case BehaviorEnum.Persue:
                    behaviors.Add(new Pursue(target));
                    steering.SetBehaviors(behaviors, "Pursue");
                    break;
                case BehaviorEnum.Evade:
                    behaviors.Add(new Evade(target));
                    steering.SetBehaviors(behaviors, "Evade");
                    break;
                case BehaviorEnum.Wander:
                    behaviors.Add(new Wander());
                    steering.SetBehaviors(behaviors, "Wander");
                    break;

                default:
                    Debug.LogError($"Behavior of type {behavior} not implemented yet");
                    break;

            }

        }
    }
}
