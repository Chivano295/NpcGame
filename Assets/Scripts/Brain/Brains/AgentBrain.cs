using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Steering;

namespace SimpleBehaviorTree.Examples
{
    public class AgentBrain : MonoBehaviour
    {
        [SerializeField] private BehaviorTree tree; // the behavior tree

        [Header("Object Settings")]
        [SerializeField] private GameObject   target; // our target object
        [SerializeField] private GameObject   group;
        [SerializeField] private GameObject   father; // The parent from the agent
        [SerializeField] private GameObject[] waypoints;
        [SerializeField] public  bool         team;

        [SerializeField] private float targetDistance = Mathf.Infinity;

        [Header("Private")]
        [SerializeField] private HunterBlackboard blackboard; // the blackboard used to pass info to the behavior tree during updates
        [SerializeField] private bool doNotMove = false; // set to true to prevent the NPC from moving (debug option)

        [Header("Steering settings")]
        [SerializeField] private string label; // label to show when running
        [SerializeField] private GenericSteering settings; // de steering settings for all behaviour

        [Header("Steering runtime")]
        public Vector3 position = Vector3.zero; // current position       
        public Vector3 velocity = Vector3.zero; // current velocity      
        public Vector3 steerfor = Vector3.zero; // steering force

        private IBehavior[] behaviors = { }; // all behaviors for this steering object

        void StopAllBehaviors()
        {
            foreach (IBehavior behav in behaviors)
                behav.Stop();
        }

        void SetBehaviors(IBehavior[] behavior, string lab)
        {
            StopAllBehaviors();

            //remember new settings
            behaviors = behavior;
            label = lab;

            //Start all behaviours
            foreach (IBehavior behav in behavior)
                behav.Start(new BehaviorContext(position, velocity, settings));
        }

        #region Custom functions

        List<Transform> GetChildren(Transform parent)
        {
            List<Transform> children = new List<Transform>();

            for (int i = 0; i < parent.childCount; i++)
                children.Add(parent.GetChild(i));

            return children;
        }

        Transform NearestEnemy()
        {
            Transform chosenOne = null;
            float distance = 999;

            foreach (Transform group in GetChildren(father.transform))
            {
                foreach (Transform unit in GetChildren(group))
                {
                    if (unit.GetComponent<MyTeam>().team == team)
                        continue;

                    float dist = Vector3.Distance(unit.position, this.position);

                    (distance, chosenOne) = dist < distance && unit != this.transform ? (dist, unit) : (distance, chosenOne);
                }
            }

            return chosenOne;
        }

        #endregion

        #region Unit Stats

        [Header("Unit Stats")]
        public int attackDamage = 1;
        public int attackSpeed  = 1;
        public int moveSpeed    = 16;
        public int viewRange    = 20;
        public int defense      = 50;
        public int hp           = 100;

        void Die()
        {
            if (group.transform.childCount == 0)
                Destroy(group);

            StopAllBehaviors();
            Destroy(this.gameObject);
        }

        public void TakeDamage(int damage)
        {
            if (Random.Range(1, 5) == 1)
                return;

            int damage2 = defense - damage;

            defense = damage2 <= 0 ? 0 : damage2;
            hp = damage2 < 0 ? hp + damage2 : hp;

            if (hp <= 0)
                Die();
        }

        public void AttackEnemy()
        {
            if (!target || !target.GetComponent<AgentBrain>())
                return;

            target.GetComponent<AgentBrain>().TakeDamage(attackDamage);
        }

        IEnumerator Attack()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(attackSpeed + Random.Range(-50, 50) / 10);

                if (label != "Attack")
                    continue;

                AttackEnemy();
            }
        }

        #endregion

        //------------------------------------------------------------------------------------------
        // Unity overrides
        //------------------------------------------------------------------------------------------

        private void Awake()
        {
            father = father != null ? father : this.transform.parent.parent.gameObject; // Making sure it always has a parent
            position = transform.position; // Sets start position
            team = GetComponent<MyTeam>().team;
            group = this.transform.parent.gameObject;
        }

        private void Start()
        {
            // init blackboard
            blackboard = new HunterBlackboard();

            // prepare behavior tree
            tree = new BehaviorTree(BuildTree1(), blackboard, BlackboardUpdater) { Name = "SimpleTree" };

            // Starts the attack loop
            StartCoroutine(Attack());
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

        private void Update()
        {
            Transform enemy = NearestEnemy();

            target = enemy != null ? enemy.gameObject : null;
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

                    .Sequence("Wander")
                        .Condition("CanWander", CanWander)
                        .Do("ToWander", ToWander)
                        .RepeatUntilFailure("RepeatUntilFailure")
                            .Condition("CanWander", CanWander)
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
            targetDistance = target ? (target.transform.position - this.transform.position).magnitude : Mathf.Infinity;
        }

        //------------------------------------------------------------------------------------------
        // All methods used by the behavior tree
        //------------------------------------------------------------------------------------------

        #region Action methods linked to the behavior tree
        private bool InApproachRangeOnly(Blackboard bb) { return !InPursueRange(bb) && InApproachRange(bb); }
        private bool InApproachRange(Blackboard bb)     { return targetDistance < settings.approachRadius;  }
        private bool InPursueRangeOnly(Blackboard bb)   { return !InAttackRange(bb) && InPursueRange(bb);   }
        private bool InPursueRange(Blackboard bb)       { return targetDistance < settings.pursueRadius;    }
        private bool InAttackRange(Blackboard bb)       { return targetDistance < settings.attackRadius;    }
        private bool CanFollowPath(Blackboard bb)       { return waypoints.Length > 0;                      }
        private bool CanWander(Blackboard bb)           { return targetDistance > settings.approachRadius;  }

        private NodeState ToApproach(Blackboard bb)
        {
            SetBehaviors(
                new IBehavior[] { new Seek(target), new AvoidObstacle(), new Flock(this.transform.parent.gameObject, this) },
                "Approach w/ avoids"
            );

            return NodeState.SUCCESS;
        }

        private NodeState ToPursue(Blackboard bb)
        {
            SetBehaviors(
                new IBehavior[] { new Pursue(target), new AvoidObstacle(), new Flock(this.transform.parent.gameObject, this) },
                "Pursue w/ avoids"
            );

            return NodeState.SUCCESS;
        }

        private NodeState ToIdle(Blackboard bb)
        {
            SetBehaviors(
                new IBehavior[] { new Idle() },
                "Idle"
            );

            return NodeState.SUCCESS;
        }

        private NodeState ToWander(Blackboard bb)
        {
            SetBehaviors(
                new IBehavior[] { new Wander(this.transform), new AvoidObstacle(), new AvoidWall() },
                "Wander"
            );

            return NodeState.SUCCESS;
        }    

        private NodeState ToAttack(Blackboard bb)
        {
            SetBehaviors(
                new IBehavior[] { new Idle() },
                "Attack"
            );

            return NodeState.SUCCESS;
        }

        private NodeState ToFollowPath(Blackboard bb)
        {
            SetBehaviors(
                new IBehavior[] { new FollowPath(waypoints) }, 
                "Follow Path"
            );

            return NodeState.SUCCESS;
        }
        #endregion
    }
}