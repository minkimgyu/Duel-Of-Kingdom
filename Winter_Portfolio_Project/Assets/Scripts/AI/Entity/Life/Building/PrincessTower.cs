using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.AI.ACTION.STATE;
using WPP.AI.CAPTURE;
using WPP.AI.FSM;
using WPP.AI.GRID;
using System;
using WPP.Battle;

namespace WPP.AI.BUILDING
{
    public class PrincessTower : Tower
    {
        // Ready --> Active State로 전환됨
        bool _isLeft;
        Action<TowerCondition> OnTowerConditionChangeRequested;

        public override void Initialize(int level, string name, float hp, OffsetRect fillOffset, CaptureTag[] targetTag, float damage, float hitSpeed, float range, float captureRange)
        {
            base.Initialize(level, name, hp, fillOffset, targetTag, damage, hitSpeed, range, captureRange);

            OnTxtVisibleRequested?.Invoke(true);

            Dictionary<ActionState, BaseState> attackStates = new Dictionary<ActionState, BaseState>()
            {
                {ActionState.Active, new ActiveState(this)}
            };

            InitializeFSM(attackStates, ActionState.Active);

            GridController gridController = FindObjectOfType<GridController>();
            OnTowerConditionChangeRequested = gridController.OnTowerConditionChange;
        }

        public override void IsLeft(bool isLeft) => _isLeft = isLeft;

        void ActiveKingTower()
        {
            KingTower[] kingTowers = FindObjectsOfType<KingTower>();
            if (kingTowers.Length == 0) return;

            for (int i = 0; i < kingTowers.Length; i++)
            {
                if (kingTowers[i].OwnershipId != OwnershipId) continue;

                kingTowers[i].FSM.OnActive();
                return;
            }
        }

        public override void Die()
        {
            ActiveKingTower(); // 킹 타워 오픈

            if (IsMyEntity == false) // 내 타워가 아닌 경우
            {
                if (_isLeft) OnTowerConditionChangeRequested?.Invoke(TowerCondition.LeftDestroy);
                else OnTowerConditionChangeRequested?.Invoke(TowerCondition.RightDestroy);
            }

            if(_isLeft) BattleManager.Instance().GetPlayerOfEntity(this).TowerSystem.DestroyLeftPrincessTower();
            else BattleManager.Instance().GetPlayerOfEntity(this).TowerSystem.DestroyRightPrincessTower();

            base.Die();
            // FillComponent에 이벤트 호출
            // KingTower 열어주기
        }
    }
}
