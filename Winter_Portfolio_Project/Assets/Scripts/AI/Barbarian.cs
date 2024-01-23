using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using WPP.AI.UNIT;
using WPP.AI.ATTACK;
using WPP.AI.BTUtility;

namespace WPP.AI
{
    public class Barbarian : GroundUnit
    {
        MeleeAttackComponent _meleeAttackComponent;

        protected override void Start()
        {
            _meleeAttackComponent = GetComponent<MeleeAttackComponent>();
            base.Start();
        }

        protected override void InitializeBT()
        {
            GameObject go = GameObject.FindWithTag("GridManager");
            if (go == null) return;

            PathFinder pathFinder = go.GetComponent<PathFinder>();
            if (pathFinder == null) return;

            List<Node> _childNodes = new List<Node>()
            {
                new Selector
                (
                    new List<Node>()
                    {
                        new Sequence
                        (
                            new List<Node>
                            {
                                new CanFindTarget(_captureComponent), // 만약 타겟이 없다면 타워를 타겟으로 지정해준다.
                                new Selector
                                (
                                    new List<Node>
                                    {
                                        new Selector
                                        (
                                            new List<Node>
                                            {
                                                new Sequence
                                                (
                                                    new List<Node>
                                                    {
                                                        // 타겟이 바뀌는 경우, 일정 시간 딜레이 넣어주기
                                                        new CheckIsNearAndCancelAttackWhenExit(_captureComponent, 1.5f, 0.3f, _meleeAttackComponent), // DelayForAttack도 넣어주기
                                                        new LookAtTarget(_captureComponent, _viewComponent),
                                                        new Stop(_moveComponent),

                                                        new Attack(_meleeAttackComponent, _captureComponent)
                                                        // 공격 진행
                                                        // 만약 공격이 진행 중인 경우 거리가 멀어져도 계속 진행
                                                    }
                                                ),
                                                new Sequence
                                                (
                                                    new List<Node>
                                                    {
                                                        new CheckIsNear(_captureComponent, 2.5f),
                                                        new GoDirectToPoint(_captureComponent, _moveComponent, _viewComponent, ResetPosListForDrawingGizmo, IsPosListEmpty)
                                                    }
                                                )
                                            }
                                        ),
                                        new FollowPath(_moveComponent, _viewComponent, _captureComponent, pathFinder, ResetPosListForDrawingGizmo)
                                    }
                                )
                            }
                        ),
                        new Stop(_moveComponent)
                    }
                )
            };

            Node rootNode = new Selector(_childNodes);
            _bt.SetUp(rootNode);
        }
    }
}