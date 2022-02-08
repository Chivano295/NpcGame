using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Steering;
using SimpleBehaviorTree.Examples;

namespace SimpleBehaviorTree.Examples
{
    class HunterBlackboardBrain : Blackboard
    {
        public float distanceToTarget = 0.0f;
    }

    public class AgentBrain : MonoBehaviour
    {
        [SerializeField]
        private BehaviorTree tree;                    // the behavior tree

        [Header("Target")]
        public GameObject target;                  // our target object
        public float pursueRadius = 7.0f; // the pursue radius in m
        public float approachRadius = 10.0f; // the approach radius in m (must be larger than pursue radius)
        public float attackRadius = 2;

        [Header("Steering Settings")]
        public float approachSpeed = 1.0f; // the approach speed in m/s  
        public float pursueSpeed = 2.0f; // the pursue speed in m/s
        public float followSpeed = 1.5f;
        public float rotationSpeed = 10.0f; // rotaton speed in degrees/s
        public bool doNotMove = true;  // set to true to prevent the NPC from moving (debug option)

        [Header("Feedback")]
        private float activeSpeed = 0.0f;  // the active speed in m/s
        private string state = "-";   // string that provides feedback on the current state

        [Header("Private")]
        private HunterBlackboard blackboard;              // the blackboard used to pass info to the behavior tree during updates

        //------------------------------------------------------------------------------------------
        // Unity overrides
        //------------------------------------------------------------------------------------------
        private void Start()
        {
            // init blackboard
            blackboard = new HunterBlackboard();

            // prepare behavior tree
            tree = new BehaviorTree(BuildTree1(), blackboard, BlackboardUpdater) { Name = "SimpleTree" };
        }

        void Update()
        {
            // calculate target direction and desired velocity
            Vector3 targetDirection = target.transform.position - transform.position;
            Vector3 desiredVelocity = targetDirection.normalized * activeSpeed;

            // update position and rotation
            transform.position = transform.position + desiredVelocity * Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDirection), rotationSpeed * Time.deltaTime);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

            // update the behavior tree
            tree.Update(Time.deltaTime);
        }

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            UnityEditor.Handles.color = Color.cyan;
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, approachRadius);
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, pursueRadius);
            UnityEditor.Handles.BeginGUI();
            UnityEditor.Handles.Label(transform.position, $"{state} (speed = {activeSpeed})");
            UnityEditor.Handles.EndGUI();
#endif
        }

        //------------------------------------------------------------------------------------------
        // Method that create a behavior tree
        //------------------------------------------------------------------------------------------

        private RootNode BuildTree1()
        {
            return new BehaviorTreeBuilder()
                .Name("AgentBrain")
                .Selector("MainSelector")
                    .Sequence("Attack")
                        .Condition("InAttackRangeOnly", InAttackRange)
                        .Do("ToAttack", ToAttack)
                        .RepeatUntilFailure("RepeatUntilFailure")
                            .Condition("InAttackRangeOnly", InAttackRange)
                        .End()
                    .End()

                    .Sequence("Approach")
                        .Condition("InApproachRangeOnly", InApproachRangeOnly)
                        .Do("ToApproach", ToApproach)
                        .RepeatUntilFailure("RepeatUntilFailure")
                            .Condition("InApproachRangeOnly", InApproachRangeOnly)
                        .End()
                    .End()

                    .Sequence("Pursue")
                        .Condition("InPursueRange", InPursueRangeOnly)
                        .Do("ToPursue", ToPursue)
                        .RepeatUntilFailure("RepeatUntilFailure")
                            .Condition("InPursueRange", InPursueRangeOnly)
                        .End()
                    .End()

                    .Sequence("Idle")
                        .Do("ToIdle", ToIdle)
                        .RepeatUntilSuccess("RepeatUntilSuccess")
                            .Condition("InApproachRange", InApproachRange)
                        .End()
                    .End()

                    .Sequence("Follow Path")
                        .Condition("CanFollowPath", CanFollowPath)
                        .Do("ToFollowPath", ToFollowPath)
                        .RepeatUntilFailure("RepeatUntilFailure")
                            .Condition("CanFollowPath", CanFollowPath)
                        .End()
                    .End()
                .Build();
        }

        private void BlackboardUpdater(Blackboard bb)
        {
            // update distance to target
            (blackboard as HunterBlackboard).distanceToTarget = (target.transform.position - transform.position).magnitude;
        }

        //------------------------------------------------------------------------------------------
        // All methods used by the behavior tree
        //------------------------------------------------------------------------------------------

        #region Action methods linked to the behavior tree
        private bool InApproachRangeOnly(Blackboard bb)
        {
            return !InPursueRange(bb) && InApproachRange(bb);
        }

        private bool InApproachRange(Blackboard bb)
        {
            return (bb as HunterBlackboard).distanceToTarget < approachRadius;
        }

        private bool InPursueRangeOnly(Blackboard bb)
        {
            return !InAttackRange(bb) && InPursueRange(bb);
        }

        private bool InPursueRange(Blackboard bb)
        {
            return (bb as HunterBlackboard).distanceToTarget < pursueRadius;
        }

        private bool InAttackRange(Blackboard bb)
        {
            return (bb as HunterBlackboard).distanceToTarget < attackRadius;
        }

        private bool CanFollowPath(Blackboard bb)
        {
            return true;
        }


        private NodeState ToApproach(Blackboard bb)
        {
            state = "Approach";
            activeSpeed = approachSpeed;
            return NodeState.SUCCESS;
        }

        private NodeState ToPursue(Blackboard bb)
        {
            state = "Pursue";
            activeSpeed = pursueSpeed;
            return NodeState.SUCCESS;
        }

        private NodeState ToIdle(Blackboard bb)
        {
            state = "Idle";
            activeSpeed = 0.0f;
            return NodeState.SUCCESS;
        }

        private NodeState ToAttack(Blackboard bb)
        {
            state = "Attack";
            activeSpeed = 0;
            return NodeState.SUCCESS;
        }

        private NodeState ToFollowPath(Blackboard bb)
        {
            state = "FollowPath";
            activeSpeed = followSpeed;
            return NodeState.SUCCESS;
        }
        #endregion
    }
}