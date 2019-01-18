using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AosHotfixRunTime
{
    public class Player : Unit
    {
        protected MainAttrib mMainAttrib;
        int mNavigating = 0;
        UnityEngine.AI.NavMeshAgent mNavMeshAgent;

        public Player(int unitID) : base(unitID)
        {
        }

        public override void Init()
        {
            base.Init();

            UpdateAttributes();
            mNavMeshAgent = UGameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();

            if (mNavMeshAgent != null)
            {
                mNavMeshAgent.enabled = false;
            }
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            if (mNavigating > 0)
                CheckNavigationEnd();
        }

        public override bool Hurt(Unit attacker, int damage, ACT.ECombatResult result)
        {
            return true;
        }

        public override int GetAttrib(EPA idx)
        {
            int ret = 0;

            return ret;
        }

        public void StartNavigation(Vector3 pos)
        {
            if (mNavMeshAgent)
            {
                mNavigating = 1;
                mNavMeshAgent.enabled = true;
                mNavMeshAgent.destination = pos;
                mNavMeshAgent.speed = GetAttrib(EPA.MoveSpeed) * 0.01f;

                PlayAction(Data1.CommonAction.RunInTown);
            }
            else
                SetPosition(pos);
        }

        void CheckNavigationEnd()
        {
            float pathEndThreshold = 0.1f;

            if (mNavigating > 10 &&
                (!mNavMeshAgent.hasPath || mNavMeshAgent.remainingDistance <= mNavMeshAgent.stoppingDistance + pathEndThreshold))
            {
                mNavigating = 0;
                mNavMeshAgent.enabled = false;
                PlayAction(Data1.CommonAction.IdleInTown);
                return;
            }

            mNavigating++;
        }
    }
}
