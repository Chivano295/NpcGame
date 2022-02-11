using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Steering;


namespace SimpleBehaviorTree.Examples
{
    class HunterBlackboardBrain : Blackboard
    {
        public float distanceToTarget = 0.0f;
    }

    public class AgentBrain : MonoBehaviour
    {
        [SerializeField] private BehaviorTree tree; // the behavior tree

        [Header("Object Settings")]
        public GameObject target; // our target object
        public GameObject father; // The parent from the agent
        public GameObject[] waypoints;

        [Header("Private")]
        [SerializeField] private HunterBlackboard blackboard; // the blackboard used to pass info to the behavior tree during updates
        [SerializeField] private bool doNotMove = false; // set to true to prevent the NPC from moving (debug option)

        [Header("Steering settings")]
        [SerializeField] private string label; // label to show when running
        [SerializeField] private GenericSteering settings; // de steering settings for all behaviour

        [Header("Steering runtime")]
        public Vector3 position = Vector3.zero; // current position       
        public Vector3 velocity = Vector3.zero; // current velocity      
        public Vector3 steerfor = Vector3.zero; //steering force

        private IBehavior[] behaviors = { }; // all behaviors for this steering object

        void SetBehaviors(IBehavior[] behavior, string lab)
        {
            //remember new settings
            behaviors = behavior;
            label = lab;

            //Start all behaviours
            foreach (IBehavior behav in behavior)
                behav.Start(new BehaviorContext(position, velocity, settings));
        }

        public void ForceWalk(Vector3 position)
        {
            SetBehaviors(
                new IBehavior[]
                {
                    new Steering.Arrive(position),
                    new Steering.Flock(father)
                },
                "Follow Path"
            );
        }

        //------------------------------------------------------------------------------------------
        // Unity overrides
        //------------------------------------------------------------------------------------------

        private void Awake()
        {
            father = father != null ? father : this.transform.parent.parent.gameObject; // Making sure it always has a parent
            position = transform.position; // Sets start position
        }

        private void Start()
        {
            // init blackboard
            blackboard = new HunterBlackboard();

            // prepare behavior tree
            tree = new BehaviorTree(BuildTree1(), blackboard, BlackboardUpdater) { Name = "SimpleTree" };
        }

        private void FixedUpdate()
        {
            steerfor = Vector3.zero;

            foreach (IBehavior behavior in behaviors)
                steerfor += behavior.CalculateSteeringForce(Time.fixedDeltaTime, new BehaviorContext(position, velocity, settings));

            steerfor.y = 0f;
            steerfor = Vector3.ClampMagnitude(steerfor, settings.maxSteeringforce);
            steerfor /= settings.mass;

            velocity = Vector3.ClampMagnitude(velocity + steerfor, settings.maxSpeed);

            position += velocity * Time.fixedDeltaTime;

            transform.position = doNotMove != true ? position : transform.position;
            transform.LookAt(position + Time.fixedDeltaTime * velocity);

            tree.Update(Time.deltaTime);
        }

        private void OnDrawGizmos()
        {
            Support.DrawWireDisc(transform.position, settings.approachRadius, Color.cyan);
            Support.DrawWireDisc(transform.position, settings.pursueRadius,   Color.cyan);

            Support.DrawRay(transform.position, velocity, Color.red);

            Support.DrawLabel(transform.position, label, Color.green);

            foreach (IBehavior behavior in behaviors)
                behavior.OnDrawGizmos(new BehaviorContext(position, velocity, settings));
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
                    
                    .Sequence("Follow Path")
                        .Condition("CanFollowPath", CanFollowPath)
                        .Do("ToFollowPath", ToFollowPath)
                        .RepeatUntilFailure("RepeatUntilFailure")
                            .Condition("CanFollowPath", CanFollowPath)
                        .End()
                    .End()

                    .Sequence("Idle")
                        .Do("ToIdle", ToIdle)
                        .RepeatUntilSuccess("RepeatUntilSuccess")
                            .Condition("InApproachRange", InApproachRange)
                        .End()
                    .End()

                .Build();
        }

        private void BlackboardUpdater(Blackboard bb)
        {
            // update distance to target
            if (target)
                (blackboard as HunterBlackboard).distanceToTarget = (target.transform.position - transform.position).magnitude;
            else
                (blackboard as HunterBlackboard).distanceToTarget = Mathf.Infinity;
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
            return (bb as HunterBlackboard).distanceToTarget < settings.approachRadius;
        }

        private bool InPursueRangeOnly(Blackboard bb)
        {
            return !InAttackRange(bb) && InPursueRange(bb);
        }

        private bool InPursueRange(Blackboard bb)
        {
            return (bb as HunterBlackboard).distanceToTarget < settings.pursueRadius;
        }

        private bool InAttackRange(Blackboard bb)
        {
            return (bb as HunterBlackboard).distanceToTarget < settings.attackRadius;
        }

        private bool CanFollowPath(Blackboard bb)
        {
            if (waypoints.Length > 0)
                return true;
            else
                return false;
        }


        private NodeState ToApproach(Blackboard bb)
        {
            SetBehaviors(
                new IBehavior[]
                {
                    new Steering.Seek(target),
                    new Steering.AvoidObstacle(),
                },
                "Approach w/ avoids"
            );

            return NodeState.SUCCESS;
        }

        private NodeState ToPursue(Blackboard bb)
        {
            SetBehaviors(
                new IBehavior[]
                {
                    new Steering.Pursue(target),
                    new Steering.AvoidObstacle()
                },
                "Pursue w/ avoids"
            );

            return NodeState.SUCCESS;
        }

        private NodeState ToIdle(Blackboard bb)
        {
            SetBehaviors(
                new IBehavior[]
                {
                    new Steering.Idle(),
                },
                "Idle"
            );

            return NodeState.SUCCESS;
        }

        private NodeState ToAttack(Blackboard bb)
        {
            SetBehaviors(
                new IBehavior[]
                {
                    new Steering.Idle(),
                },
                "Attack"
            );

            return NodeState.SUCCESS;
        }

        private NodeState ToFollowPath(Blackboard bb)
        {
            SetBehaviors(
                new IBehavior[]
                {
                    new Steering.FollowPath(waypoints)
                },
                "Follow Path"
            );

            Debug.Log("ok");

            return NodeState.SUCCESS;
        }
        #endregion
    }
}