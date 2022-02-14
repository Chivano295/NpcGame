using System.Collections.Generic;
using UnityEngine;

namespace Steering
{
    using ColliderList = List<Collider>;
    using HideList     = List<Vector3>;

    public class Hide : Behavior 
    {
        readonly private GameObject target; // the target object we are hiding from
                                 
        // info used to find a hiding place and draw gizmos 
        private ColliderList colliders;     // all the colliders in the scene that match the hide layer
        private HideList     hidingPlaces;  // the list with hiding places
        private Vector3      hidingPlace;   // the current hiding place

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        public Hide(GameObject targ) 
        {
            target = targ;
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        override public void Start(BehaviorContext context)
        {
            base.Start(context);

            // find all obstacles that match our hide layer name
            colliders = FindCollidersWithLayer(context.settings.hideLayer);
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        static public List<Collider> FindCollidersWithLayer(string layerName)
        {
            // get layer mask using the hide layer name
            int colliderLayer = LayerMask.NameToLayer(layerName);

            // get all collider game objects in the scene and find the ones with our layer
            Collider[] allColliders = GameObject.FindObjectsOfType(typeof(Collider)) as Collider[];
            List<Collider> colliders = new List<Collider>();
            foreach (Collider gameObject in allColliders)
            {
                if (gameObject.gameObject.layer == colliderLayer)
                    colliders.Add(gameObject);
            }

            // return the colliders that match the layer name
            return colliders;
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        public Vector3 CalculateHidingPlace(BehaviorContext context, Collider collider, Vector3 enemy_position)
        {
            // calculate place for the current obstacle
            Vector3 obstacleDirection = (collider.transform.position - enemy_position).normalized;
            Vector3 pointOtherSide    =  collider.transform.position + obstacleDirection;
            Vector3 hidingPlace       =  collider.ClosestPoint(pointOtherSide) + (obstacleDirection * context.settings.hideOffset);

            // return hiding place
            return hidingPlace;
        }

        public Vector3 CalculateHidingPlace(BehaviorContext context, Vector3 enemy_position)
        {
            // loop over colliders, find all hiding places, and find the nearest hiding place
            float closestDistanceSqr = float.MaxValue;
            hidingPlace            = context.position;
            hidingPlaces           = new HideList();
            for (int i = 0; i < colliders.Count; i++)
            {
                // calculate hiding place for the current obstacle and remember it so we can draw gizmos
                Vector3 hideplace = CalculateHidingPlace(context, colliders[i], enemy_position);
                hidingPlaces.Add(hideplace);

                // update closest hiding place if this hiding place is closer than the previous
                float distanceToHidingPlaceSqr = (context.position - hideplace).sqrMagnitude;
                if (distanceToHidingPlaceSqr < closestDistanceSqr)
                {
                    closestDistanceSqr = distanceToHidingPlaceSqr; // we have a new closest point
                    hidingPlace        = hideplace;              // remember it as the new hiding place
                }
            }

            // return hiding place 
            return hidingPlace;
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        override public Vector3 CalculateSteeringForce(float dt, BehaviorContext context)
        {
            // update target position plus desired velocity and return steering force    
            positionTarget  = CalculateHidingPlace(context, target.transform.position);
            if (ArriveEnabled(context) && WithinArriveSlowingDistance(context, positionTarget))
                velocityDesired = CalculateArriveSteeringForce(context, positionTarget);
            else
                velocityDesired = (positionTarget - context.position).normalized * context.settings.maxDesiredVelocity;
            return velocityDesired - context.velocity;
        }

        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        override public void OnDrawGizmos(BehaviorContext context)
        {
            base.OnDrawGizmos(context);

            // draw solid discs at all possible hiding places and the selected one
            foreach (Vector3 hidingPlace in hidingPlaces)
                Support.DrawSolidDisc(hidingPlace, 0.25f, Color.blue);

            Support.DrawWireDisc(hidingPlace, 0.35f, Color.blue);

            if (ArriveEnabled(context))
                OnDrawArriveGizmos(context);
        }
    }
}
