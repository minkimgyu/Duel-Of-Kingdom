using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPP.AI.ACTION.STATE;
using WPP.AI.CAPTURE;
using WPP.AI.FSM;
using WPP.GRID;

namespace WPP.AI.BUILDING
{
    public class PrincessTower : AttackBuilding
    {
        // Ready --> Active State·Î ÀüÈ¯µÊ

        public override void Initialize(int id, int level, string name, float hp, OffsetFromCenter fillOffset, CaptureTag[] targetTag, float damage, float hitSpeed, float range, float captureRange)
        {
            base.Initialize(id, level, name, hp, fillOffset, targetTag, damage, hitSpeed, range, captureRange);

            OnTxtVisibleRequested?.Invoke(true);

            Dictionary<ActionState, BaseState> attackStates = new Dictionary<ActionState, BaseState>()
            {
                {ActionState.Active, new ActiveState(this)}
            };

            InitializeFSM(attackStates, ActionState.Active);
        }
    }
}
