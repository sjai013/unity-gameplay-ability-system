using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D
{
    // This class allow to distribute arc around a target, used for "crowding" by ennemis, so they all
    // come at the player (or any target) from different direction.
    [DefaultExecutionOrder(-1)]
    public class TargetDistributor : MonoBehaviour
    {
        //Use as a mean to communicate between this target and the followers
        public class TargetFollower
        {
            //target should set that to true when they require the system to give them a position
            public bool requireSlot;
            //will be -1 if none is currently assigned
            public int assignedSlot;
            //the position the follower want to reach for the target.
            public Vector3 requiredPoint;

            public TargetDistributor distributor;

            public TargetFollower(TargetDistributor owner)
            {
                distributor = owner;
                requiredPoint = Vector3.zero;
                requireSlot = false;
                assignedSlot = -1;
            }
        }

        public int arcsCount;

        protected Vector3[] m_WorldDirection;

        protected bool[] m_FreeArcs;
        protected float arcDegree;

        protected List<TargetFollower> m_Followers;

        public void OnEnable()
        {
            m_WorldDirection = new Vector3[arcsCount];
            m_FreeArcs = new bool[arcsCount];

            m_Followers = new List<TargetFollower>();

            arcDegree = 360.0f / arcsCount;
            Quaternion rotation = Quaternion.Euler(0, -arcDegree, 0);
            Vector3 currentDirection = Vector3.forward;
            for (int i = 0; i < arcsCount; ++i)
            {
                m_FreeArcs[i] = true;
                m_WorldDirection[i] = currentDirection;
                currentDirection = rotation * currentDirection;
            }
        }

        public TargetFollower RegisterNewFollower()
        {
            TargetFollower follower = new TargetFollower(this);
            m_Followers.Add(follower);
            return follower;
        }

        public void UnregisterFollower(TargetFollower follower)
        {
            if (follower.assignedSlot != -1)
            {
                m_FreeArcs[follower.assignedSlot] = true;
            }


            m_Followers.Remove(follower);
        }

        //at the end of the frame, we distribute target position to all follower that asked for one.
        private void LateUpdate()
        {
            for (int i = 0; i < m_Followers.Count; ++i)
            {
                var follower = m_Followers[i];

                //we free whatever arc this follower may already have. 
                //If it still need it, it will be picked again next lines.
                //if it changed position the new one will be picked.
                if (follower.assignedSlot != -1)
                {
                    m_FreeArcs[follower.assignedSlot] = true;
                }

                if (follower.requireSlot)
                {
                    follower.assignedSlot = GetFreeArcIndex(follower);
                }
            }
        }

        public Vector3 GetDirection(int index)
        {
            return m_WorldDirection[index];
        }

        public int GetFreeArcIndex(TargetFollower follower)
        {
            bool found = false;

            Vector3 wanted = follower.requiredPoint - transform.position;
            Vector3 rayCastPosition = transform.position + Vector3.up * 0.4f;

            wanted.y = 0;
            float wantedDistance = wanted.magnitude;

            wanted.Normalize();

            float angle = Vector3.SignedAngle(wanted, Vector3.forward, Vector3.up);
            if (angle < 0)
                angle = 360 + angle;

            int wantedIndex = Mathf.RoundToInt(angle / arcDegree);
            if (wantedIndex >= m_WorldDirection.Length)
                wantedIndex -= m_WorldDirection.Length;

            int choosenIndex = wantedIndex;

            RaycastHit hit;
            if (!Physics.Raycast(rayCastPosition, GetDirection(choosenIndex), out hit, wantedDistance))
                found = m_FreeArcs[choosenIndex];

            if (!found)
            {//we are going to test left right with increasing offset
                int offset = 1;
                int halfCount = arcsCount / 2;
                while (offset <= halfCount)
                {
                    int leftIndex = wantedIndex - offset;
                    int rightIndex = wantedIndex + offset;

                    if (leftIndex < 0) leftIndex += arcsCount;
                    if (rightIndex >= arcsCount) rightIndex -= arcsCount;

                    if (!Physics.Raycast(rayCastPosition, GetDirection(leftIndex), wantedDistance) &&
                        m_FreeArcs[leftIndex])
                    {
                        choosenIndex = leftIndex;
                        found = true;
                        break;
                    }

                    if (!Physics.Raycast(rayCastPosition, GetDirection(rightIndex), wantedDistance) &&
                        m_FreeArcs[rightIndex])
                    {
                        choosenIndex = rightIndex;
                        found = true;
                        break;
                    }

                    offset += 1;
                }
            }

            if (!found)
            {//we couldn't find a free direction, return -1 to tell the caller there is no free space
                return -1;
            }

            m_FreeArcs[choosenIndex] = false;
            return choosenIndex;
        }

        public void FreeIndex(int index)
        {
            m_FreeArcs[index] = true;
        }
    }

}