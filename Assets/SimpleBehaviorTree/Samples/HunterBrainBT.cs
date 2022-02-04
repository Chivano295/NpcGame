﻿using System.Collections.Generic;
using UnityEngine;

namespace SimpleBehaviorTree.Examples
{
    /// <summary>
    /// The blackboard class that passes on information to all nodes in the behavior tree during the update.
    /// </summary>
    class HunterBlackboard : Blackboard
    {
        public float m_distanceToTarget = 0.0f;
    }

    /// <summary>
    /// Hunter brain class using behavior tree. Also implements debug feedback (dumping tree and reporting status updates)
    /// </summary>
    public class HunterBrainBT : MonoBehaviour
    {
        public enum BuildOption { Tree1, Tree2 }

        [SerializeField] 
        private BehaviorTree     m_tree;                                // the behavior tree

        [Header("Target")]
        public GameObject        m_target;                              // our target object
        public float             m_pursueRadius    =  7.0f;             // the pursue radius in m
        public float             m_approachRadius  = 10.0f;             // the approach radius in m (must be larger than pursue radius)

        [Header("Config")]
        public BuildOption       m_buildOption     = BuildOption.Tree1; // which behavior tree do we use?

        [Header("Steering Settings")]                                        
        public float             m_approachSpeed   =  1.0f;             // the approach speed in m/s  
        public float             m_pursueSpeed     =  2.0f;             // the pursue speed in m/s
        public float             m_rotationSpeed   = 10.0f;             // rotaton speed in degrees/s
        public bool              m_doNotMove       = true;              // set to true to prevent the NPC from moving (debug option)

        [Header("Feedback")]
        private float            m_activeSpeed     = 0.0f;              // the active speed in m/s
        private string           m_state           = "-";               // string that provides feedback on the current state

        [Header("Private")]
        private HunterBlackboard m_blackboard;                          // the blackboard used to pass info to the behavior tree during updates
        
        //------------------------------------------------------------------------------------------
        // Unity overrides
        //------------------------------------------------------------------------------------------
        private void Start()
        {
            // init blackboard
            m_blackboard = new HunterBlackboard();

            // prepare behavior tree
            switch (m_buildOption)
            {
                case BuildOption.Tree1: m_tree = new BehaviorTree(BuildTree1(), m_blackboard, BlackboardUpdater) { Name = "Tree1" }; break;
                case BuildOption.Tree2: m_tree = new BehaviorTree(BuildTree2(), m_blackboard, BlackboardUpdater) { Name = "Tree2" }; break;
            }
        }

        void Update()
        {
            // do not move if requested
            if (!m_doNotMove)
            {
                // calculate target direction and desired velocity
                Vector3 targetDirection = m_target.transform.position - transform.position;
                Vector3 desiredVelocity = targetDirection.normalized * m_activeSpeed;

                // update position and rotation
                transform.position    = transform.position + desiredVelocity * Time.deltaTime;
                transform.rotation    = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDirection), m_rotationSpeed * Time.deltaTime);
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            }

            // update the behavior tree
            m_tree.Update(Time.deltaTime);
        }

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            UnityEditor.Handles.color = Color.cyan;
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, m_approachRadius);
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, m_pursueRadius);
            UnityEditor.Handles.BeginGUI();
            UnityEditor.Handles.Label       (transform.position, $"{m_state} (speed = {m_activeSpeed})");
            UnityEditor.Handles.EndGUI();
