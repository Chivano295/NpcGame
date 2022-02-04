using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Steering
{
    [RequireComponent(typeof(Steering))]
    public class AgentBrain : MonoBehaviour
    {
        enum AgentBehaviours { Aggressive, Defensive, Loyal, Wanderer, GuardPathA, GuardPathB };


        private Steering steering;

        private void Awake()
        {
            steering = GetComponent<Steering>();
        }

        void SetBehavior(IBehavior[] behavior, string label)
        {
            List<IBehavior> behaviors = new List<IBehavior>();

            for (int i = 0; i < behavior.Length; i++)
                behaviors.Add(behavior[i]);

            steering.SetBehaviors(behaviors, label);
        }


        public void MoveTo(Vector3 position)
        {
            SetBehavior(new IBehavior[] { new Wander(this.transform), new AvoidObstacle() }, "MoveTo");
        }

        private void Start()
        {
            MoveTo(new Vector3(10, 0, 10));
        }
    }
}