using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.AI.ACTION.STATE;
using WPP.AI.CAPTURE;
using WPP.AI.FSM;
using WPP.AI.GRID;
using WPP.Battle;

namespace WPP.AI.BUILDING
{
    public class KingTower : Tower
    {
        // Natural --> Active State로 전환됨

        public override void Initialize(int level, string name, float hp, OffsetRect fillOffset, CaptureTag[] targetTag, float damage, float hitSpeed, float range, float captureRange)
        {
            base.Initialize(level, name, hp, fillOffset, targetTag, damage, hitSpeed, range, captureRange);

            OnTxtVisibleRequested?.Invoke(true);

            Dictionary<ActionState, BaseState> attackStates = new Dictionary<ActionState, BaseState>()
            {
                {ActionState.Neutral, new NeutralState(this)},
                {ActionState.Active, new ActiveState(this)}
            };

            InitializeFSM(attackStates, ActionState.Neutral);
        }

        public override void Die()
        {
            BattleManager.Instance().GetPlayerOfEntity(this).TowerSystem.DestroyKingTower();
            // 여기에 게임 종료 시퀀스 넣어주기
            base.Die();
        }
    }
}