#endif
        }

        //------------------------------------------------------------------------------------------
        // Methods that create a behavior tree
        //------------------------------------------------------------------------------------------

		/// <summary>
        /// Build a Hunter Brain behavior tree using the BehaviorTreeBuilder.
        /// </summary>
        /// <returns>A Hunter Brain behavior tree.</returns>
        private RootNode BuildTree1()
        {
            return new BehaviorTreeBuilder()
                .Name("HunterBrain")
                .Selector("MainSelector")
                    .Sequence("Approach")
                        .Condition("InApproachRangeOnly", InApproachRangeOnly)
                        .Do       ("ToApproach"         , ToApproach)
                        .RepeatUntilFailure("RepeatUntilFailure")
                            .Condition("InApproachRangeOnly", InApproachRangeOnly)
                        .End()
                    .End()
                    .Sequence("Pursue")
                        .Condition("InPursueRange", InPursueRange)
                        .Do       ("ToPursue"     , ToPursue)
                        .RepeatUntilFailure("RepeatUntilFailure")
                            .Condition("InPursueRange", InPursueRange)
                        .End()
                    .End()
                    .Sequence("Idle")
                        .Do("ToIdle", ToIdle)
                        .RepeatUntilSuccess("RepeatUntilSuccess")
                            .Condition("InApproachRange", InApproachRange)
                        .End()
                    .End()
                .End()
                .Build();
        }
        
        /// <summary>
        /// Build an alternate Hunter Brain behavior tree using the BehaviorTreeBuilder.
        /// </summary>
        /// <returns>A Hunter Brain behavior tree.</returns>
        private RootNode BuildTree2()
        {
            return new BehaviorTreeBuilder()
                .Name("HunterBrain")
                .Selector("MainSelector")
                    .Sequence("PursueOrApproach")
                        .Condition("InApproachRange", InApproachRange)
                        .Selector()
                            .Sequence("Pursue")
                                .Condition("InPursueRange", InPursueRange)
                                .Do       ("ToPursue"     , ToPursue)
                                .RepeatUntilFailure("RepeatUntilFailure")
                                    .Condition("InPursueRange", InPursueRange)
                                .End()
                            .End()
                            .Sequence("Approach")
                                .Do("ToApproach", ToApproach)
                                .RepeatUntilFailure("RepeatUntilFailure")
                                    .Sequence()
                                        .Inverter()
                                            .Condition("InPursueRange", InPursueRange)
                                        .End()
                                        .Condition("InApproachRange", InApproachRange)
                                    .End()
                                .End()
                            .End()
                        .End()
                    .End()
                    .Sequence("Idle")
                        .Do("ToIdle", ToIdle)
                        .RepeatUntilSuccess("RepeatUntilSuccess")
                            .Condition("InApproachRange", InApproachRange)
                        .End()
                    .End()
                .End()
                .Build();
        }

        /// <summary>
        /// Method that updates out blackboard before each update.
        /// </summary>
        /// <param name="bb">The blackboard to update.</param>
        private void BlackboardUpdater(Blackboard bb)
        {
            // update distance to target
            (m_blackboard as HunterBlackboard).m_distanceToTarget = (m_target.transform.position - transform.position).magnitude;
        }

        //------------------------------------------------------------------------------------------
        // All methods used by the behavior tree
        //------------------------------------------------------------------------------------------

        #region Action methods linked to the behavior tree
        /// <summary>
        /// Check wether the target is further away than the pursue range but still within the approach range.
        /// </summary>
        /// <param name="bb">Our blackboard with an updated distance to target.</param>
        /// <returns>True if the target is further away than the pursue range but still within the approach range.</returns>
        private bool InApproachRangeOnly(Blackboard bb)
        {
            return !InPursueRange(bb) && InApproachRange(bb);
        }

        /// <summary>
        /// Check wether the target is within approach range.
        /// </summary>
        /// <param name="bb">Our blackboard with an updated distance to target.</param>
        /// <returns>True if the target is within approach range.</returns>
        private bool InApproachRange(Blackboard bb)
        {
            return (bb as HunterBlackboard).m_distanceToTarget < m_approachRadius;
        }

        /// <summary>
        /// Check wether the target is within pursue range.
        /// </summary>
        /// <param name="bb">Our blackboard with an updated distance to target.</param>
        /// <returns>True if the target is within pursue range.</returns>
        private bool InPursueRange(Blackboard bb)
        {
            return (bb as HunterBlackboard).m_distanceToTarget < m_pursueRadius;
        }

        /// <summary>
        /// Switch to approach modus.
        /// </summary>
        /// <param name="bb">Our blackboard.</param>
        /// <returns>Success if the action succeeded.</returns>
        private NodeState ToApproach(Blackboard bb)
        {
            m_state       = "Approach";
            m_activeSpeed = m_approachSpeed;
            return NodeState.SUCCESS;
        }

        /// <summary>
        /// Switch to pursue modus.
        /// </summary>
        /// <param name="bb">Our blackboard.</param>
        /// <returns>Success if the action succeeded.</returns>
        private NodeState ToPursue(Blackboard bb)
        {
            m_state       = "Pursue";
            m_activeSpeed = m_pursueSpeed;
            return NodeState.SUCCESS;
        }

        /// <summary>
        /// Switch to idle modus.
        /// </summary>
        /// <param name="bb">Our blackboard.</param>
        /// <returns>Success if the action succeeded.</returns>
        private NodeState ToIdle(Blackboard bb)
        {
            m_state       = "Idle";
            m_activeSpeed = 0.0f;
            return NodeState.SUCCESS;
        }
        #endregion
    }
}